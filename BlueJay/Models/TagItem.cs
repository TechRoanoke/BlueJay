using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueJay
{
    // Json Serializable
    public class TagItem
    {
        public string elementname { get; set; }
        public string elementdescription { get; set; }
        public long elementid { get; set; }
        public string year { get; set; }

        public override string ToString()
        {
            return this.elementname;
        }
    }

}
