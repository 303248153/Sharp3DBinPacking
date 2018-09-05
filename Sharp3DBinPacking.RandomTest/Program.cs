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
            var averageVolumeRate = 0m;
            var round = 0;
            while (true)
            {
                var tuple = TestBinPacker(binPacker);
                var result = tuple.Item1;
                var volumeRate = tuple.Item2;
                averageVolumeRate = (averageVolumeRate * round + volumeRate) / (round + 1);
                round++;
                var binCount = result.BestResult.Count;
                var cuboidCount = result.BestResult.Sum(x => x.Count);
                Console.WriteLine(
                    $"Round {round} finished, {binCount} bins contains {cuboidCount} cuboids, " +
                    $"average volume rate {averageVolumeRate.ToString("0.0000")}");
                Thread.Sleep(1);
            }
        }

        static Tuple<BinPackResult, decimal> TestBinPacker(IBinPacker binPacker)
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
            var result = binPacker.Pack(parameter);
            var volumeRate = BinPacker.GetVolumeRate(parameter, result.BestResult);
            return Tuple.Create(result, volumeRate);
        }
    }
}
