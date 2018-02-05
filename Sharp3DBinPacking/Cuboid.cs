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
        internal decimal Score { get; set; } = decimal.MaxValue;

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
            Score = decimal.MaxValue;
        }

        public static bool CompareMaxEdgeGT(Cuboid i, Cuboid j)
        {
            decimal iMax = Math.Max(Math.Max(i.Width, i.Height), i.Depth);
            decimal jMax = Math.Max(Math.Max(j.Width, j.Height), j.Depth);
            return iMax > jMax;
        }

        public static bool CompareVolumeGT(Cuboid i, Cuboid j)
        {
            return (i.Width * i.Height * i.Depth) > (j.Width * j.Height * j.Depth);
        }

        public static bool CompareVolumeLT(Cuboid i, Cuboid j)
        {
            return (i.Width * i.Height * i.Depth) < (j.Width * j.Height * j.Depth);
        }

        public override string ToString()
        {
            return $"Cuboid(X: {X}, Y: {Y}, Z:{Z}, Width: {Width}, Height:{Height}, Depth:{Depth})";
        }
    }
}
