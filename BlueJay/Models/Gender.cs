using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BlueJay
{
    [JsonConverter(typeof(SafeEnumConverter<Gender>))]
    public enum Gender
    {
        U, // Unknown
        M, // Male
        F  // Female
    }
}
