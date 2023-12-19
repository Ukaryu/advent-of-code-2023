﻿using AdventOfCode2023;

namespace AdventOfCode2023
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Choose day:");
            var day = Console.ReadLine();
            int parsedDay = 0;

            while (!int.TryParse(day, out parsedDay))
            {
                Console.WriteLine("Input a integer!");
                day = Console.ReadLine();
                Console.Clear();
                Console.WriteLine("Choose day:");
                Console.WriteLine(day);
            }

            switch (parsedDay)
            {
                case 1:
                    Console.WriteLine(await Trebuchet.SumCalibrationValues());
                    break;
                case 2:
                    Console.WriteLine(await CubeConundrum.GetSumOfValidGameIds());
                    break;
                case 3:
                    Console.WriteLine(await GearRatios.GetSumOfAllPartNumbers());
                    break;
                case 4:
                    Console.WriteLine(await Scratchcards.GetSumOfWinningScratchCards());
                    break;
                case 5:
                    Console.WriteLine(await FoodProduction.GetLowestLocationNumber());
                    break;
                case 6:
                    Console.WriteLine(await BoatRace.GetPowerOfWaysToWin());
                    break;
                case 7:
                    Console.WriteLine(await CamelCards.GetTotalWinnings());
                    break;
                case 8:
                    Console.WriteLine(await HauntedWasteland.GetRequiredSteps());
                    break;
                case 9:
                    Console.WriteLine(await MirageMaintenance.GetSumOfExtrapolatedValues());
                    break;
                case 10:
                    Console.WriteLine(await PipeMaze.GetStepsFarthestFromStarting());
                    break;
                case 11:
                    Console.WriteLine(await CosmicExpansion.GetSumOfShortestPathsBetweenGalaxies());
                    break;
                default:
                    Console.WriteLine("No valid day chosen");
                    break;
            }
        }
    }
}
