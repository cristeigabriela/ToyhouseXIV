using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using ToyHouseXIV.ModSync;
using ToyHouseXIV.ModSync.Penumbra.Collections;

namespace ToyHouseXIV.ModSync.Penumbra.Individuals
{
    /// <summary>
    /// Responsible for trying to parse a `JsonNode` for details that will be necessary later.
    /// </summary>
    public class Character
    {
        // ------------------------------------------------------------------------------------------------------------------------------------

        private readonly string[] _knownCharacterTypes = ["Player"];

        // ------------------------------------------------------------------------------------------------------------------------------------

        public readonly string? type = null;
        public readonly string? playerName = null;
        // Actively selected collection
        public readonly Guid? collection = null;

        // ------------------------------------------------------------------------------------------------------------------------------------

        public readonly JsonNode? jsonNode = null;

        public Character(JsonNode individual)
        {
            // ------------------------------------------------------------------------------------------------------------------------------------

            var type = (string?)individual["Type"];
            if (type == null || !_knownCharacterTypes.Contains(type))
            {
                throw new Exception("Character field \"Type\" either missing or not in known list of types");
            }
            this.type = type;

            var playerName = (string?)individual["PlayerName"];
            if (playerName == null)
            {
                throw new Exception("Character field \"PlayerName\" is missing");
            }
            this.playerName = playerName;

            var collection = (Guid?)individual["Collection"];
            if (collection == null)
            {
                throw new Exception("Character field \"Collection\" is either missing or invalid Guid");
            }
            this.collection = collection;

            // ------------------------------------------------------------------------------------------------------------------------------------

            this.jsonNode = individual;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------

        public Collection? GetCollection(XIVMods ctx)
        {
            if (this.collection == null)
            {
                return null;
            }

            var collectionJson = ctx.GetPenumbraCollectionJsonFromId(this.collection.Value);
            if (collectionJson == null)
            {
                return null;
            }

            var collection = new Collection(collectionJson);
            return collection;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------
    }
}
