using Newtonsoft.Json;
namespace BlueJay
{
    // Result from 
    // GET /apis/core_lookup/extended_voterlookup
    public class VoterResult
    {
        public string id { get; set; }

        public VoterFields fields { get; set; }
   
        public override string ToString()
        {
            return this.fields.ToString();
        }     
    }

    public static class VoterResultExtensions
    {
        // Helper to create a voter contact record for a given voter record. 
        public static VoterContactRequest NewContact(this VoterResult voter)
        {
            return voter.fields.NewContact();
        }
    }
}