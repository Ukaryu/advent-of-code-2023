using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode2023.Day2;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdventOfCode2023.Day3
{
    public static class GearRatios
    {
        private record EngineSchematic(
            IEnumerable<EnginePart> EngineParts,
            IEnumerable<EngineSymbol> EngineSymbols
        );

        private record EnginePart(int PartNo, IEnumerable<Point> DigitLocations);

        private record EngineSymbol(char Symbol, Point Location);

        private static async Task<EngineSchematic> ReadEngineSchematic()
        {
            var engineSchematicLines = await File.ReadAllLinesAsync("./Day3/EngineSchematic.txt");

            var engineParts = new List<EnginePart>();
            var engineSymbols = new List<EngineSymbol>();

            for (var lineIdx = 0; lineIdx < engineSchematicLines.Length; lineIdx++)
            {
                var line = engineSchematicLines[lineIdx];

                string? numberStr = null;
                var numberLocations = new List<Point>();

                for (var charIdx = 0; charIdx < line.Length; charIdx++)
                {
                    var character = line[charIdx];

                    if (char.IsDigit(character))
                    {
                        numberLocations.Add(new(charIdx, lineIdx));

                        if (numberStr == null)
                            numberStr = character.ToString();
                        else
                            numberStr += character.ToString();
                    }

                    var nextCharIdx = charIdx + 1;

                    if (
                        (nextCharIdx == line.Length || !char.IsDigit(line[nextCharIdx]))
                        && numberStr != null
                    )
                    {
                        var number = int.Parse(numberStr);
                        engineParts.Add(new(number, numberLocations));
                        numberStr = null;
                        numberLocations = new List<Point>();
                    }

                    if (character != '.' && !char.IsDigit(character))
                    {
                        engineSymbols.Add(new(character, new Point(charIdx, lineIdx)));
                    }
                }
            }

            return new EngineSchematic(engineParts, engineSymbols);
        }

        private static IEnumerable<EnginePart> GetValidEngineParts(EngineSchematic schematic)
        {
            var validEngineParts = new List<EnginePart>();

            foreach (var enginePart in schematic.EngineParts)
            {
                foreach (var digitLocation in enginePart.DigitLocations)
                {
                    foreach (var engineSymbol in schematic.EngineSymbols)
                    {
                        var dist = Math.Sqrt(
                            Math.Pow(digitLocation.X - engineSymbol.Location.X, 2)
                                + Math.Pow(digitLocation.Y - engineSymbol.Location.Y, 2)
                        );

                        if (Math.Floor(dist) == 1 && !validEngineParts.Contains(enginePart))
                            validEngineParts.Add(enginePart);
                    }
                }
            }

            return validEngineParts;
        }

        private static int GetGearRatios(EngineSchematic schematic)
        {
            var gearRatios = 0;
            var gears = new List<(int, int)>();

            foreach (var engineSymbol in schematic.EngineSymbols.Where(sym => sym.Symbol == '*'))
            {
                EnginePart? firstGear = null;
                EnginePart? secondGear = null;

                foreach (var enginePart in schematic.EngineParts)
                {
                    foreach (var digitLocation in enginePart.DigitLocations)
                    {
                        var dist = Math.Sqrt(
                            Math.Pow(digitLocation.X - engineSymbol.Location.X, 2)
                                + Math.Pow(digitLocation.Y - engineSymbol.Location.Y, 2)
                        );

                        if (Math.Floor(dist) == 1)
                        {
                            if (firstGear == null)
                                firstGear = enginePart;
                            else if (secondGear == null && firstGear != enginePart)
                                secondGear = enginePart;
                        }
                    }

                    if (firstGear != null && secondGear != null)
                        break;
                }

                if (firstGear != null && secondGear != null)
                {
                    gears.Add((firstGear.PartNo, secondGear.PartNo));
                    firstGear = null;
                    secondGear = null;
                }
            }

            gearRatios = gears.Sum(gear => gear.Item1 * gear.Item2);

            return gearRatios;
        }

        public static async Task<int> GetSumOfAllPartNumbers()
        {
            var engineSchematic = await ReadEngineSchematic();

            var validEngineParts = GetValidEngineParts(engineSchematic);
            var sumOfValidEngineParts = validEngineParts.Sum(part => part.PartNo);

            var gearRations = GetGearRatios(engineSchematic);

            return gearRatios;
        }
    }
}
