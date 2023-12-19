using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AdventOfCode2023.Day9
{
    public static class MirageMaintenance
    {
        private static async Task<IEnumerable<int[]>> ReadOasisValues()
        {
            var oasisLines = await File.ReadAllLinesAsync("./Day9/Oasis.txt");

            var oasisVals = new List<int[]>();
            foreach (var oasisLine in oasisLines)
                oasisVals.Add(oasisLine.Split(' ').Select(int.Parse).ToArray());

            return oasisVals;
        }

        private static int GetNextValueOfSequence(int[] oasisSeq)
        {
            var calcSeq = new List<List<int>>() { oasisSeq.ToList() };
            var seqToEval = oasisSeq;
            do
            {
                var seq = new List<int>();

                for (int oasisIdx = 0; oasisIdx < seqToEval.Length - 1; oasisIdx++)
                {
                    var firstVal = seqToEval[oasisIdx];
                    var secondVal = seqToEval[oasisIdx + 1];
                    seq.Add(secondVal - firstVal);
                }
                seqToEval = seq.ToArray();
                calcSeq.Add(seq);
            } while (!calcSeq.Last().All(i => i == 0));

            var nextVal = 0;

            foreach (var seq in calcSeq)
                nextVal += seq.Last();

            return nextVal;
        }

        private static int GetPreviousValueOfSequence(int[] oasisSeq)
        {
            var calcSeq = new List<List<int>>() { oasisSeq.ToList() };
            var seqToEval = oasisSeq;
            do
            {
                var seq = new List<int>();

                for (int oasisIdx = 0; oasisIdx < seqToEval.Length - 1; oasisIdx++)
                {
                    var firstVal = seqToEval[oasisIdx];
                    var secondVal = seqToEval[oasisIdx + 1];
                    seq.Add(secondVal - firstVal);
                }
                seqToEval = seq.ToArray();
                calcSeq.Add(seq);
            } while (!calcSeq.Last().All(i => i == 0));

            calcSeq.Reverse();
            var previousVal = 0;

            foreach (var seq in calcSeq)
                previousVal = seq.First() - previousVal;

            return previousVal;
        }

        private static IEnumerable<int> GetAllNextValuesInSequences(
            IEnumerable<int[]> oasisSequences
        )
        {
            var nextVals = new List<int>();

            foreach (var oasisSequence in oasisSequences)
            {
                var nextValue = GetNextValueOfSequence(oasisSequence);

                nextVals.Add(nextValue);
            }

            return nextVals;
        }

        private static IEnumerable<int> GetAllPreviousValuesInSequences(
            IEnumerable<int[]> oasisSequences
        )
        {
            var nextVals = new List<int>();

            foreach (var oasisSequence in oasisSequences)
            {
                var nextValue = GetPreviousValueOfSequence(oasisSequence);

                nextVals.Add(nextValue);
            }

            return nextVals;
        }

        public static async Task<int> GetSumOfExtrapolatedValues()
        {
            var oasisVals = await ReadOasisValues();
            var nextVals = GetAllNextValuesInSequences(oasisVals);
            var previousVals = GetAllPreviousValuesInSequences(oasisVals);

            return previousVals.Sum();
        }
    }
}
