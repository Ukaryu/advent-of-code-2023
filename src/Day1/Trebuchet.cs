using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    public static class Trebuchet
    {
        private static async Task<string[]> ReadCalibrationValuesFile()
        {
            var calibrationValues = await File.ReadAllLinesAsync("./Day1/CalibrationValues.txt");

            return calibrationValues;
        }

        private static async Task<List<int>> ParseCalibrationValues()
        {
            var calibrationValues = await ReadCalibrationValuesFile();

            var parsedCalibrationValues = new List<int>();

            foreach (var calibrationValue in calibrationValues.ToList())
            {
                if (calibrationValue != null)
                {
                    string? firstIntCharacter = null;
                    string? lastIntCharacter = null;
                    for (int i = 0; i < calibrationValue.Length; i++)
                    {
                        if (char.IsNumber(calibrationValue[i]))
                        {
                            if (firstIntCharacter == null)
                                firstIntCharacter = calibrationValue[i].ToString();
                            else
                                lastIntCharacter = calibrationValue[i].ToString();
                        }
                        else
                        {
                            var parsedDigit = DigitWordsToInt(calibrationValue.Substring(i));
                            if (parsedDigit != null)
                            {
                                if (firstIntCharacter == null)
                                    firstIntCharacter = parsedDigit.ToString();
                                else
                                    lastIntCharacter = parsedDigit.ToString();
                            }
                        }
                    }

                    if (
                        int.TryParse(
                            firstIntCharacter + (lastIntCharacter ?? firstIntCharacter),
                            out int parsedCharacters
                        )
                    )
                        parsedCalibrationValues.Add(parsedCharacters);
                }
            }

            return parsedCalibrationValues;
        }

        public static async Task<int> SumCalibrationValues()
        {
            var parsedCalibrationValues = await ParseCalibrationValues();

            return parsedCalibrationValues.Sum();
        }

        public static int? DigitWordsToInt(string digitWord) =>
            digitWord switch
            {
                string s when s.StartsWith("one") => 1,
                string s when s.StartsWith("two") => 2,
                string s when s.StartsWith("three") => 3,
                string s when s.StartsWith("four") => 4,
                string s when s.StartsWith("five") => 5,
                string s when s.StartsWith("six") => 6,
                string s when s.StartsWith("seven") => 7,
                string s when s.StartsWith("eight") => 8,
                string s when s.StartsWith("nine") => 9,
                _ => null
            };
    }
}
