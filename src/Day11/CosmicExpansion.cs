using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    public static class CosmicExpansion
    {
        private record Galaxy(int Id, Point Coordinate);

        private static async Task<IEnumerable<Galaxy>> ReadUniverse()
        {
            var expandedUniverse = new List<string>();
            var expansionRate = 2;
            var universeLines = await File.ReadAllLinesAsync("./Day11/Universe.txt");

            var expansionRow = new string(
                Enumerable.Range(0, expansionRate).Select(i => '.').ToArray()
            );

            foreach (var universeLine in universeLines)
            {
                var expandedUniverseLine = "";
                var indexesForExpansion = new List<int>();

                for (int i = 0; i < universeLine.Length; i++)
                {
                    if (indexesForExpansion.Contains(i))
                    {
                        expandedUniverseLine += expansionRow;
                        break;
                    }

                    var expand = true;

                    foreach (var innerUniverseLine in universeLines)
                    {
                        if (innerUniverseLine[i] != '.')
                        {
                            expand = false;
                            break;
                        }
                    }

                    if (expand)
                    {
                        indexesForExpansion.Add(i);
                        expandedUniverseLine += expansionRow;
                    }
                    else
                    {
                        expandedUniverseLine += universeLine[i];
                    }
                }

                if (expandedUniverseLine.All(c => c == '.'))
                    expandedUniverse.AddRange(
                        Enumerable.Range(0, expansionRate).Select(i => expandedUniverseLine)
                    );
                else
                    expandedUniverse.Add(expandedUniverseLine);
            }

            var galaxies = new List<Galaxy>();

            for (int y = 0; y < expandedUniverse.Count; y++)
            {
                for (int x = 0; x < expandedUniverse.ElementAt(y).Length; x++)
                {
                    var symbol = expandedUniverse.ElementAt(y)[x];

                    if (symbol == '#')
                        galaxies.Add(new(galaxies.Count, new Point(x, y)));
                }
            }

            return galaxies;
        }

        private static IEnumerable<int> GetShortestPaths(IEnumerable<Galaxy> universe)
        {
            var shortestPaths = new List<int>();

            for (int galaxyIdx = 0; galaxyIdx < universe.Count(); galaxyIdx++)
            {
                var galaxy = universe.ElementAt(galaxyIdx);

                for (
                    int subGalaxyIdx = galaxyIdx + 1;
                    subGalaxyIdx < universe.Count();
                    subGalaxyIdx++
                )
                {
                    var subGalaxy = universe.ElementAt(subGalaxyIdx);
                    var shortestPath = GetShortestPathBetweenPoints(
                        subGalaxy.Coordinate,
                        galaxy.Coordinate
                    );

                    shortestPaths.Add(shortestPath);
                }
            }

            return shortestPaths;
        }

        private static int GetShortestPathBetweenPoints(Point dest, Point src) =>
            Math.Abs(dest.X - src.X) + Math.Abs(dest.Y - src.Y);

        public static async Task<int> GetSumOfShortestPathsBetweenGalaxies()
        {
            var universe = await ReadUniverse();
            var galaxy5 = universe.Where(g => g.Id == 7).First();
            var galaxy9 = universe.Where(g => g.Id == 8).First();
            var sda = GetShortestPathBetweenPoints(galaxy9.Coordinate, galaxy5.Coordinate);

            var shortestPaths = GetShortestPaths(universe);

            var shortestPathsSum = shortestPaths.Sum();
            return shortestPathsSum;
        }
    }
}
