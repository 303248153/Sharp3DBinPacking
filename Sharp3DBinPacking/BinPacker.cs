using Sharp3DBinPacking.Algorithms;
using Sharp3DBinPacking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharp3DBinPacking
{
    public delegate IBinPackAlgorithm BinPackAlgorithmFactory(BinPackParameter parameter);

    public class BinPacker : IBinPacker
    {
        private readonly BinPackAlgorithmFactory[] _factories;
        private readonly BinPackerVerifyOption _verifyOption;

        public BinPacker(BinPackerVerifyOption verifyOption, params BinPackAlgorithmFactory[] factories)
        {
            _verifyOption = verifyOption;
            _factories = factories;
        }

        public BinPackResult Pack(BinPackParameter parameter)
        {
            // [ [ cuboid in bin a, cuboid in bin a, ... ], [ cuboid in bin b, ... ] ]
            IList<IList<Cuboid>> bestResult = null;
            string bestAlgorithmName = null;
            foreach (var factory in _factories)
            {
                foreach (var cuboids in GetCuboidsPermutations(parameter.Cuboids))
                {
                    // reset cuboids state
                    var unpackedCuboids = cuboids.ToList();
                    foreach (var cuboid in unpackedCuboids)
                        cuboid.ResetPlacedInformation();
                    var result = new List<IList<Cuboid>>();
                    var algorithmName = "";
                    while (unpackedCuboids.Count > 0)
                    {
                        // pack single bin
                        var algorithm = factory(parameter);
                        algorithmName = algorithm.ToString();
                        algorithm.Insert(unpackedCuboids);
                        // find out which cuboids are placed
                        var packedCuboids = unpackedCuboids.Where(c => c.IsPlaced).ToList();
                        if (packedCuboids.Count == 0)
                            break;
                        result.Add(packedCuboids);
                        // pack remain cuboids
                        unpackedCuboids = unpackedCuboids.Where(c => !c.IsPlaced).ToList();
                    }
                    // verify this result
                    if (_verifyOption == BinPackerVerifyOption.All)
                        Verify(parameter, algorithmName, result);
                    // update best result if all cuboids is placed and uses less bins
                    if (unpackedCuboids.Count == 0 &&
                        (bestResult == null || result.Count < bestResult.Count))
                    {
                        bestResult = result;
                        bestAlgorithmName = algorithmName;
                    }
                }
            }
            if (bestResult == null)
            {
                throw new InvalidOperationException(
                    "no algorithm can pack these cuboids\n" +
                    $"binWidth: {parameter.BinWidth}, binHeight: {parameter.BinHeight}, " +
                    $"binDepth: {parameter.BinDepth}, binWeight: {parameter.BinWeight}\n" +
                    $"cuboids: {string.Join("\n", parameter.Cuboids.Select(x => x.ToString()))}");
            }
            // verify the best result
            if (_verifyOption == BinPackerVerifyOption.BestOnly)
                Verify(parameter, bestAlgorithmName, bestResult);
            return new BinPackResult(bestResult, bestAlgorithmName);
        }

        private void Verify(BinPackParameter parameter, string algorithmName, IList<IList<Cuboid>> result)
        {
            //       o--------o
            //      /|       /|
            //     / |      / |
            //  h o--------o  |
            //  e |  o-----|--o h
            //y i | /      | / t
            //  g |/       |/ p z
            //  h o--------o e
            //  t | width   d
            //    |  x
            // (0, 0, 0)
            foreach (var cuboids in result)
            {
                for (int a = 0; a < cuboids.Count; ++a)
                {
                    // check if cuboid out of bin
                    var cuboid = cuboids[a];
                    if (cuboid.X < 0 || cuboid.Y < 0 || cuboid.Z < 0)
                    {
                        throw new ArithmeticException(
                            $"verify cuboid failed: negative position, algorithm: {algorithmName}, cuboid: {cuboid}");
                    }
                    if (cuboid.X + cuboid.Width > parameter.BinWidth ||
                        cuboid.Y + cuboid.Height > parameter.BinHeight ||
                        cuboid.Z + cuboid.Depth > parameter.BinDepth)
                    {
                        throw new ArithmeticException(
                            $"verify cuboid failed: out of bin, algorithm: {algorithmName}, cuboid: {cuboid}");
                    }
                    // check if this cuboid intersects others
                    for (int b = a + 1; b < cuboids.Count; ++b)
                    {
                        var otherCuboid = cuboids[b];
                        if (cuboid.X < otherCuboid.X + otherCuboid.Width &&
                            otherCuboid.X < cuboid.X + cuboid.Width &&
                            cuboid.Y < otherCuboid.Y + otherCuboid.Height &&
                            otherCuboid.Y < cuboid.Y + cuboid.Height &&
                            cuboid.Z < otherCuboid.Z + otherCuboid.Depth &&
                            otherCuboid.Z < cuboid.Z + cuboid.Depth)
                        {
                            throw new ArithmeticException(
                                $"verify cuboid failed: cuboid intersects others, algorithm: {algorithmName}, cuboid a: {cuboid}, cuboid b: {otherCuboid}");
                        }
                    }
                }
                // check is cuboids overweight
                if (cuboids.Sum(c => c.Weight) > parameter.BinWeight)
                {
                    throw new ArithmeticException(
                        $"verify cuboid failed: cuboids overweight, algorithm: {algorithmName}");
                }
            }
        }

        public IEnumerable<IEnumerable<Cuboid>> GetCuboidsPermutations(IEnumerable<Cuboid> cuboids)
        {
            yield return cuboids;
            yield return cuboids.OrderByDescending(x => x.Width);
            yield return cuboids.OrderByDescending(x => x.Height);
            yield return cuboids.OrderByDescending(x => x.Depth);
            yield return cuboids.OrderByDescending(x => x.Width * x.Height * x.Depth);
        }

        public static IBinPacker GetDefault(BinPackerVerifyOption verifyOption)
        {
            return new BinPacker(verifyOption,
                parameter => new BinPackShelfAlgorithm(
                    parameter,
                    FreeRectChoiceHeuristic.RectBestAreaFit,
                    GuillotineSplitHeuristic.SplitLongerLeftoverAxis,
                    ShelfChoiceHeuristic.ShelfFirstFit),
                parameter => new BinPackShelfAlgorithm(
                    parameter,
                    FreeRectChoiceHeuristic.RectBestAreaFit,
                    GuillotineSplitHeuristic.SplitLongerLeftoverAxis,
                    ShelfChoiceHeuristic.ShelfNextFit),
                parameter => new BinPackGuillotineAlgorithm(
                    parameter,
                    FreeCuboidChoiceHeuristic.CuboidMinHeight,
                    GuillotineSplitHeuristic.SplitLongerLeftoverAxis),
                parameter => new BinPackGuillotineAlgorithm(
                    parameter,
                    FreeCuboidChoiceHeuristic.CuboidMinHeight,
                    GuillotineSplitHeuristic.SplitShorterLeftoverAxis));
        }
    }
}
