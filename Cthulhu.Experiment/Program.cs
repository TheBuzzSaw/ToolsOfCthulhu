using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Cthulhu.Serialization;

namespace Cthulhu.Experiment
{
    class Program
    {
        static async Task Main(string[] args)
        {
            foreach (var arg in args)
            {
                var stopwatch = Stopwatch.StartNew();
                var bytes = await File.ReadAllBytesAsync(arg);
                Console.WriteLine($"Loaded file ({bytes.Length} bytes) in {stopwatch.Elapsed}");
                stopwatch.Restart();
                var world = World.FromWorldData(bytes);
                Console.WriteLine("Parsed world data in " + stopwatch.Elapsed);

                // var options = new JsonSerializerOptions
                // {
                //     WriteIndented = true
                // };

                // var json = JsonSerializer.Serialize(world, options);
                // Console.WriteLine(json);

                Console.Title = "Terraria Item Search: " + world.Name;
                
                while (true)
                {
                    Console.WriteLine("Enter an item ID:");
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
                    
                    Console.WriteLine("Search completed in " + elapsed);
                }                
            }
        }
    }
}
