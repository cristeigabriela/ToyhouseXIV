using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ToyHouseXIV.ModSync.Glamourer.Automations
{
    public class Design
    {
        // ------------------------------------------------------------------------------------------------------------------------------------

        public readonly Guid? design = null;
        public readonly int? type = null;
        public readonly int? jobGroup = null;

        // ------------------------------------------------------------------------------------------------------------------------------------

        public readonly JsonNode? jsonNode = null;

        public Design(JsonNode obj)
        {
            // ------------------------------------------------------------------------------------------------------------------------------------

            var design = (Guid?)obj["Design"];
            if (design == null)
            {
                throw new Exception("Automation Design field \"Design\" is either missing or invalid Guid");
            }
            this.design = design;

            var type = (int?)obj["Type"];
            if (type == null)
            {
                throw new Exception("Automation Design field \"Type\" is missing");
            }
            this.type = type;

            var conditions = (JsonObject?)obj["Conditions"];
            if (conditions == null)
            {
                throw new Exception("Automation Design field \"Conditions\" is missing");
            }

            var jobGroup = (int?)conditions["JobGroup"];
            if (jobGroup == null)
            {
                throw new Exception("Automation Design field \"JobGroup\" is missing");
            }
            this.jobGroup = jobGroup;

            // ------------------------------------------------------------------------------------------------------------------------------------

            jsonNode = obj;
        }
    }
}
