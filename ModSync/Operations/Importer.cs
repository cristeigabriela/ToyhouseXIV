using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using ToyHouseXIV.ModSync;
using ToyHouseXIV.ModSync.Operations;

namespace ToyHouseXIV.ModSync.Operations
{
    public struct ImporterSettings
    {
        public struct Character
        {
            public string playerName;
        }

        public Character character;
    }

    public class Importer
    {
        // ------------------------------------------------------------------------------------------------------------------------------------

        private readonly XIVMods _ctx;
        private readonly ImporterSettings _importerSettings;

        // ------------------------------------------------------------------------------------------------------------------------------------

        public Importer(XIVMods ctx, ImporterSettings importerSettings)
        {
            _ctx = ctx;
            _importerSettings = importerSettings;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------

        private void RegisterCharacterToCollection(string collection)
        {

        }

        private static string GetRootFolder(string path)
        {
            while (true)
            {
                string? temp = Path.GetDirectoryName(path);
                if (String.IsNullOrEmpty(temp))
                    break;

                path = temp;
            }
            return path;
        }

        private void ImportPenumbraModFiles(ZipArchive archive)
        {
            var currentlyExtracting = new List<string>();
            foreach (var entry in archive.Entries)
            {
                // Check if entry is a Penumbra mod
                if (entry == null || !entry.FullName.StartsWith($"{Constants.PenumbraMods}"))
                {
                    continue;
                }

                // Remove the prefix from the path
                var modRelative = entry.FullName.Substring(Constants.PenumbraMods.Length + 1);

                // Get the actual mod name
                var modName = GetRootFolder(modRelative);

                // Get the filesystem mod path
                var modFsPath = Path.Join(_ctx.penumbraModsFolder!, modName);

                // If the mod already exists, and it's root path isn't registered as
                // a mod that's *currently* being extracted, then skip altogether.
                // We don't want to replace existing mods!
                if (Path.Exists(modFsPath) && !currentlyExtracting.Contains(modFsPath))
                {
                    Console.WriteLine($"WARNING: {modName} is already present in Penumbra mods folder, skipping!");
                    continue;
                }

                // For next passes to not be skipped, register ourselves as a mod that's being currently
                // extracted, bypassing the above if check.
                currentlyExtracting.Add(modFsPath);

                // Get the path on the filesystem for the mod
                var modFileFsPath = Path.Join(_ctx.penumbraModsFolder!, modRelative);

                // Exceptions would be thrown if the folder that contains the file
                // is not already present on file system before extraction.
                var modFolderFsPath = Path.GetDirectoryName(modFileFsPath);
                if (!Directory.Exists(modFolderFsPath))
                {
                    Directory.CreateDirectory(modFolderFsPath!);
                }

                // Finally, we can extract the mod to the filesystem.
                entry.ExtractToFile(modFileFsPath);

                // Now that we have the collection file, register the character
                // to it in active collections.
            }
        }

        private void ImportPenumbraCollectionFiles(ZipArchive archive)
        {
            foreach (var entry in archive.Entries)
            {
                // Check if entry is a Penumbra collection
                if (entry == null || !entry.FullName.StartsWith($"{Constants.PenumbraCollections}"))
                {
                    continue;
                }

                // Remove the prefix from the path
                var collection = entry.FullName.Substring(Constants.PenumbraCollections.Length + 1);

                // Make sure we don't override existing collections
                var collectionFs = Path.Join(_ctx.penumbraCollectionsFolder!, collection);
                if (File.Exists(collectionFs))
                {
                    Console.WriteLine($"WARNING: Collection {collection} already exists, skipping!");
                    continue;
                }

                entry.ExtractToFile(collectionFs);
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------------------

        public void Import(string path)
        {
            using (var stream = new FileStream($"{path}", FileMode.Open))
            {
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    ImportPenumbraModFiles(archive);
                    ImportPenumbraCollectionFiles(archive);
                }
            }
        }
    }
}
