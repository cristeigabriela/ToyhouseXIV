using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using ToyHouseXIV.ModSync.Glamourer.Automations;
using ToyHouseXIV.ModSync.Penumbra.Collections;
using ToyHouseXIV.ModSync.Util;

namespace ToyHouseXIV.ModSync.Operations
{
    public struct ExporterSettings
    {
        public string penumbraCollection;
        public string glamourerAutomation;
    }

    public class Exporter
    {
        // ------------------------------------------------------------------------------------------------------------------------------------

        private readonly XIVMods _ctx;
        private readonly ExporterSettings _exporterSettings;

        // ------------------------------------------------------------------------------------------------------------------------------------

        public Exporter(XIVMods ctx, ExporterSettings exporterSettings)
        {
            _ctx = ctx;
            _exporterSettings = exporterSettings;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------

        private void ExportPenumbraModFiles(ZipArchive archive, Collection collection)
        {
            var mods = collection.GetAllModsPaths(_ctx!);

            foreach (var mod in mods)
            {
                var dirName = Path.GetFileName(mod);
                if (dirName == null || !Directory.Exists(mod))
                {
                    throw new Exception($"{dirName} is not a directory, full path: {mod}");
                }

                archive.CreateEntryFromDirectory(mod, $"{Constants.PenumbraMods}/{dirName}", CompressionLevel.SmallestSize);
            }
        }

        private void ExportPenumbraCollectionFile(ZipArchive archive, Collection collection)
        {
            var collectionId = collection.id;

            ZipArchiveEntry collectionEntry = archive.CreateEntry($"{Constants.PenumbraCollections}/{collectionId.ToString()}.json", CompressionLevel.SmallestSize);
            using (StreamWriter writer = new StreamWriter(collectionEntry.Open()))
            {
                var options = new JsonSerializerOptions
                {
                    // Don't escape single quotes, etc
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    // Avoid writing compactly, as Penumbra does not either
                    WriteIndented = true
                };

                writer.Write(collection.jsonNode!.ToJsonString(options));
            }
        }

        private void ExportPenumbra(ZipArchive archive)
        {
            var collection = _exporterSettings.penumbraCollection;
            var collections = _ctx.GetPenumbraCollections();
            var selectedCollection = collections[collection];

            ExportPenumbraModFiles(archive, selectedCollection);
            ExportPenumbraCollectionFile(archive, selectedCollection);
        }

        // ------------------------------------------------------------------------------------------------------------------------------------
        
        private void ExportGlamourerAutomationFile(ZipArchive archive, Automation automation)
        {
            ZipArchiveEntry automationEntry = archive.CreateEntry($"{Constants.GlamourerAutomations}/automation-data.json", CompressionLevel.SmallestSize);
            using (StreamWriter writer = new StreamWriter(automationEntry.Open()))
            {
                var options = new JsonSerializerOptions
                {
                    // Don't escape single quotes, etc
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    // Avoid writing compactly, as Glamourer does not either
                    WriteIndented = true
                };

                writer.Write(automation.jsonNode!.ToJsonString(options));
            }
        }

        private void ExportGlamourerDesignFiles(ZipArchive archive, Automation automation)
        {
            var designs = automation.GetDesigns();

            foreach (var design in designs)
            {
                var designPath = _ctx.GetGlamourerDesignPathFromId(design);
                if (designPath == null || !File.Exists(designPath))
                {
                    throw new Exception($"{designPath} is not a file");
                }

                var fileName = Path.GetFileName(designPath);
                if (fileName == null)
                {
                    throw new Exception($"Couldn't reduce {designPath} to filename");
                }

                archive.CreateEntryFromFile(designPath, $"{Constants.GlamourerDesigns}/{fileName}", CompressionLevel.SmallestSize);
            }
        }

        private void ExportGlamourer(ZipArchive archive)
        {
            var automation = _exporterSettings.glamourerAutomation;
            var automations = _ctx.GetGlamourerAutomations();
            var selectedAutomation = automations[automation];

            ExportGlamourerAutomationFile(archive, selectedAutomation);
            ExportGlamourerDesignFiles(archive, selectedAutomation);
        }

        // ------------------------------------------------------------------------------------------------------------------------------------

        public void Export(string path)
        {
            using (var stream = new FileStream($"{path}.toy", FileMode.Create))
            {
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
                {
                    ExportPenumbra(archive);
                    ExportGlamourer(archive);
                }
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------------------
    }
}
