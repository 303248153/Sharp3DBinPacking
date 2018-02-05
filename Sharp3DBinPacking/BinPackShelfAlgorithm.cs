﻿using Sharp3DBinPacking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharp3DBinPacking
{
    public class BinPackShelfAlgorithm : IBinPackAlgorithm
    {
        private readonly decimal _binWidth;
        private readonly decimal _binHeight;
        private readonly decimal _binDepth;
        private readonly FreeRectChoiceHeuristic _rectChoice;
        private readonly GuillotineSplitHeuristic _splitMethod;
        private readonly ShelfChoiceHeuristic _shelfChoice;
        // stores the starting y coordinate of the latest(topmost) shelf
        private decimal _currentY;
        private readonly IList<Shelf> _shelves;

        public BinPackShelfAlgorithm(
            decimal binWidth,
            decimal binHeight,
            decimal binDepth,
            FreeRectChoiceHeuristic rectChoice,
            GuillotineSplitHeuristic splitMethod,
            ShelfChoiceHeuristic shelfChoice)
        {
            _binWidth = binWidth;
            _binHeight = binHeight;
            _binDepth = binDepth;
            _rectChoice = rectChoice;
            _splitMethod = splitMethod;
            _shelfChoice = shelfChoice;
            _currentY = 0;
            _shelves = new List<Shelf>();
            StartNewShelf(0);
        }

        public void Insert(IEnumerable<Cuboid> cuboids)
        {
            foreach (var cuboid in cuboids)
            {
                Insert(cuboid, _shelfChoice);
            }
        }

        private void Insert(Cuboid cuboid, ShelfChoiceHeuristic method)
        {
            switch (method)
            {
                case ShelfChoiceHeuristic.ShelfNextFit:
                    PutOnShelf(_shelves.Last(), cuboid);
                    if (cuboid.IsPlaced)
                    {
                        AddToShelf(_shelves.Last(), cuboid);
                        return;
                    }
                    break;

                case ShelfChoiceHeuristic.ShelfFirstFit:
                    foreach (var shelf in _shelves)
                    {
                        PutOnShelf(shelf, cuboid);
                        if (cuboid.IsPlaced)
                        {
                            AddToShelf(shelf, cuboid);
                            return;
                        }
                    }
                    break;
            }

            // The rectangle did not fit on any of the shelves. Open a new shelf.

            // Sort edges in decreasing order
            var edges = new List<decimal>() { cuboid.Width, cuboid.Height, cuboid.Depth };
            edges.Sort();
            var max = edges[2];
            var middle = edges[1];
            var min = edges[0];

            var whdSet = new[]
            {
                new { w = middle, h = max, d = min },
                new { w = max, h = middle, d = min },
                new { w = middle, h = min, d = max }
            };
            foreach (var whd in whdSet)
            {
                cuboid.Width = whd.w;
                cuboid.Height = whd.h;
                cuboid.Depth = whd.d;
                if (CanStartNewShelf(cuboid.Height))
                {
                    StartNewShelf(cuboid.Height);
                    PutOnShelf(_shelves.Last(), cuboid);
                    if (cuboid.IsPlaced)
                    {
                        AddToShelf(_shelves.Last(), cuboid);
                        return;
                    }
                }
            }

            // The rectangle didn't fit.
        }

        private void PutOnShelf(Shelf shelf, Cuboid cuboid)
        {
            var width = cuboid.Width;
            var height = cuboid.Height;
            var depth = cuboid.Depth;

            // Sort edges in decreasing order
            var edges = new List<decimal>() { width, height, depth };
            edges.Sort();
            var max = edges[2];
            var middle = edges[1];
            var min = edges[0];

            // Set cuboid's longest egde vertically
            if (max > shelf.Height)
            {
                // pass
            }
            else
            {
                var maxVerticalRect = new Rectangle(middle, min, 0, 0);
                var freeRectIndex = 0;
                shelf.Guillotine.Insert(maxVerticalRect, _rectChoice, out freeRectIndex);
                if (maxVerticalRect.IsPlaced)
                {
                    shelf.Guillotine.InsertOnPosition(maxVerticalRect, _splitMethod, freeRectIndex);
                    cuboid.IsPlaced = true;
                    cuboid.Width = maxVerticalRect.Width;
                    cuboid.Height = max;
                    cuboid.Depth = maxVerticalRect.Height;
                    cuboid.X = maxVerticalRect.X;
                    cuboid.Z = maxVerticalRect.Y;
                    return;
                }
            }

            // Set cuboid's second longest egde vertically
            if (middle > shelf.Height)
            {
                // pass
            }
            else
            {
                var middleVerticalRect = new Rectangle(min, max, 0, 0);
                var freeRectIndex = 0;
                shelf.Guillotine.Insert(middleVerticalRect, _rectChoice, out freeRectIndex);
                if (middleVerticalRect.IsPlaced)
                {
                    shelf.Guillotine.InsertOnPosition(middleVerticalRect, _splitMethod, freeRectIndex);
                    cuboid.IsPlaced = true;
                    cuboid.Width = middleVerticalRect.Width;
                    cuboid.Height = middle;
                    cuboid.Depth = middleVerticalRect.Height;
                    cuboid.X = middleVerticalRect.X;
                    cuboid.Z = middleVerticalRect.Y;
                    return;
                }
            }

            // Set cuboid's smallest egde vertically
            if (min > shelf.Height)
            {
                // pass
            }
            else
            {
                var minVerticalRect = new Rectangle(middle, max, 0, 0);
                var freeRectIndex = 0;
                shelf.Guillotine.Insert(minVerticalRect, _rectChoice, out freeRectIndex);
                if (minVerticalRect.IsPlaced)
                {
                    shelf.Guillotine.InsertOnPosition(minVerticalRect, _splitMethod, freeRectIndex);
                    cuboid.IsPlaced = true;
                    cuboid.Width = minVerticalRect.Width;
                    cuboid.Height = min;
                    cuboid.Depth = minVerticalRect.Height;
                    cuboid.X = minVerticalRect.X;
                    cuboid.Z = minVerticalRect.Y;
                    return;
                }
            }

            // Place failed
        }

        private void AddToShelf(Shelf shelf, Cuboid newCuboid)
        {
            if (shelf.Height < newCuboid.Height)
                throw new ArithmeticException("shelf.Height < newCuboid.Height");
            newCuboid.Y = shelf.StartY;
        }

        private bool CanStartNewShelf(decimal height)
        {
            var lastShelf = _shelves.Last();
            return lastShelf.StartY + lastShelf.Height + height <= _binHeight;
        }

        private void StartNewShelf(decimal startingHeight)
        {
            if (_shelves.Count > 0)
                _currentY += _shelves.Last().Height;
            var shelf = new Shelf(_currentY, startingHeight, _binWidth, _binDepth);
            _shelves.Add(shelf);
        }

        public override string ToString()
        {
            return $"Shelf({_rectChoice}, {_splitMethod}, {_shelfChoice})";
        }
    }
}
