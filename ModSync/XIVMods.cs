using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using ToyHouseXIV.ModSync.Penumbra.Individuals;
using ToyHouseXIV.ModSync.Penumbra.Collections;
using ToyHouseXIV.ModSync.Glamourer.Automations;

namespace ToyHouseXIV.ModSync
{
    public class XIVMods
    {
        // ------------------------------------------------------------------------------------------------------------------------------------

        public readonly string? penumbraModsFolder = null;

        // ------------------------------------------------------------------------------------------------------------------------------------

        public readonly string? xivLauncherFolder = null;
        public readonly string? pluginConfigsFolder = null;

        // ------------------------------------------------------------------------------------------------------------------------------------

        public readonly string? penumbraFolder = null;
        public readonly string? glamourerFolder = null;
        
        // ------------------------------------------------------------------------------------------------------------------------------------

        public readonly string? penumbraCollectionsFolder = null;
        public readonly string? penumbraActiveCollectionsFile = null;

        // ------------------------------------------------------------------------------------------------------------------------------------

        public readonly string? glamourerDesignsFolder = null;
        public readonly string? glamourerAutomationFile = null;

        // ------------------------------------------------------------------------------------------------------------------------------------

        public XIVMods()
        {
            // ------------------------------------------------------------------------------------------------------------------------------------
            // Start resolving the regular paths for directories and files for your mods...

            var xivModsFolder = @"%appdata%\XIVLauncher";
            xivLauncherFolder = Path.GetFullPath(Environment.ExpandEnvironmentVariables(xivModsFolder));

            if (!Path.Exists(xivLauncherFolder))
            {
                throw new DirectoryNotFoundException($"The XIVLauncher folder is missing, attempt at resolving it: {xivLauncherFolder}");
            }

            pluginConfigsFolder = Path.Join(xivLauncherFolder, "pluginConfigs");
            if (!Path.Exists(pluginConfigsFolder))
            {
                throw new DirectoryNotFoundException($"The pluginConfigs folder is missing, attempt at resolving it: {pluginConfigsFolder}");
            }

            var penumbraConfigFile = Path.Join(pluginConfigsFolder, "Penumbra.json");
            if (!File.Exists(penumbraConfigFile))
            {
                throw new FileNotFoundException($"The Penumbra.json file is missing, attempt at resolving it: {penumbraConfigFile}");
            }

            penumbraModsFolder = ParsePenumbraModDirectory(penumbraConfigFile);
            if (penumbraModsFolder == null)
            {
                throw new Exception("Could not parse ModDirectory from Penumbra configuration, please configure Penumbra");
            }

            // ------------------------------------------------------------------------------------------------------------------------------------

            penumbraFolder = Path.Join(pluginConfigsFolder, "Penumbra");
            if (!Path.Exists(penumbraFolder))
            {
                throw new DirectoryNotFoundException($"The pluginConfigs/Penumbra folder is missing, attempt at resolving it: {penumbraFolder}");
            }

            glamourerFolder = Path.Join(pluginConfigsFolder, "Glamourer");
            if (!Path.Exists(glamourerFolder))
            {
                throw new DirectoryNotFoundException($"The pluginConfigs/Glamourer folder is missing, attempt at resolving it: {glamourerFolder}");
            }

            // ------------------------------------------------------------------------------------------------------------------------------------

            penumbraCollectionsFolder = Path.Join(penumbraFolder, "collections");
            if (!Path.Exists(penumbraCollectionsFolder))
            {
                throw new DirectoryNotFoundException($"The Penumbra/collections folder is missing, attempt at resolving it: {penumbraCollectionsFolder}");
            }

            penumbraActiveCollectionsFile = Path.Join(penumbraFolder, "active_collections.json");
            if (!File.Exists(penumbraActiveCollectionsFile))
            {
                throw new FileNotFoundException($"The Penumbra/active_collections.json file is missing, attempt at resolving it: {penumbraActiveCollectionsFile}");
            }

            // ------------------------------------------------------------------------------------------------------------------------------------

            glamourerDesignsFolder = Path.Join(glamourerFolder, "designs");
            if (!Path.Exists(penumbraActiveCollectionsFile))
            {
                throw new DirectoryNotFoundException($"The Glamourer/designs folder is missing, attempt at resolving it: {glamourerDesignsFolder}");
            }

            glamourerAutomationFile = Path.Join(glamourerFolder, "automation.json");
            if (!File.Exists(glamourerAutomationFile))
            {
                throw new FileNotFoundException($"The Glamourer/automation.json file is missing, attempt at resolving it: {glamourerDesignsFolder}");
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------------------

        private string GetPenumbraConfigFile(string path)
        {
            return File.ReadAllText(path);
        }

        private JsonNode? GetPenumbraConfigJson(string path)
        {
            return JsonNode.Parse(GetPenumbraConfigFile(path));
        }

        private string? ParsePenumbraModDirectory(string path)
        {
            var config = GetPenumbraConfigJson(path);
            if (config == null)
            {
                return null;
            }

            var modDirectory = (string?)config["ModDirectory"];
            if (modDirectory == null)
            {
                return null;
            }
            return modDirectory;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------

        private string? GetPenumbraActiveCollectionsFile()
        {
            if (penumbraActiveCollectionsFile == null)
            {
                return null;
            }

            return File.ReadAllText(penumbraActiveCollectionsFile);
        }

        private JsonNode? GetPenumbraActiveCollectionsJson()
        {
            var file = GetPenumbraActiveCollectionsFile();
            if (file == null)
            {
                return null;
            }

            return JsonNode.Parse(file);
        }

        // ------------------------------------------------------------------------------------------------------------------------------------

        public Dictionary<string, Collection> GetPenumbraCollections()
        {
            var collections = new Dictionary<string, Collection>();

            if (penumbraCollectionsFolder == null)
            {
                return collections;
            }

            var collectionFiles = Directory.GetFiles(penumbraCollectionsFolder);
            if (collectionFiles == null)
            {
                return collections;
            }

            foreach (var collectionFile in collectionFiles)
            {
                var id = Guid.NewGuid();
                try
                {
                    id = Guid.Parse(Path.GetFileNameWithoutExtension(collectionFile))!;
                    var collectionJson = GetPenumbraCollectionJsonFromId(id);
                    if (collectionJson == null)
                    {
                        throw new FileNotFoundException($"Failed finding collection {id} in Penumbra collections folder");
                    }
                    var collection = new Collection(collectionJson);
                    collections.Add(collection.name!, collection);
                } catch (Exception ex)
                {
                    Console.WriteLine($"WARNING: Failed parsing collection {id}: {ex}");
                    continue;
                }
            }

            return collections;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------

        public string? GetPenumbraCollectionPathFromId(Guid guid)
        {
            if (penumbraCollectionsFolder == null)
            {
                return null;
            }

            var collectionFile = Path.Join(penumbraCollectionsFolder, $"{guid.ToString()}.json");
            if (!Path.Exists(collectionFile))
            {
                return null;
            }

            return collectionFile;
        }

        private string? GetPenumbraCollectionFileFromId(Guid guid)
        {
            var collectionFile = GetPenumbraCollectionPathFromId(guid);
            if (collectionFile == null)
            {
                return null;
            }

            return File.ReadAllText(collectionFile);
        }

        public JsonNode? GetPenumbraCollectionJsonFromId(Guid guid)
        {
            var file = GetPenumbraCollectionFileFromId(guid);
            if (file == null)
            {
                return null;
            }

            return JsonNode.Parse(file);
        }

        // ------------------------------------------------------------------------------------------------------------------------------------

        private string? GetGlamourerAutomationFile()
        {
            if (glamourerAutomationFile  == null)
            {
                return null;
            }

            return File.ReadAllText(glamourerAutomationFile);
        }

        private JsonNode? GetGlamourerAutomationJson()
        {
            var file = GetGlamourerAutomationFile();
            if (file == null)
            {
                return null;
            }

            return JsonNode.Parse(file);
        }

        // ------------------------------------------------------------------------------------------------------------------------------------

        public string? GetGlamourerDesignPathFromId(Guid guid)
        {
            if (glamourerDesignsFolder == null)
            {
                return null;
            }

            var designFile = Path.Join(glamourerDesignsFolder, $"{guid.ToString()}.json");
            if (!Path.Exists(designFile))
            {
                return null;
            }

            return designFile;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------

        public Dictionary<string, Automation> GetGlamourerAutomations()
        {
            var automations = new Dictionary<string, Automation>();

            var automationJson = GetGlamourerAutomationJson();
            if (automationJson == null)
            {
                return automations;
            }

            var data = (JsonArray?)automationJson["Data"];
            if (data == null)
            {
                return automations;
            }

            foreach (var entry in data)
            {
                var automation = new Automation(entry!);
                automations.Add(automation.name!, automation);
            }

            return automations;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------
        private List<Character> ParseCharacters(JsonNode activeCollections)
        {
            var characters = new List<Character>();

            var individuals = (JsonArray?)activeCollections["Individuals"];
            if (individuals == null)
            {
                return characters;
            }

            foreach (var individual in individuals)
            {
                try
                {
                    var character = new Character(individual!);
                    characters.Add(character);
                } catch (Exception ex)
                {
                    Console.WriteLine($"WARNING: Failed parsing character: \"{ex}\", node: \"{individual}\"");
                    continue;
                }
            }

            return characters;
        }

        public List<Character> GetCharacters()
        {
            var characters = new List<Character>();

            var activeCollectionsJson = GetPenumbraActiveCollectionsJson();
            if (activeCollectionsJson == null)
            {
                return characters;
            }

            characters = ParseCharacters(activeCollectionsJson);
            return characters;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------
    }
}
