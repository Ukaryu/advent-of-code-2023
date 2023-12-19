using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    public static class Scratchcards
    {
        private record Card(int Id, IEnumerable<int> WinningNumbers, IEnumerable<int> CardNumbers);

        private static async Task<IEnumerable<Card>> ReadScratchCards()
        {
            var scratchCardLines = await File.ReadAllLinesAsync("./Day4/Scratchcards.txt");
            var cards = new List<Card>();

            foreach (var cardLine in scratchCardLines)
            {
                var cardIdAndNumbers = cardLine.Split(": ");

                var cardId = int.Parse(
                    cardIdAndNumbers[0].Replace("card", "", StringComparison.OrdinalIgnoreCase)
                );

                var winningNumbersAndNumbers = cardIdAndNumbers[1].Split(" | ");

                var winningNumbersStrs = winningNumbersAndNumbers[0].Split(" ");
                var winningNumbers = ParseNumbersFromStringArr(winningNumbersStrs);

                var numbersStrs = winningNumbersAndNumbers[1].Split(" ");
                var numbers = ParseNumbersFromStringArr(numbersStrs);

                cards.Add(new(cardId, winningNumbers, numbers));
            }

            return cards;
        }

        private static IEnumerable<int> ParseNumbersFromStringArr(string[] numberStrs)
        {
            var numbers = new List<int>();
            foreach (var numberStr in numberStrs)
                if (int.TryParse(numberStr, out int number))
                    numbers.Add(number);

            return numbers;
        }

        private static int GetWinningPoints(IEnumerable<Card> cards)
        {
            var totalPoints = 0;
            foreach (var card in cards)
            {
                var points = 0;

                foreach (var cardNum in card.CardNumbers)
                    if (card.WinningNumbers.Contains(cardNum))
                        if (points == 0)
                            points++;
                        else
                            points *= 2;

                totalPoints += points;
            }

            return totalPoints;
        }

        private static int GetTotalCountOfCopiedCards(IEnumerable<Card> cards)
        {
            var copiedCards = new List<Card>();

            for (var cardIdx = 0; cardIdx < cards.Count(); cardIdx++)
            {
                var card = cards.ElementAt(cardIdx);

                var existingCopiedCards = copiedCards.Where(c => c.Id == card.Id).ToList();

                foreach (var cardCopy in existingCopiedCards)
                {
                    var cardCopymatches = 0;

                    foreach (var cardNum in cardCopy.CardNumbers)
                    {
                        if (card.WinningNumbers.Contains(cardNum))
                        {
                            cardCopymatches++;
                            var cardToCopy = cards.ElementAtOrDefault(cardIdx + cardCopymatches);
                            if (cardToCopy != null)
                                copiedCards.Add(cardToCopy);
                        }
                    }
                }

                var matches = 0;
                foreach (var cardNum in card.CardNumbers)
                {
                    if (card.WinningNumbers.Contains(cardNum))
                    {
                        matches++;
                        var cardToCopy = cards.ElementAtOrDefault(cardIdx + matches);
                        if (cardToCopy != null)
                            copiedCards.Add(cardToCopy);
                    }
                }
            }
            return copiedCards.Count + cards.Count();
        }

        public static async Task<int> GetSumOfWinningScratchCards()
        {
            var cards = await ReadScratchCards();
            var winningPoints = GetWinningPoints(cards);
            var totalCountOfCards = GetTotalCountOfCopiedCards(cards);
            return totalCountOfCards;
        }
    }
}
