using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueJay
{
    // An empty status response 
    public class ApiStatusResponse
    {
        public string APIStatus { get; set; }

        // Status are human readable strings.
        // HAve a list of known goods.
        static HashSet<string> _successCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        static ApiStatusResponse()
        {
            _successCodes.Add("Successfully Added Contact Details");
        }

        [JsonIgnore]
        public bool IsSuccess
        {
            get { 
                return _successCodes.Contains(this.APIStatus); 
            }
        }
    }
}
