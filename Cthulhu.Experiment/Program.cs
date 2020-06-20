using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Cthulhu.Serialization;

namespace Cthulhu.Experiment
{
    class Program
    {
        static void DumpSize<T>() where T : struct
        {
            // unsafe
            {
                var size = Unsafe.SizeOf<T>();
                var nullabeSize = Unsafe.SizeOf<Nullable<T>>();
                Console.WriteLine($"{typeof(T)} - {size} - {nullabeSize}");
            }
        }
        
        static async Task Main(string[] args)
        {
            DumpSize<short>();
            DumpSize<int>();
            DumpSize<Color24>();
            DumpSize<double>();

            var stopwatch = Stopwatch.StartNew();
            var xml = await File.ReadAllTextAsync("../tiles.xml");
            Console.WriteLine("Read XML in " + stopwatch.Elapsed);
            stopwatch.Restart();
            var worldInfo = WorldInfo.FromWorldInfoData(xml);
            Console.WriteLine("Parsed tile data in " + stopwatch.Elapsed);
        }
    }
}
