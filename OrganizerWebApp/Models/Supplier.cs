using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OrganizerWebApp.Models
{
    public class Supplier
    {
        // This will really be a GUID, stringified
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        // URL to the Products API
        [JsonProperty(PropertyName = "apiBaseUrl")]
        public string ApiBaseUrl { get; set; }

        [JsonProperty(PropertyName = "apiSubscriptionKey")]
        public string ApiSubscriptionKey { get; set; }

        [JsonProperty(PropertyName = "registrationTime")]
        public string RegistrationTime { get; set; }
    }
}
