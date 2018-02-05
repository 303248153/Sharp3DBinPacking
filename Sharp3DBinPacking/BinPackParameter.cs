using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp3DBinPacking
{
    public class BinPackParameter
    {
        public decimal BinWidth { get; private set; }
        public decimal BinHeight { get; private set; }
        public decimal BinDepth { get; private set; }
        public IEnumerable<Cuboid> Cuboids { get; private set; }

        public BinPackParameter(
            decimal binWidth, decimal binHeight, decimal binDepth, IEnumerable<Cuboid> cuboids)
        {
            BinWidth = binWidth;
            BinHeight = binHeight;
            BinDepth = binDepth;
            Cuboids = cuboids;
        }
    }
}
