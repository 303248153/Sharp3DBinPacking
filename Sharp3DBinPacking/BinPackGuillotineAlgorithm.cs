using Sharp3DBinPacking.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp3DBinPacking
{
    public class BinPackGuillotineAlgorithm : IBinPackAlgorithm
    {
        private readonly decimal _binWidth;
        private readonly decimal _binHeight;
        private readonly decimal _binDepth;
        private readonly FreeCuboidChoiceHeuristic _cuboidChoice;
        private readonly GuillotineSplitHeuristic _splitMethod;
        private readonly IList<Cuboid> _usedCuboids;
        private readonly IList<Cuboid> _freeCuboids;

        public BinPackGuillotineAlgorithm(
            decimal binWidth,
            decimal binHeight,
            decimal binDepth,
            FreeCuboidChoiceHeuristic cuboidChoice,
            GuillotineSplitHeuristic splitMethod)
        {
            _binWidth = binWidth;
            _binHeight = binHeight;
            _binDepth = binDepth;
            _cuboidChoice = cuboidChoice;
            _splitMethod = splitMethod;
            _usedCuboids = new List<Cuboid>();
            _freeCuboids = new List<Cuboid>();
            AddFreeCuboid(new Cuboid(_binWidth, _binHeight, _binDepth));
        }

        public void Insert(IEnumerable<Cuboid> cuboids)
        {
            foreach (var cuboid in cuboids)
            {
                Insert(cuboid, _cuboidChoice, _splitMethod);
            }
        }

        private void Insert(
            Cuboid cuboid,
            FreeCuboidChoiceHeuristic cuboidChoice,
            GuillotineSplitHeuristic splitMethod)
        {
            // Find where to put the new cuboid
            var freeNodeIndex = 0;
            FindPositionForNewNode(cuboid, cuboidChoice, out freeNodeIndex);

            // Abort if we didn't have enough space in the bin
            if (!cuboid.IsPlaced)
                return;

            // Remove the space that was just consumed by the new cuboid
            if (freeNodeIndex < 0)
                throw new ArithmeticException("freeNodeIndex < 0");
            SplitFreeCuboidByHeuristic(_freeCuboids[freeNodeIndex], cuboid, splitMethod);
            _freeCuboids.RemoveAt(freeNodeIndex);

            // Remember the new used cuboid
            _usedCuboids.Add(cuboid);
        }

        private void FindPositionForNewNode(
            Cuboid cuboid,
            FreeCuboidChoiceHeuristic cuboidChoice,
            out int freeCuboidIndex)
        {
            var width = cuboid.Width;
            var height = cuboid.Height;
            var depth = cuboid.Depth;
            var bestScore = decimal.MaxValue;
            freeCuboidIndex = -1;

            // Try each free cuboid to find the best one for placement a given cuboid.
            // Rotate a cuboid in every possible way and find which choice is the best.
            for (int index = 0; index < _freeCuboids.Count; ++index)
            {
                var freeCuboid = _freeCuboids[index];

                // Width x Height x Depth
                if (width <= freeCuboid.Width &&
                    height <= freeCuboid.Height &&
                    depth <= freeCuboid.Depth)
                {
                    var score = ScoreByHeuristic(cuboid, freeCuboid, cuboidChoice);
                    if (score < bestScore)
                    {
                        cuboid.IsPlaced = true;
                        cuboid.X = freeCuboid.X;
                        cuboid.Y = freeCuboid.Y;
                        cuboid.Z = freeCuboid.Z;
                        cuboid.Width = width;
                        cuboid.Height = height;
                        cuboid.Depth = depth;
                        bestScore = score;
                        freeCuboidIndex = index;
                    }
                }

                // Width x Depth x Height
                if (width <= freeCuboid.Width &&
                    depth <= freeCuboid.Height &&
                    height <= freeCuboid.Depth)
                {
                    var score = ScoreByHeuristic(cuboid, freeCuboid, cuboidChoice);
                    if (score < bestScore)
                    {
                        cuboid.IsPlaced = true;
                        cuboid.X = freeCuboid.X;
                        cuboid.Y = freeCuboid.Y;
                        cuboid.Z = freeCuboid.Z;
                        cuboid.Width = width;
                        cuboid.Height = depth;
                        cuboid.Depth = height;
                        bestScore = score;
                        freeCuboidIndex = index;
                    }
                }

                // Depth x Height x Width
                if (depth <= freeCuboid.Width &&
                    height <= freeCuboid.Height &&
                    width <= freeCuboid.Depth)
                {
                    var score = ScoreByHeuristic(cuboid, freeCuboid, cuboidChoice);
                    if (score < bestScore)
                    {
                        cuboid.IsPlaced = true;
                        cuboid.X = freeCuboid.X;
                        cuboid.Y = freeCuboid.Y;
                        cuboid.Z = freeCuboid.Z;
                        cuboid.Width = depth;
                        cuboid.Height = height;
                        cuboid.Depth = width;
                        bestScore = score;
                        freeCuboidIndex = index;
                    }
                }

                // Depth x Width x Height
                if (depth <= freeCuboid.Width &&
                    width <= freeCuboid.Height &&
                    height <= freeCuboid.Depth)
                {
                    var score = ScoreByHeuristic(cuboid, freeCuboid, cuboidChoice);
                    if (score < bestScore)
                    {
                        cuboid.IsPlaced = true;
                        cuboid.X = freeCuboid.X;
                        cuboid.Y = freeCuboid.Y;
                        cuboid.Z = freeCuboid.Z;
                        cuboid.Width = depth;
                        cuboid.Height = width;
                        cuboid.Depth = height;
                        bestScore = score;
                        freeCuboidIndex = index;
                    }
                }

                // Height x Width x Depth
                if (height <= freeCuboid.Width &&
                    width <= freeCuboid.Height &&
                    depth <= freeCuboid.Depth)
                {
                    var score = ScoreByHeuristic(cuboid, freeCuboid, cuboidChoice);
                    if (score < bestScore)
                    {
                        cuboid.IsPlaced = true;
                        cuboid.X = freeCuboid.X;
                        cuboid.Y = freeCuboid.Y;
                        cuboid.Z = freeCuboid.Z;
                        cuboid.Width = height;
                        cuboid.Height = width;
                        cuboid.Depth = depth;
                        bestScore = score;
                        freeCuboidIndex = index;
                    }
                }

                // Height x Depth x Width
                if (height <= freeCuboid.Width &&
                    depth <= freeCuboid.Height &&
                    width <= freeCuboid.Depth)
                {
                    var score = ScoreByHeuristic(cuboid, freeCuboid, cuboidChoice);
                    if (score < bestScore)
                    {
                        cuboid.IsPlaced = true;
                        cuboid.X = freeCuboid.X;
                        cuboid.Y = freeCuboid.Y;
                        cuboid.Z = freeCuboid.Z;
                        cuboid.Width = height;
                        cuboid.Height = depth;
                        cuboid.Depth = width;
                        bestScore = score;
                        freeCuboidIndex = index;
                    }
                }
            }
        }

        private decimal ScoreByHeuristic(
            Cuboid cuboid,
            Cuboid freeCuboid,
            FreeCuboidChoiceHeuristic cuboidChoice)
        {
            switch (cuboidChoice)
            {
                case FreeCuboidChoiceHeuristic.CuboidMinHeight:
                    return freeCuboid.Y + cuboid.Height;
                default:
                    throw new NotSupportedException($"cuboid choice is unsupported: {cuboidChoice}");
            }
        }

        private void SplitFreeCuboidByHeuristic(
            Cuboid freeCuboid,
            Cuboid placedCuboid,
            GuillotineSplitHeuristic method)
        {
            // Compute the lengths of the leftover area.
            var w = freeCuboid.Width - placedCuboid.Width;
            var d = freeCuboid.Depth - placedCuboid.Depth;

            // Use the given heuristic to decide which choice to make.

            bool splitHorizontal;
            switch (method)
            {
                case GuillotineSplitHeuristic.SplitShorterLeftoverAxis:
                    // Split along the shorter leftover axis.
                    splitHorizontal = (w <= d);
                    break;
                case GuillotineSplitHeuristic.SplitLongerLeftoverAxis:
                    // Split along the longer leftover axis.
                    splitHorizontal = (w > d);
                    break;
                case GuillotineSplitHeuristic.SplitShorterAxis:
                    // Split along the shorter total axis.
                    splitHorizontal = (freeCuboid.Width <= freeCuboid.Depth);
                    break;
                case GuillotineSplitHeuristic.SplitLongerAxis:
                    // Split along the longer total axis.
                    splitHorizontal = (freeCuboid.Width > freeCuboid.Depth);
                    break;
                default:
                    throw new NotSupportedException($"split method is unsupported: {method}");
            }

            // Perform the actual split.
            SplitFreeCuboidAlongAxis(freeCuboid, placedCuboid, splitHorizontal);
        }

        private void SplitFreeCuboidAlongAxis(
            Cuboid freeCuboid,
            Cuboid placedCuboid,
            bool splitHorizontal)
        {
            var bottom = new Cuboid();
            bottom.X = freeCuboid.X;
            bottom.Y = freeCuboid.Y;
            bottom.Z = freeCuboid.Z + placedCuboid.Depth;
            bottom.Depth = freeCuboid.Depth - placedCuboid.Depth;
            bottom.Height = placedCuboid.Height;

            var right = new Cuboid();
            right.X = freeCuboid.X + placedCuboid.Width;
            right.Y = freeCuboid.Y;
            right.Z = freeCuboid.Z;
            right.Width = freeCuboid.Width - placedCuboid.Width;
            right.Height = placedCuboid.Height;

            var top = new Cuboid();
            top.X = freeCuboid.X;
            top.Y = freeCuboid.Y + placedCuboid.Height;
            top.Z = freeCuboid.Z;
            top.Height = freeCuboid.Height - placedCuboid.Height;
            top.Width = freeCuboid.Width;
            top.Depth = freeCuboid.Depth;

            if (splitHorizontal)
            {
                bottom.Width = freeCuboid.Width;
                right.Depth = placedCuboid.Depth;
            }
            else // Split vertically
            {
                bottom.Width = placedCuboid.Width;
                right.Depth = freeCuboid.Depth;
            }

            // Add new free cuboids.
            if (bottom.Width > 0 && bottom.Height > 0 && bottom.Depth > 0)
                AddFreeCuboid(bottom);
            if (right.Width > 0 && right.Height > 0 && right.Depth > 0)
                AddFreeCuboid(right);
            if (top.Width > 0 && top.Height > 0 && top.Depth > 0)
                AddFreeCuboid(top);
        }

        private void AddFreeCuboid(Cuboid freeCuboid)
        {
            if (freeCuboid.X < 0 || freeCuboid.Y < 0 || freeCuboid.Z < 0)
            {
                throw new ArithmeticException(
                    $"add free cuboid failed: negative position, algorithm: {this}, cuboid: {freeCuboid}");
            }
            if (freeCuboid.X + freeCuboid.Width > _binWidth ||
                freeCuboid.Y + freeCuboid.Height > _binHeight ||
                freeCuboid.Z + freeCuboid.Depth > _binDepth)
            {
                throw new ArithmeticException(
                    $"add free cuboid failed: out of bin, algorithm: {this}, cuboid: {freeCuboid}");
            }
            _freeCuboids.Add(freeCuboid);
        }

        public override string ToString()
        {
            return $"Guillotine({_cuboidChoice}, {_splitMethod})";
        }
    }
}
