using System.Runtime.InteropServices;
using ToyHouseXIV.ModSync;
using ToyHouseXIV.ModSync.Operations;

namespace ToyHouseXIV
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var mods = new XIVMods();

                //var exporterSettings = new ExporterSettings
                //{
                //    penumbraCollection = "Bhoomi",
                //    glamourerAutomation = "Bhoomi Itzcan"
                //};
                //var exporter = new Exporter(mods, exporterSettings);
                //exporter.Export(@"C:\toyhousexiv\bhoomi");

                var importerSettings = new ImporterSettings
                {
                    character =
                    {
                        playerName = "Bhoomi Itzcan"
                    }
                };
                var importer = new Importer(mods, importerSettings);
                importer.Import(@"C:\toyhousexiv\bhoomi.toy");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ToyHouseXIV failed during execution: {ex}");
                Console.WriteLine($"Last error: {Marshal.GetLastSystemError()}");
                System.Environment.Exit(1);
            }
        }
    }
}
