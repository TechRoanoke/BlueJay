using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BlueJay
{
    /// <summary>
    /// Common methods and properties for all AWS4 signer variants
    /// </summary>
    public class AWS4SignerBase
    {
        // SHA256 hash of an empty request body
        public const string EMPTY_BODY_SHA256 = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

        public const string SCHEME = "AWS4";
        public const string ALGORITHM = "HMAC-SHA256";
        public const string TERMINATOR = "aws4_request";

        // format strings for the date/time and date stamps required during signing
        public const string ISO8601BasicFormat = "yyyyMMddTHHmmssZ";
        public const string DateStringFormat = "yyyyMMdd";

        // some common x-amz-* parameters
        public const string X_Amz_Algorithm = "X-Amz-Algorithm";
        public const string X_Amz_Credential = "X-Amz-Credential";
        public const string X_Amz_SignedHeaders = "X-Amz-SignedHeaders";
        public const string X_Amz_Date = "X-Amz-Date";
        public const string X_Amz_Signature = "X-Amz-Signature";
        public const string X_Amz_Expires = "X-Amz-Expires";
        public const string X_Amz_Content_SHA256 = "X-Amz-Content-SHA256";
        public const string X_Amz_Decoded_Content_Length = "X-Amz-Decoded-Content-Length";
        public const string X_Amz_Meta_UUID = "X-Amz-Meta-UUID";

        // the name of the keyed hash algorithm used in signing
        public const string HMACSHA256 = "HMACSHA256";

        // request canonicalization requires multiple whitespace compression
        protected static readonly Regex CompressWhitespaceRegex = new Regex("\\s+");

        // algorithm used to hash the canonical request that is supplied to
        // the signature computation
        public static HashAlgorithm CanonicalRequestHashAlgorithm = HashAlgorithm.Create("SHA-256");


        // Key info for signing. 
        private Uri _endpoint;
        private string _clientId;
        private string _secret;

        public AWS4SignerBase(Uri endpoint,
            string clientId,
            string secret)
        {
            this._endpoint = endpoint;
            this._clientId = clientId;
            this._secret = secret;
        }

        private static string BuildQuery(IDictionary<string, string> queryParams)
        {
            // Sigining wants query parameters in alphabetical order 
            string[] names = queryParams.Keys.ToArray();
            string[] values = queryParams.Values.ToArray();

            Array.Sort(names, values);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < names.Length; i++)
            {
                if (sb.Length > 0)
                {
                    sb.Append('&');
                }
                sb.AppendFormat("{0}={1}", names[i], values[i]);
            }
            return sb.ToString();
        }

        // Signinging requires Query parameters must be in alpha
        public HttpRequestMessage CreateSignedRequest(
            HttpMethod method,
            string path, // Url to invoke, with query parameters. 
            IDictionary<string, string> queryParams = null,
            string postBody = null// NULL for GET
            )
        {
            //string path = new Uri(pathAndQuery).P

            var requestDateTime = DateTime.UtcNow;
            var dateTimeStamp = requestDateTime.ToString(ISO8601BasicFormat, CultureInfo.InvariantCulture);

            // update the headers with required 'x-amz-date' and 'host' values
            Dictionary<string, string> headerParams = new Dictionary<string, string>();
            headerParams.Add("Accept", "application/json");
            headerParams.Add("X-Amz-Date", dateTimeStamp);
            headerParams.Add("Host", _endpoint.Host);

            string fullUrl;
            string canonicalizedQueryParameters = string.Empty;
            if (queryParams != null)
            {
                canonicalizedQueryParameters = BuildQuery(queryParams);
                fullUrl = _endpoint + path + "?" + canonicalizedQueryParameters;
            }
            else
            {
                fullUrl =  _endpoint + path;
            }

            HttpRequestMessage rq = new HttpRequestMessage(method, new Uri(fullUrl));
            foreach(var kv in headerParams)
            {
                rq.Headers.Add(kv.Key, kv.Value);
            }

            string bodyHash = "";
            if (postBody != null)
            {
                bodyHash = postBody;
                rq.Content = new StringContent(postBody, Encoding.UTF8, "application/json");
                //request.AddParameter("application/json", postBody, ParameterType.RequestBody);
            }

            HashAlgorithm algorithm = new SHA256Managed();
            byte[] crypto = algorithm.ComputeHash(Encoding.UTF8.GetBytes((bodyHash)));
            bodyHash = "";
            foreach (byte b in crypto)
            {
                bodyHash += b.ToString("x2");
            }

            // canonicalize the headers; we need the set of header names as well as the
            // names and values to go into the signature process
            var canonicalizedHeaderNames = CanonicalizeHeaderNames(headerParams);
            var canonicalizedHeaders = CanonicalizeHeaders(headerParams);

          

            // canonicalize the various components of the request
            var canonicalRequest = CanonicalizeRequest(new Uri(_endpoint + path),
                method.ToString(),
                canonicalizedQueryParameters,
                canonicalizedHeaderNames,
                canonicalizedHeaders,
                bodyHash);

            // generate a hash of the canonical request, to go into signature computation
			var canonicalRequestHashBytes
			= CanonicalRequestHashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(canonicalRequest));

            	// construct the string to be signed
			var stringToSign = new StringBuilder();

			var dateStamp = requestDateTime.ToString(DateStringFormat, CultureInfo.InvariantCulture);
			var scope = string.Format("{0}/{1}/{2}/{3}",
				dateStamp,
				"us-east-1",
				"execute-api",
				TERMINATOR);

			stringToSign.AppendFormat("{0}-{1}\n{2}\n{3}\n", SCHEME, ALGORITHM, dateTimeStamp, scope);
			stringToSign.Append(ToHexString(canonicalRequestHashBytes, true));
				var kha = KeyedHashAlgorithm.Create(HMACSHA256);
			kha.Key = DeriveSigningKey (HMACSHA256, _secret, "us-east-1", dateStamp);
			// compute the AWS4 signature and return it
			var signature = kha.ComputeHash(Encoding.UTF8.GetBytes(stringToSign.ToString()));
			var signatureString = ToHexString(signature, true);
		    var authString = new StringBuilder();
			authString.AppendFormat("{0}-{1} ", SCHEME, ALGORITHM);
            //string scheme = string.Format("{0}-{1} ", SCHEME, ALGORITHM);
			authString.AppendFormat("Credential={0}/{1}, ", _clientId, scope);
			authString.AppendFormat("SignedHeaders={0}, ", canonicalizedHeaderNames);
			authString.AppendFormat("Signature={0}", signatureString);

			var authorization = authString.ToString();
        	//request.AddHeader ("Authorization", authorization);
            //return request;

            //rq.Headers.Add("Authorization", authorization);

            // Note this generates a header with illegal characters, so we need to skip validation. 
            rq.Headers.TryAddWithoutValidation("Authorization", authorization);
            //rq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(scheme, authorization);
            return rq;
        }

        /// <summary>
        /// Returns the canonical collection of header names that will be included in
        /// the signature. For AWS4, all header names must be included in the process 
        /// in sorted canonicalized order.
        /// </summary>
        /// <param name="headers">
        /// The set of header names and values that will be sent with the request
        /// </param>
        /// <returns>
        /// The set of header names canonicalized to a flattened, ;-delimited string
        /// </returns>
        protected string CanonicalizeHeaderNames(IDictionary<string, string> headers)
        {
            var headersToSign = new List<string>(headers.Keys);
            headersToSign.Sort(StringComparer.OrdinalIgnoreCase);

            var sb = new StringBuilder();
            foreach (var header in headersToSign)
            {
                if (sb.Length > 0)
                    sb.Append(";");
                sb.Append(header.ToLower());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Computes the canonical headers with values for the request. 
        /// For AWS4, all headers must be included in the signing process.
        /// </summary>
        /// <param name="headers">The set of headers to be encoded</param>
        /// <returns>Canonicalized string of headers with values</returns>
        protected virtual string CanonicalizeHeaders(IDictionary<string, string> headers)
        {
            if (headers == null || headers.Count == 0)
                return string.Empty;

            // step1: sort the headers into lower-case format; we create a new
            // map to ensure we can do a subsequent key lookup using a lower-case
            // key regardless of how 'headers' was created.
            var sortedHeaderMap = new SortedDictionary<string, string>();
            foreach (var header in headers.Keys)
            {
                sortedHeaderMap.Add(header.ToLower(), headers[header]);
            }

            // step2: form the canonical header:value entries in sorted order. 
            // Multiple white spaces in the values should be compressed to a single 
            // space.
            var sb = new StringBuilder();
            foreach (var header in sortedHeaderMap.Keys)
            {
                var headerValue = CompressWhitespaceRegex.Replace(sortedHeaderMap[header], " ");
                sb.AppendFormat("{0}:{1}\n", header, headerValue.Trim());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the canonical request string to go into the signer process; this 
        /// consists of several canonical sub-parts.
        /// </summary>
        /// <param name="endpointUri"></param>
        /// <param name="httpMethod"></param>
        /// <param name="queryParameters"></param>
        /// <param name="canonicalizedHeaderNames">
        /// The set of header names to be included in the signature, formatted as a flattened, ;-delimited string
        /// </param>
        /// <param name="canonicalizedHeaders">
        /// </param>
        /// <param name="bodyHash">
        /// Precomputed SHA256 hash of the request body content. For chunked encoding this
        /// should be the fixed string ''.
        /// </param>
        /// <returns>String representing the canonicalized request for signing</returns>
        protected string CanonicalizeRequest(Uri endpointUri,
                                             string httpMethod,
                                             string queryParameters,
                                             string canonicalizedHeaderNames,
                                             string canonicalizedHeaders,
                                             string bodyHash)
        {
            var canonicalRequest = new StringBuilder();

            canonicalRequest.AppendFormat("{0}\n", httpMethod);
            canonicalRequest.AppendFormat("{0}\n", CanonicalResourcePath(endpointUri));
            canonicalRequest.AppendFormat("{0}\n", queryParameters);

            canonicalRequest.AppendFormat("{0}\n", canonicalizedHeaders);
            canonicalRequest.AppendFormat("{0}\n", canonicalizedHeaderNames);

            canonicalRequest.Append(bodyHash);

            return canonicalRequest.ToString();
        }

        /// <summary>
        /// Returns the canonicalized resource path for the service endpoint
        /// </summary>
        /// <param name="endpointUri">Endpoint to the service/resource</param>
        /// <returns>Canonicalized resource path for the endpoint</returns>
        protected string CanonicalResourcePath(Uri endpointUri)
        {
            if (string.IsNullOrEmpty(endpointUri.AbsolutePath))
                return "/";

            // encode the path per RFC3986
            return UrlEncode(endpointUri.AbsolutePath, true);
        }

        /// <summary>
        /// Helper routine to url encode canonicalized header names and values for safe
        /// inclusion in the presigned url.
        /// </summary>
        /// <param name="data">The string to encode</param>
        /// <param name="isPath">Whether the string is a URL path or not</param>
        /// <returns>The encoded string</returns>
        public static string UrlEncode(string data, bool isPath = false)
        {
            // The Set of accepted and valid Url characters per RFC3986. Characters outside of this set will be encoded.
            const string validUrlCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

            var encoded = new StringBuilder(data.Length * 2);
            string unreservedChars = String.Concat(validUrlCharacters, (isPath ? "/:" : ""));

            foreach (char symbol in System.Text.Encoding.UTF8.GetBytes(data))
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                    encoded.Append(symbol);
                else
                    encoded.Append("%").Append(String.Format("{0:X2}", (int)symbol));
            }

            return encoded.ToString();
        }

        /// <summary>
        /// Compute and return the multi-stage signing key for the request.
        /// </summary>
        /// <param name="algorithm">Hashing algorithm to use</param>
        /// <param name="awsSecretAccessKey">The clear-text AWS secret key</param>
        /// <param name="region">The region in which the service request will be processed</param>
        /// <param name="date">Date of the request, in yyyyMMdd format</param>
        /// <param name="service">The name of the service being called by the request</param>
        /// <returns>Computed signing key</returns>
        protected byte[] DeriveSigningKey(string algorithm, string awsSecretAccessKey, string region, string date)
        {
            const string ksecretPrefix = SCHEME;
            char[] ksecret = null;

            ksecret = (ksecretPrefix + awsSecretAccessKey).ToCharArray();

            byte[] hashDate = ComputeKeyedHash(algorithm, Encoding.UTF8.GetBytes(ksecret), Encoding.UTF8.GetBytes(date));
            byte[] hashRegion = ComputeKeyedHash(algorithm, hashDate, Encoding.UTF8.GetBytes(region));
            byte[] hashService = ComputeKeyedHash(algorithm, hashRegion, Encoding.UTF8.GetBytes("execute-api"));
            return ComputeKeyedHash(algorithm, hashService, Encoding.UTF8.GetBytes(TERMINATOR));
        }

        /// <summary>
        /// Compute and return the hash of a data blob using the specified algorithm
        /// and key
        /// </summary>
        /// <param name="algorithm">Algorithm to use for hashing</param>
        /// <param name="key">Hash key</param>
        /// <param name="data">Data blob</param>
        /// <returns>Hash of the data</returns>
        protected byte[] ComputeKeyedHash(string algorithm, byte[] key, byte[] data)
        {
            var kha = KeyedHashAlgorithm.Create(algorithm);
            kha.Key = key;
            return kha.ComputeHash(data);
        }

        /// <summary>
        /// Helper to format a byte array into string
        /// </summary>
        /// <param name="data">The data blob to process</param>
        /// <param name="lowercase">If true, returns hex digits in lower case form</param>
        /// <returns>String version of the data</returns>
        public static string ToHexString(byte[] data, bool lowercase)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString(lowercase ? "x2" : "X2"));
            }
            return sb.ToString();
        }
    }
}
