using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Sharp3DBinPacking.RandomTest
{
    static class Program
    {
        static Random RandomInstance = new Random();

        static void Main(string[] args)
        {
            var binPacker = BinPacker.GetDefault(BinPackerVerifyOption.All);
            var bestAlgorithmRecords = new Dictionary<string, int>();
            var round = 0;
            while (true)
            {
                var result = TestBinPacker(binPacker);
                if (bestAlgorithmRecords.ContainsKey(result.BestAlgorithmName))
                    bestAlgorithmRecords[result.BestAlgorithmName]++;
                else
                    bestAlgorithmRecords[result.BestAlgorithmName] = 1;
                round++;
                var binCount = result.BestResult.Count;
                var cuboidCount = result.BestResult.Sum(x => x.Count);
                Console.WriteLine($"Round {round} finished, {binCount} bins contains {cuboidCount} cuboids");
                Console.WriteLine($"Best algorithm records:");
                foreach (var pair in bestAlgorithmRecords.OrderByDescending(x => x.Value))
                {
                    Console.WriteLine($"{pair.Key}: {pair.Value}");
                }
                Console.WriteLine();
                Thread.Sleep(1);
            }
        }

        static BinPackResult TestBinPacker(IBinPacker binPacker)
        {
            var binWidth = RandomInstance.Next(100, 5001);
            var binHeight = RandomInstance.Next(100, 5001);
            var binDepth = RandomInstance.Next(100, 5001);
            var binWeight = RandomInstance.Next(100, 5001);
            var cuboidsCount = RandomInstance.Next(50, 501);
            var allowRotateVertically = RandomInstance.Next(0, 2) == 0;
            var cuboids = new List<Cuboid>();
            for (var x = 0; x < cuboidsCount; ++x)
            {
                var width = RandomInstance.Next(1, binWidth + 1);
                var height = RandomInstance.Next(1, binHeight + 1);
                var depth = RandomInstance.Next(1, binDepth + 1);
                var weight = RandomInstance.Next(1, binWeight / 20 + 1);
                cuboids.Add(new Cuboid(width, height, depth, weight, null));
            }
            var parameter = new BinPackParameter(
                binWidth, binHeight, binDepth, binWeight, allowRotateVertically, cuboids);
            return binPacker.Pack(parameter);
        }
    }
}
