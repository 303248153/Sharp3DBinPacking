using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp3DBinPacking
{
    public class Cuboid
    {
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Depth { get; set; }
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public decimal Z { get; set; }
        internal bool IsPlaced { get; set; }

        public Cuboid() { }
        public Cuboid(decimal width, decimal height, decimal depth) : this(width, height, depth, 0, 0, 0) { }
        public Cuboid(decimal width, decimal height, decimal depth, decimal x, decimal y, decimal z)
        {
            Width = width;
            Height = height;
            Depth = depth;
            X = x;
            Y = y;
            Z = z;
        }

        public void ResetPlacedInformation()
        {
            X = 0;
            Y = 0;
            Z = 0;
            IsPlaced = false;
        }

        public override string ToString()
        {
            return $"Cuboid(X: {X}, Y: {Y}, Z:{Z}, Width: {Width}, Height:{Height}, Depth:{Depth})";
        }
    }
}
