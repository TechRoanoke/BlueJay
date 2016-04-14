using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BlueJay
{
    // Helper methods on the interface. 
    public static class BlueJayClientExtensions
    {
        // Enumerate through all segments to get all voters.
        // This could take a while on large lists. 
        // invoke progressCallback(current, total) to provide progress on the large download. 
        public static async Task<VoterResult[]> GetAllVotersAsync(this IBlueJayClient client, ExtendedLookupParameters query, Action<int,int> progressCallback = null)
        {
            List<VoterResult> all = new List<VoterResult>();
            
            string continuationToken= null;
            while(true)
            {
                var segment = await client.GetExtendedVoterLookupAsync(query, continuationToken);
                all.AddRange(segment.results);
                if (progressCallback != null)
                {
                    progressCallback(all.Count, segment.count);
                }

                if (segment.next == null)
                {
                    break;
                }
                if (continuationToken == segment.next)
                {
                    // If we get the same continuation token twice in a row, then we're at the end. 
                    break;
                }
                continuationToken = segment.next;
            }

            return all.ToArray();
        }


        // Convenience method to get a ContactDetails key for posting survey responses. 
        public static Task<VoterContactResponse> PostVoterContactAsync(this IBlueJayClient client, VoterResult voterResult)
        {
            var contactRequest = voterResult.NewContact();
            return client.PostVoterContactAsync(contactRequest);
        }
    }
}