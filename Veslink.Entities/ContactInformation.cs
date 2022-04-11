using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veslink.Entities
{
    public class ContactInformation
    {
        public string OpsCoordinator { get; set; }

        [JsonProperty("User.FullName")]
        public string UserFullName { get; set; }

        [JsonProperty("User.Email")]
        public string UserEmail { get; set; }
    }
}
