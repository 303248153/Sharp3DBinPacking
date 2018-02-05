using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp3DBinPacking
{
    public class BinPackResult
    {
        public IList<IList<Cuboid>> BestResult { get; private set; }
        public string BestAlgorithmName { get; private set; }

        public BinPackResult(IList<IList<Cuboid>> bestResult, string bestAlgorithmName)
        {
            BestResult = bestResult;
            BestAlgorithmName = bestAlgorithmName;
        }
    }
}
