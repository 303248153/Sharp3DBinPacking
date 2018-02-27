using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp3DBinPacking
{
    public class BinPackResult
    {
        public IList<IList<Cuboid>> BestResult { get; private set; }
        public IList<IList<IList<Cuboid>>> AllResults { get; private set; }
        public string BestAlgorithmName { get; private set; }

        public BinPackResult(IList<IList<Cuboid>> bestResult, string bestAlgorithmName) :
            this(bestResult, new List<IList<IList<Cuboid>>>(), bestAlgorithmName)
        { }

        public BinPackResult(
            IList<IList<Cuboid>> bestResult,
            IList<IList<IList<Cuboid>>> allResults,
            string bestAlgorithmName)
        {
            BestResult = bestResult;
            AllResults = allResults;
            BestAlgorithmName = bestAlgorithmName;
        }
    }
}
