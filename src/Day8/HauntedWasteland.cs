using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static AdventOfCode2023.Day7.CamelCards;

namespace AdventOfCode2023.Day8
{
    public static class HauntedWasteland
    {
        private record Instruction(IEnumerable<char> Directions, IEnumerable<Element> Elements);

        private record Element(string Name, string LeftElementName, string RightElementName);

        private static async Task<Instruction> ReadInstructions()
        {
            var instructionLines = await File.ReadAllLinesAsync("./Day8/Instructions.txt");
            var instructions = instructionLines[0].AsEnumerable();
            var elements = new List<Element>();

            foreach (var instructionLine in instructionLines.Skip(2))
            {
                var firstSplits = instructionLine.Split(" = ");

                var secondSplit = firstSplits[1].Replace("(", "").Replace(")", "").Split(", ");

                elements.Add(new(firstSplits[0], secondSplit[0], secondSplit[1]));
            }
            return new Instruction(instructions, elements);
        }

        private static long Navigate(Instruction instruction)
        {
            var expectedEnd = "Z";

            var currElems = instruction.Elements.Where(e => e.Name.EndsWith('A')).ToHashSet();
            var elementsLookup = instruction.Elements.ToLookup(e => e.Name);
            var stepsList = new List<int>();
            foreach (var currElem in currElems)
            {
                var currElemCopy = currElem with { };
                int steps = 0;

                while (!currElemCopy.Name.EndsWith(expectedEnd))
                {
                    foreach (var direction in instruction.Directions)
                    {
                        if (direction == 'R')
                        {
                            currElemCopy = elementsLookup[currElemCopy.RightElementName].First();
                        }
                        else if (direction == 'L')
                        {
                            currElemCopy = elementsLookup[currElemCopy.LeftElementName].First();
                        }
                        steps++;
                    }
                }

                stepsList.Add(steps);
            }

            long lcm = 0;

            for (int stepIdx = 0; stepIdx < stepsList.Count; stepIdx++)
            {
                var step = stepsList[stepIdx];
                if (lcm == 0)
                {
                    stepIdx++;
                    var nextStep = stepsList[stepIdx];
                    lcm = GetLeastCommonMultiple(step, nextStep);
                }
                else
                    lcm = GetLeastCommonMultiple(lcm, step);
            }

            return lcm;
        }

        //Basically just a copout
        private static long GetGreatestCommonFactor(long a, long b)
        {
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        private static long GetLeastCommonMultiple(long a, long b)
        {
            return (a / GetGreatestCommonFactor(a, b)) * b;
        }

        public static async Task<long> GetRequiredSteps()
        {
            var instructions = await ReadInstructions();
            var steps = Navigate(instructions);
            return steps;
        }
    }
}
