
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Prem.N3.PayerActivation.Processors.Entities
{
    public class PayerActivationCommand
    {
        [JsonProperty(PropertyName = "RequestID")]
        public string RequestID { get; set; }

        [JsonProperty(PropertyName = "PayerNumber")]
        public string PayerNumber { get; set; }

        [JsonProperty(PropertyName = "IsActive")]
        public bool IsActive { get; set; }

        public string Status { get; set; }
    }
}
