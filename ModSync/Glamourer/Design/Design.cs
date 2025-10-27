using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ToyHouseXIV.ModSync.Glamourer.Design
{
    public class Design
    {
        // ------------------------------------------------------------------------------------------------------------------------------------

        public readonly Guid? id = null;
        public readonly string? name = null;

        // ------------------------------------------------------------------------------------------------------------------------------------

        public readonly JsonNode? jsonNode = null;

        public Design(JsonNode obj)
        {
            // ------------------------------------------------------------------------------------------------------------------------------------

            var id = (Guid?)obj["Identifier"];
            if (id == null)
            {
                throw new Exception("Design field \"Identifier\" is either missing or invalid Guid");
            }
            this.id = id;

            var name = (string?)obj["Name"];
            if (name == null)
            {
                throw new Exception("Design field \"Name\" is missing");
            }
            this.name = name;

            // ------------------------------------------------------------------------------------------------------------------------------------

            this.jsonNode = obj;
        }
    }
}
