using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static AdventOfCode2023.Day7.CamelCards;

namespace AdventOfCode2023.Day8
{
    public static class HauntedWasteland
    {
        private static async Task<IEnumerable<Instruction>> ReadInstructions()
        {
            var instructionLines = await File.ReadAllLinesAsync("./Day8/Instructions.txt");
            var instructions = new List<Instruction>();

            foreach (var instructionLine in instructionLines)
            {
                var firstSplits = instructionLine.Split(" = ");

                var secondSplit = firstSplits[1].Replace("(", "").Replace(")", "").Split(", ");

                instructions.Add(new(firstSplits[0], secondSplit[0], secondSplit[1]));
            }

            return instructions;
        }

        private static int Navigate(IEnumerable<Instruction> instructions)
        {
            var expectedEnd = "ZZZ";
            var steps = 0;

            var currentInstruction = instructions.First();

            var traversedInstructions = new List<Instruction>();

            while (currentInstruction.Name != expectedEnd)
            {
                var previousInstruction = currentInstruction;
                steps++;

                if (!traversedInstructions.Contains(currentInstruction))
                {
                    currentInstruction = instructions
                        .Where(i => i.Name == currentInstruction.Right)
                        .First();
                }
                else
                {
                    currentInstruction = instructions
                        .Where(i => i.Name == currentInstruction.Left)
                        .First();
                }

                traversedInstructions.Add(previousInstruction);
            }

            return steps;
        }

        public static async Task GetRequiredSteps()
        {
            var instructions = await ReadInstructions();
            var steps = Navigate(instructions);
        }

        private record Instruction(string Name, string Left, string Right);
    }
}
