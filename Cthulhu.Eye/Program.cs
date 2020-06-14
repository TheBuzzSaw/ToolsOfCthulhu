using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Cthulhu.Serialization;

namespace Cthulhu.Eye
{
    class Program
    {
        private static readonly string theDivider = string.Join(' ', Enumerable.Repeat("---", 4));
        private static readonly ImmutableArray<Option<Action<World>>> theOpenWorldOptions = ImmutableArray.Create<Option<Action<World>>>(
            CreateWao("Dump world meta data as JSON", DumpWorldMetaDataAsJson),
            CreateWao("Search Chests by Item ID", SearchChestsByItemId)
        );

        static Option<Action<World>> CreateWao(string displayText, Action<World> action) => Option.Create(displayText, action);

        static void DumpWorldMetaDataAsJson(World world)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            
            var json = JsonSerializer.Serialize(world, options);
            Console.WriteLine(json);
        }

        static void SearchChestsByItemId(World world)
        {
            var stopwatch = new Stopwatch();

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine($"Search all chests in {world.Name} by item ID");
                Console.WriteLine("Enter item ID:");

                var line = Console.ReadLine();

                if (!int.TryParse(line, out var itemId))
                    break;
                
                stopwatch.Restart();
                var chests = world.Chests
                    .Where(c => c.Items.Any(item => item.Id == itemId))
                    .ToList();
                var elapsed = stopwatch.Elapsed;
                
                foreach (var chest in chests)
                    Console.WriteLine(chest);
                
                var word = chests.Count == 1 ? "chest" : "chests";
                Console.WriteLine($"Found {chests.Count} {word} in {elapsed}");
            }
        }
        
        static void OpenWorld(string worldFile)
        {
            var stopwatch = Stopwatch.StartNew();
            var bytes = File.ReadAllBytes(worldFile);
            Console.WriteLine($"Loaded file into memory ({bytes.Length} bytes) in {stopwatch.Elapsed}");
            stopwatch.Restart();
            var world = World.FromWorldData(bytes);
            Console.WriteLine($"Parsed world data (version {world.Version}) in {stopwatch.Elapsed}");

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine(worldFile);

                for (int i = 0; i < theOpenWorldOptions.Length; ++i)
                {
                    var displayText = theOpenWorldOptions[i].DisplayText;
                    Console.WriteLine($"[{i}] {displayText}");
                }

                var line = Console.ReadLine();

                if (!int.TryParse(line, out var index) || index < 0 || theOpenWorldOptions.Length <= index)
                    break;
                
                theOpenWorldOptions[index].Value.Invoke(world);
            }
        }

        static void SelectWorld()
        {
            var worldFolder = World.GetFolder();

            while (true)
            {
                var worldFiles = Directory.GetFiles(worldFolder, "*.wld");
                if (worldFiles is null || worldFiles.Length == 0)
                {
                    Console.WriteLine("No worlds found in " + worldFolder);
                    return;
                }

                var worldOptions = Array.ConvertAll(worldFiles, wf => Option.Create(Path.GetFileName(wf), wf));
                Array.Sort(worldOptions, (a, b) => a.DisplayText.CompareTo(b.DisplayText));
                
                Console.WriteLine("Worlds found in " + worldFolder);

                for (int i = 0; i < worldOptions.Length; ++i)
                {
                    var fileName = worldOptions[i].DisplayText;
                    Console.WriteLine($"[{i}] {fileName}");
                }

                Console.WriteLine($"Enter world index (0 - {worldOptions.Length - 1}):");

                var line = Console.ReadLine();

                if (!int.TryParse(line, out var index) || index < 0 || worldOptions.Length <= index)
                    break;
                
                try
                {
                    OpenWorld(worldOptions[index].Value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(theDivider);
                    Console.WriteLine(ex);
                    Console.WriteLine(theDivider);
                }
            }
        }

        static int Main(string[] args)
        {
            try
            {
                var version = typeof(Program).Assembly.GetName().Version;
                Console.Title = "Eye of Cthulhu version " + version;
                SelectWorld();
                Console.WriteLine("Farewell");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }
    }
}
