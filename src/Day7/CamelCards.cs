using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    public static class CamelCards
    {
        private const int LengthOfHand = 5;

        private static readonly char[] Labels =
        [
            'A',
            'K',
            'Q',
            //'J',
            'T',
            '9',
            '8',
            '7',
            '6',
            '5',
            '4',
            '3',
            '2'
        ];

        public enum HandType
        {
            HighCard = 0,
            OnePair = 1,
            TwoPairs = 2,
            ThreeOfAKind = 3,
            FullHouse = 4,
            FourOfAKind = 5,
            FiveOfAKind = 6,
        }

        private static async Task<IEnumerable<Hand>> ReadHands()
        {
            var hands = await File.ReadAllLinesAsync("./Day7/Hands.txt");

            var handsAndBets = new List<Hand>();

            foreach (var hand in hands)
            {
                var splitHand = hand.Split(' ');

                if (int.TryParse(splitHand[1], out int parsedBet))
                    handsAndBets.Add(new() { Value = splitHand[0], Bet = parsedBet });
            }

            return handsAndBets;
        }

        private static IEnumerable<Hand> SetType(IEnumerable<Hand> hands)
        {
            foreach (var hand in hands)
            {
                foreach (var label in Labels)
                {
                    var count = hand.Value.Count(h => h == label);

                    if (hand.Type == HandType.OnePair)
                    {
                        hand.Type = count switch
                        {
                            3 => HandType.FullHouse,
                            2 => HandType.TwoPairs,
                            _ => hand.Type
                        };
                    }
                    else if (hand.Type == HandType.ThreeOfAKind)
                    {
                        hand.Type = count switch
                        {
                            2 => HandType.FullHouse,
                            _ => hand.Type
                        };
                    }
                    else if (count > ((int)hand.Type - 1))
                    {
                        hand.Type = count switch
                        {
                            5 => HandType.FiveOfAKind,
                            4 => HandType.FourOfAKind,
                            3 => HandType.ThreeOfAKind,
                            2 => HandType.OnePair,
                            _ => hand.Type
                        };
                    }
                }

                //Part Two

                var jokerCount = hand.Value.Count(h => h == 'J');

                if (jokerCount > 0)
                {
                    if (hand.Type == HandType.FourOfAKind)
                    {
                        hand.Type = HandType.FiveOfAKind;
                    }
                    else if (hand.Type == HandType.ThreeOfAKind)
                    {
                        hand.Type = jokerCount switch
                        {
                            2 => HandType.FiveOfAKind,
                            1 => HandType.FourOfAKind,
                            _ => hand.Type
                        };
                    }
                    else if (hand.Type == HandType.TwoPairs)
                    {
                        hand.Type = HandType.FullHouse;
                    }
                    else if (hand.Type == HandType.OnePair)
                    {
                        hand.Type = jokerCount switch
                        {
                            3 => HandType.FiveOfAKind,
                            2 => HandType.FourOfAKind,
                            1 => HandType.ThreeOfAKind,
                            _ => hand.Type
                        };
                    }
                    else if (hand.Type == HandType.HighCard)
                    {
                        hand.Type = jokerCount switch
                        {
                            5 => HandType.FiveOfAKind,
                            4 => HandType.FiveOfAKind,
                            3 => HandType.FourOfAKind,
                            2 => HandType.ThreeOfAKind,
                            1 => HandType.OnePair,
                            _ => hand.Type
                        };
                    }
                }
            }

            return hands;
        }

        private static int SumTotalHands(IEnumerable<Hand> hands)
        {
            var totalSum = 0;
            for (var i = 0; i < hands.Count(); i++)
            {
                var hand = hands.ElementAt(i);
                totalSum += hand.Bet * (i + 1);
            }

            return totalSum;
        }

        public static async Task<int> GetTotalWinnings()
        {
            var hands = await ReadHands();
            hands = SetType(hands);
            hands = hands.Order(new HandComparer());
            var totalWinnings = SumTotalHands(hands);
            return totalWinnings;
        }

        private static int LabelToValue(char label) =>
            label switch
            {
                '2' => 0,
                '3' => 1,
                '4' => 2,
                '5' => 3,
                '6' => 4,
                '7' => 5,
                '8' => 6,
                '9' => 7,
                'T' => 8,
                'J' => -1,
                'Q' => 10,
                'K' => 11,
                'A' => 12,
                _ => throw new ArgumentException("Invalid character")
            };

        internal class Hand
        {
            public string Value { get; set; } = null!;
            public int Bet { get; set; }
            public HandType Type { get; set; } = HandType.HighCard;
        }

        internal class HandComparer : IComparer<Hand>
        {
            public int Compare(Hand? x, Hand? y)
            {
                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                if (x.Type > y.Type)
                    return 1;
                else if (x.Type < y.Type)
                    return -1;
                else
                {
                    for (int i = 0; i < LengthOfHand; i++)
                    {
                        var xLabelVal = LabelToValue(x.Value[i]);
                        var yLabelVal = LabelToValue(y.Value[i]);

                        if (yLabelVal > xLabelVal)
                            return -1;
                        else if (yLabelVal < xLabelVal)
                            return 1;
                    }

                    return 0;
                }
            }
        }
    }
}
