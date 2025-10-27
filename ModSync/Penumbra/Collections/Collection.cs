using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using ToyHouseXIV.ModSync;

namespace ToyHouseXIV.ModSync.Penumbra.Collections
{
    /// <summary>
    /// Responsible for trying to parse a `JsonNode` for details that will be necessary later.
    /// </summary>
    public class Collection
    {
        // ------------------------------------------------------------------------------------------------------------------------------------

        public readonly Guid? id = null;
        public readonly string? name = null;
        public readonly IDictionary<string, JsonNode>? settings = null;

        // ------------------------------------------------------------------------------------------------------------------------------------

        public readonly JsonNode? jsonNode = null;

        public Collection(JsonNode collection)
        {
            // ------------------------------------------------------------------------------------------------------------------------------------

            var id = (Guid?)collection["Id"];
            if (id == null)
            {
                throw new Exception("Collection field \"Id\" is either missing or invalid Guid");
            }
            this.id = id;

            var name = (string?)collection["Name"];
            if (name == null)
            {
                throw new Exception("Collection field \"Name\" is missing");
            }
            this.name = name;

            var settingsDict = new Dictionary<string, JsonNode>();

            var settings = (JsonObject?)collection["Settings"];
            if (settings == null)
            {
                throw new Exception("Collection field \"Settings\" is missing");
            }

            foreach (var setting in settings)
            {
                if (setting.Value == null)
                {
                    continue;
                }

                var enabled = (bool?)setting.Value["Enabled"];
                if (enabled == null || enabled == false)
                {
                    continue;
                }

                settingsDict.Add(setting.Key, setting.Value);
            }
            this.settings = settingsDict!;

            // ------------------------------------------------------------------------------------------------------------------------------------

            this.jsonNode = collection;
        }


        // ------------------------------------------------------------------------------------------------------------------------------------

        public ICollection<string>? GetAllMods()
        {
            if (this.settings == null)
            {
                return null;
            }

            return this.settings.Keys;
        }

        public List<string> GetAllModsPaths(XIVMods ctx)
        {
            var paths = new List<string>();

            var mods = GetAllMods();
            if (mods == null)
            {
                return paths;
            }

            foreach (var mod in mods)
            {
                var modPath = Path.Join(ctx.penumbraModsFolder, mod);
                if (!Path.Exists(modPath))
                {
                    Console.WriteLine($"WARNING: Mod \"{mod}\" in collection \"{this.name}\" is missing in Penumbra Mods folder.");
                    continue;
                }

                paths.Add(modPath);
            }

            return paths;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------
    }
}
