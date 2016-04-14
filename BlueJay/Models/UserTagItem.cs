using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueJay
{
    // Json Serializable
    // Result from GET /apis/core_lookup/list_voter_tags 
    public class UserTagItem
    {
        public int id { get; set; }
        public string userid { get; set; }
        public string user_fullname { get; set; }
        public string key { get; set; }
        public string state { get; set; }
        public string rncid { get; set; }
        public string voterkey { get; set; }
        public string statevoterid { get; set; }
        public string jurisdictionid { get; set; }
        public string datacenterid { get; set; }
        public string tagid { get; set; }
        public string tagname { get; set; }
        public string createdby { get; set; }
        public string rnc_regid { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1})", this.tagname, this.tagid);
        }
    }
}