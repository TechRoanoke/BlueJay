using Newtonsoft.Json;

namespace BlueJay
{
    // Used to allocate a API_VoterContactKey that can then be used to upload survey responses.
    // Json serialize
    // POST /apis/core_lookup/voter_contacts 
    public class VoterContactRequest
    {
        public string state { get; set; }

        // Most important primary key. This is the rncRegId. 
        public string targetvoterkey { get; set; } 
        public string targetstatevoterid { get; set; }
    }
}