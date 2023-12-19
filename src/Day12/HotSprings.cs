namespace AdventOfCode2023
{
    public static class HotSprings
    {
        private record ConditionRecord(string Arrangement, List<int> GroupSizes);

        private static async Task<IEnumerable<ConditionRecord>> ReadConditionRecords()
        {
            var conditionRecordLines = await File.ReadAllLinesAsync("./Day12/ConditionRecords.txt");

            var conditionRecords = new List<ConditionRecord>();

            foreach (var conditionRecordLine in conditionRecordLines)
            {
                var arrangmentAndGroupSize = conditionRecordLine.Split(' ').ToList();

                var arrangement = arrangmentAndGroupSize[0];
                var groupSizes = arrangmentAndGroupSize[1].Split(',').Select(int.Parse).ToList();

                conditionRecords.Add(new(arrangement, groupSizes));
            }

            return conditionRecords;
        }

        private static int GetAllPossibleArrangements(IEnumerable<ConditionRecord> records)
        {
            var sumPossibleArrangements = 0;
            foreach (var record in records)
            {
                sumPossibleArrangements += GetPossibleArrangmentsCount(
                    record.Arrangement,
                    record.GroupSizes.ToArray()
                );
            }

            return sumPossibleArrangements;
        }

        private static int GetPossibleArrangmentsCount(string arrangement, int[] sizes)
        {
            var arrangementsCount = 0;

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

            if (".?".Contains(arrangement[0]))
                arrangementsCount += GetPossibleArrangmentsCount(arrangement[1..], sizes);

            if ("#?".Contains(arrangement[0]))
                if (
                    sizes[0] <= arrangement.Length
                    && !arrangement[..sizes[0]].Contains('.')
                    && (sizes[0] == arrangement.Length || arrangement[sizes[0]] != '#')
                )
                    if (sizes[0] + 1 <= arrangement.Length)
                        arrangementsCount += GetPossibleArrangmentsCount(
                            arrangement[(sizes[0] + 1)..],
                            sizes[1..]
                        );
                    else
                        arrangementsCount += GetPossibleArrangmentsCount(
                            arrangement[sizes[0]..],
                            sizes[1..]
                        );
                else
                    arrangementsCount += 0;

            return arrangementsCount;
        }

        public static async Task<int> GetSumOfAmountOfPossibleArrangements()
        {
            var records = await ReadConditionRecords();
            var possibleArrangements = GetAllPossibleArrangements(records);
            return possibleArrangements;
        }
    }
}
