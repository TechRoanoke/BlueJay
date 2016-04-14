using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueJay
{
    // Don't serialize
    // possible query criteria for doing a lookup to /apis/core_lookup/extended_voterlookup    
    public class ExtendedLookupParameters
    {
        public string statelowerhousedistrict { get; set; }
        public string stateupperhousedistrict { get; set; }
        public string registrationcity { get; set; }
        public string state { get; set; }

        public string firstname { get; set; }

        public string lastname { get; set; }
        public string registrationzip5 { get; set; }

        public Gender? sex { get; set; }   
    }
}