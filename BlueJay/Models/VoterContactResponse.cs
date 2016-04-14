using Newtonsoft.Json;

namespace BlueJay
{
    // Response from 
    // POST /apis/core_lookup/voter_contacts 
    public class VoterContactResponse
    {
        // Use this key to post individual question responses for this voter. 
        public string API_VoterContactKey { get; set; }
    }

    public static class VoterContactResponseExtensions
    {
        public static VoterContactDetailRequest NewDetails(this VoterContactResponse response, string question, string answer, string tagname)
        {
            return new VoterContactDetailRequest
            {
                question = question,
                answer = answer,
                tagname = tagname,
                api_votercontactkey = response.API_VoterContactKey
            };
        }
    }
}