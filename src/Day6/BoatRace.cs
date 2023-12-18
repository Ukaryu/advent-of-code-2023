using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day6
{
    public static class BoatRace
    {
        private record RaceRecord(long Time, long Distance);

        private static async Task<List<RaceRecord>> ReadRaceRecordsFromFile()
        {
            var almanacLines = await File.ReadAllLinesAsync("./Day6/RaceRecords.txt");
            var raceRecords = new List<RaceRecord>();

            var times = GetNumbersFromStringArr(almanacLines[0].Split(" "));
            var distances = GetNumbersFromStringArr(almanacLines[1].Split(" "));

            for (int i = 0; i < times.Length; i++)
                raceRecords.Add(new RaceRecord(times[i], distances[i]));

            return raceRecords;
        }

        private static long[] GetNumbersFromStringArr(string[] strings)
        {
            var numbers = new List<long>();

            foreach (var str in strings)
                if (long.TryParse(str, out long number))
                    numbers.Add(number);

            return numbers.ToArray();
        }

        private static long CalculateDistance(long timeHeld, long timeLimit)
        {
            var timeRemaining = timeLimit - timeHeld;

            var distanceMoved = timeHeld * timeRemaining;

            return distanceMoved;
        }

        private static long GetCountOfWaysToWin(RaceRecord raceRecord)
        {
            var winCount = 0;
            var timeHeld = 1;

            while (timeHeld < raceRecord.Time)
            {
                var distanceMoved = CalculateDistance(timeHeld, raceRecord.Time);

                if (distanceMoved > raceRecord.Distance)
                    winCount++;

                timeHeld++;
            }

            return winCount;
        }

        private static long GetMultiplicationOfWins(List<RaceRecord> raceRecords)
        {
            long sum = 1;
            foreach (var raceRecord in raceRecords)
            {
                sum *= GetCountOfWaysToWin(raceRecord);
            }

            return sum;
        }

        public static async Task<long> GetPowerOfWaysToWin()
        {
            var records = await ReadRaceRecordsFromFile();
            var power = GetMultiplicationOfWins(records);
            return power;
        }
    }
}
