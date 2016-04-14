using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BlueJay
{
    public class BlueJayClient : ClientBase, IBlueJayClient
    {
        public BlueJayClient(Uri endpoint, string clientId, string secret)
            : base(endpoint, clientId, secret)
        {            
        }

        // GET apis/core_lookup/get_tag_list 
        public async Task<TagItem[]> GetSuggestedTagListAsync(string state)
        {
            var queryParams = new Dictionary<string, string>();
            queryParams["state"] = state;
            var request = MakeGetRequest("/apis/core_lookup/get_tag_list", queryParams);
            var values =  await this.SendJsonAsync<TagItem[]>(request);
            return values;
        }

        // Tags that I created with this access token. 
        // GET /apis/core_lookup/list_voter_tags 
        public async Task<UserTagItem[]> GetMyTagListAsync()
        {
            var queryParams = new Dictionary<string, string>();            
            var request = MakeGetRequest("/apis/core_lookup/list_voter_tags", queryParams);
            var values = await this.SendJsonAsync<UserTagItem[]>(request);
            return values;
        }


        // POST /apis/core_lookup/extended_voterlookup
        public async Task<Segment<VoterResult>> GetExtendedVoterLookupAsync(ExtendedLookupParameters query, string continuationToken = null)
        {
            var queryParams = new Dictionary<string, string>();
            Utility.ToQueryParams(queryParams, query);
            if (continuationToken != null)
            {
                queryParams["cursor"] = continuationToken;
            }

            var request = MakeGetRequest("/apis/core_lookup/extended_voterlookup", queryParams);
            var segment = await this.SendJsonAsync<Segment<VoterResult>>(request);
            return segment;
        }

        // POST /apis/core_lookup/voter_contacts 
        public async Task<VoterContactResponse> PostVoterContactAsync(VoterContactRequest voterContact)
        {
            var request = MakePostRequest("/apis/core_lookup/voter_contacts", voterContact);
            var contact = await this.SendJsonAsync<VoterContactResponse>(request);
            return contact;
        }

        // Add survey responses for the contact. 
        public async Task PostVoterContactDetailsAsync(params VoterContactDetailRequest[] details)
        {
            foreach (var detail in details)
            {
                var request = MakePostRequest("/apis/core_lookup/voter_contact_details", detail);
                var x = await this.SendJsonAsync<ApiStatusResponse>(request);
            }
        }
        
        private const string ISO8601_DATETIME_FORMAT = "o";

        private static string Convert(DateTime dateTime)
        {
            // Return a formatted date string - Can be customized with Configuration.DateTimeFormat
            // Defaults to an ISO 8601, using the known as a Round-trip date/time pattern ("o")
            // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx#Anchor_8
            // For example: 2009-06-15T13:45:30.0000000
            return dateTime.ToString(ISO8601_DATETIME_FORMAT);
        }
    }
}
