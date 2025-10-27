using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using ToyHouseXIV.ModSync.Glamourer.Automations;

namespace ToyHouseXIV.ModSync.Glamourer.Automations
{
    public class Automation
    {
        // ------------------------------------------------------------------------------------------------------------------------------------

        private readonly string[] _knownCharacterTypes = ["Player"];

        // ------------------------------------------------------------------------------------------------------------------------------------

        public readonly string? name = null;
        public readonly string? type = null;
        public readonly string? playerName = null;
        public readonly bool? enabled = null;
        public readonly IList<Design>? designs = null; 

        // ------------------------------------------------------------------------------------------------------------------------------------

        public readonly JsonNode? jsonNode = null;

        public Automation(JsonNode automation)
        {
            // ------------------------------------------------------------------------------------------------------------------------------------

            var name = (string?)automation["Name"];
            if (name == null)
            {
                throw new Exception("Automation field \"Name\" is missing");
            }
            this.name = name;

            var identifier = (JsonObject?)automation["Identifier"];
            if (identifier == null)
            {
                throw new Exception("Automation object field \"Identifier\" is missing");
            }

            var type = (string?)identifier["Type"];
            if (type == null || !_knownCharacterTypes.Contains(type))
            {
                throw new Exception("Automation field \"Type\" either missing or not in known list of types");
            }
            this.type = type;

            var playerName = (string?)identifier["PlayerName"];
            if (playerName == null)
            {
                throw new Exception("Automation field \"PlayerName\" is missing");
            }
            this.playerName = playerName;

            var enabled = (bool?)automation["Enabled"];
            if (enabled == null)
            {
                throw new Exception("Automation field \"Enabled\" is missing");
            }
            this.enabled = enabled;

            var designs = (JsonArray?)automation["Designs"];
            if (designs == null)
            {
                throw new Exception("Automation field \"Designs\" is missing");
            }

            this.designs = ParseDesigns(designs);

            // ------------------------------------------------------------------------------------------------------------------------------------

            this.jsonNode = automation;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------

        private List<Design> ParseDesigns(JsonArray designs)
        {
            var result = new List<Design>();

            foreach (var designNode in designs)
            {
                if (designNode == null)
                {
                    continue;
                }

                var design = new Design(designNode);
                result.Add(design);
            }

            return result;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------

        public List<Guid> GetDesigns()
        {
            var designs = this.designs!.Select(x => x.design!.Value).ToList();
            return designs!;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------
    }
}
