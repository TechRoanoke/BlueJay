using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlueJay
{
    // Use when sending responses:
    // POST /apis/core_lookup/voter_contact_details 
    public class VoterContactDetailRequest
    {
        public string question { get; set; }

        public string answer { get; set; }

        public string tagname { get; set; }

        public string api_votercontactkey { get; set; }
    }
}
