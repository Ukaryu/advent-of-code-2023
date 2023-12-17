using AdventOfCode2023.Day1;
using AdventOfCode2023.Day2;
using AdventOfCode2023.Day3;
using AdventOfCode2023.Day7;
using AdventOfCode2023.Day8;

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
                case 7:
                    Console.WriteLine(await CamelCards.GetTotalWinnings());
                    break;
                case 8:
                    await HauntedWasteland.GetRequiredSteps();
                    break;
                default:
                    Console.WriteLine("No valid day chosen");
                    break;
            }
        }
    }
}
