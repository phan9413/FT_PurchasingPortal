using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinPR.Helpers
{
    // an object to send over server 
    // to activate action and verification of token
    public class Cio
    {
        public string token { get; set; }
        public string request { get; set; } // used to process action

        /// <summary>
        /// Json ignore attribute 
        /// used to inform json to ignore this field, when convert object into json string
        /// this field with this annotation will be ignore and continue 
        /// </summary>
        [JsonIgnore]
        public string password { get; set; }
    }
}
