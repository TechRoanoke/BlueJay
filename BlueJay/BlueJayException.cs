using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BlueJay
{
    // Track an HTTP exception from the server
    public class BlueJayException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public BlueJayException(HttpStatusCode code, string message)
            : base(message)
        {
            this.StatusCode = code;
        }
    }
}