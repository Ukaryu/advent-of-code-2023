using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    public record Game(int Id, IEnumerable<CubePull> CubePulls);

    public record CubePull(int RedCubesCount, int BlueCubesCount, int GreenCubesCount);

    public static class CubeConundrum
    {
        private const int MaxRedCubes = 12;
        private const int MaxGreenCubes = 13;
        private const int MaxBlueCubes = 14;

        private static async Task<IEnumerable<Game>> ReadGameRecordFile()
        {
            var games = new List<Game>();

            var gameRecords = await File.ReadAllLinesAsync("./Day2/GameRecord.txt");

            foreach (var gameRecord in gameRecords)
            {
                var splitGameRecord = gameRecord.Split(": ");
                var gameId = int.Parse(splitGameRecord[0].Replace("Game ", ""));
                var cubePullStrings = splitGameRecord[1].Split("; ");

                var cubePulls = new List<CubePull>();

                foreach (var cubePullString in cubePullStrings)
                {
                    var cubeStrings = cubePullString.Split(", ");

                    var redCount = 0;
                    var blueCount = 0;
                    var greenCount = 0;

                    foreach (var cubeString in cubeStrings)
                    {
                        var cubeAndCount = cubeString.Split(" ");
                        switch (cubeAndCount[1])
                        {
                            case "blue":
                                blueCount += int.Parse(cubeAndCount[0]);
                                break;
                            case "red":
                                redCount += int.Parse(cubeAndCount[0]);
                                break;
                            case "green":
                                greenCount += int.Parse(cubeAndCount[0]);
                                break;
                            default:
                                break;
                        }
                    }

                    cubePulls.Add(new CubePull(redCount, blueCount, greenCount));
                }

                games.Add(new Game(gameId, cubePulls));
            }

            return games;
        }

        private static IEnumerable<Game> GetValidGames(IEnumerable<Game> games)
        {
            var validGames = new List<Game>();

            foreach (var game in games)
            {
                var valid = true;

                foreach (var pull in game.CubePulls)
                {
                    if (
                        pull.RedCubesCount > MaxRedCubes
                        || pull.BlueCubesCount > MaxBlueCubes
                        || pull.GreenCubesCount > MaxGreenCubes
                    )
                        valid = false;
                }

                if (valid)
                    validGames.Add(game);
            }

            return validGames;
        }

        private static int GetSumOfPowerOfGames(IEnumerable<Game> games)
        {
            var sumOfPowerOfGames = 0;

            foreach (var game in games)
            {
                var minRedCount = 0;
                var minBlueCount = 0;
                var minGreenCount = 0;

                foreach (var pull in game.CubePulls)
                {
                    if (pull.RedCubesCount > minRedCount)
                        minRedCount = pull.RedCubesCount;

                    if (pull.BlueCubesCount > minBlueCount)
                        minBlueCount = pull.BlueCubesCount;

                    if (pull.GreenCubesCount > minGreenCount)
                        minGreenCount = pull.GreenCubesCount;
                }

                sumOfPowerOfGames += minRedCount * minBlueCount * minGreenCount;
            }

            return sumOfPowerOfGames;
        }

        public static async Task<int> GetSumOfValidGameIds()
        {
            var games = await ReadGameRecordFile();

            var validGames = GetValidGames(games);

            var sumValidGameIds = validGames.Sum(g => g.Id);

            var sumOfPowerOfGames = GetSumOfPowerOfGames(games);

            return sumOfPowerOfGames;
        }
    }
}
