using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Win32.SafeHandles;

namespace AdventOfCode2023
{
    public static class HotSprings
    {
        private record ConditionRecord(string Arrangement, int[] GroupSizes);

        private static async Task<IEnumerable<ConditionRecord>> ReadConditionRecords()
        {
            var conditionRecordLines = await File.ReadAllLinesAsync("./Day12/ConditionRecords.txt");

            var conditionRecords = new List<ConditionRecord>();

            foreach (var conditionRecordLine in conditionRecordLines)
            {
                var arrangmentAndGroupSize = conditionRecordLine.Split(' ').ToList();

                var arrangement = arrangmentAndGroupSize[0];

                var unfoldedArrangement = arrangement;
                for (int i = 0; i < 4; i++)
                    unfoldedArrangement += '?' + arrangement;

                var groupSizes = arrangmentAndGroupSize[1].Split(',').Select(int.Parse).ToList();

                var unfoldedGroupSizes = new List<int>();
                for (int i = 0; i < 5; i++)
                    unfoldedGroupSizes.AddRange(groupSizes);

                conditionRecords.Add(new(unfoldedArrangement, [.. unfoldedGroupSizes]));
            }

            return conditionRecords;
        }

        private static long GetAllPossibleArrangements(IEnumerable<ConditionRecord> records)
        {
            long sumPossibleArrangements = 0;
            foreach (var record in records)
            {
                sumPossibleArrangements += GetPossibleArrangmentsCount(
                    record.Arrangement,
                    record.GroupSizes
                );
            }

            return sumPossibleArrangements;
        }

        //private static Dictionary<string, long> EvaluatedArrangements =
        //    new Dictionary<string, long>();
        //private static Func<string, int[], long> CachedFunc = Memoized.Of<string, int[], long>(
        //    GetPossibleArrangmentsCount,
        //    new MyEqualityComparer()
        //);

        static Dictionary<(string, int[]), long> EvaluatedArrangements =
            new(new MyEqualityComparer());

        private static long GetPossibleArrangmentsCount(string arrangement, int[] sizes)
        {
            long arrangementsCount = 0;

            if (arrangement == "")
                if (sizes.Length == 0)
                    return 1;
                else
                    return 0;

            if (sizes.Length == 0)
                if (arrangement.Contains('#'))
                    return 0;
                else
                    return 1;

            if (EvaluatedArrangements.TryGetValue((arrangement, sizes), out long cachedCount))
                return cachedCount;

            if (".?".Contains(arrangement[0]))
                arrangementsCount += GetPossibleArrangmentsCount(arrangement[1..], sizes);

            if ("#?".Contains(arrangement[0]))
                if (
                    sizes[0] <= arrangement.Length
                    && !arrangement[..sizes[0]].Contains('.')
                    && (sizes[0] == arrangement.Length || arrangement[sizes[0]] != '#')
                )
                {
                    var arrStart = sizes[0] + 1 <= arrangement.Length ? sizes[0] + 1 : sizes[0];
                    arrangementsCount += GetPossibleArrangmentsCount(
                        arrangement[arrStart..],
                        sizes[1..]
                    );
                }
                else
                    arrangementsCount += 0;

            EvaluatedArrangements.Add((arrangement, sizes), arrangementsCount);

            return arrangementsCount;
        }

        public static async Task<long> GetSumOfAmountOfPossibleArrangements()
        {
            var records = await ReadConditionRecords();
            var possibleArrangements = GetAllPossibleArrangements(records);
            return possibleArrangements;
        }
    }

    public class Memoized
    {
        public static Func<T1, T2, TRet> Of<T1, T2, TRet>(Func<T1, T2, TRet> f)
        {
            ConcurrentDictionary<(T1, T2), TRet> cache = new ConcurrentDictionary<(T1, T2), TRet>();

            return (arg1, arg2) =>
                cache.GetOrAdd((arg1, arg2), (xarg) => f(xarg.Item1, xarg.Item2));
        }

        public static Func<T1, T2, TRet> Of<T1, T2, TRet>(
            Func<T1, T2, TRet> f,
            IEqualityComparer<(T1, T2)> equalityComparer
        )
        {
            ConcurrentDictionary<(T1, T2), TRet> cache = new ConcurrentDictionary<(T1, T2), TRet>(
                equalityComparer
            );

            return (arg1, arg2) =>
                cache.GetOrAdd((arg1, arg2), (xarg) => f(xarg.Item1, xarg.Item2));
        }
    }

    public class MyEqualityComparer : IEqualityComparer<(string, int[])>
    {
        public bool Equals((string, int[]) x, (string, int[]) y)
        {
            if (x.Item2.Length != y.Item2.Length)
                return false;

            for (int i = 0; i < x.Item2.Length; i++)
                if (x.Item2[i] != y.Item2[i])
                    return false;

            if (!x.Item1.Equals(x.Item1))
                return false;

            return true;
        }

        public int GetHashCode([DisallowNull] (string, int[]) obj)
        {
            int result = 17;
            for (int i = 0; i < obj.Item2.Length; i++)
            {
                unchecked
                {
                    result = result * 23 + obj.Item2[i].GetHashCode();
                }
            }
            result += obj.Item1.Select(c => c.GetHashCode()).Sum();

            return result;
        }
    }
}
