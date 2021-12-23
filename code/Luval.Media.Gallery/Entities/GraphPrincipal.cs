using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.Entities
{
    public class GraphPrincipal
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("surname")]
        public string Surname { get; set; }
        [JsonProperty("givenName")]
        public string GivenName { get; set; }
        [JsonProperty("userPrincipalName")]
        public string UserPrincipalName { get; set; }
        [JsonProperty("mail")]
        public string Mail { get; set; }
        [JsonProperty("preferredLanguage")]
        public string PreferredLanguage { get; set; }
    }
}
