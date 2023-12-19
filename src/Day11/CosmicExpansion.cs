using System.Drawing;

namespace AdventOfCode2023
{
    public static class CosmicExpansion
    {
        private record Galaxy(int Id, Point Coordinate);

        private static async Task<IEnumerable<Galaxy>> ReadUniverse()
        {
            var expansionRate = 1000000;

            var universeLines = await File.ReadAllLinesAsync("./Day11/Universe.txt");

            var indexesForColumnExpansion = GetIndexesForColumnExpansion(universeLines);

            var indexesForRowExpansion = GetIndexesForRowExpansion(universeLines);

            var galaxies = new List<Galaxy>();

            for (int y = 0; y < universeLines.Length; y++)
            {
                for (int x = 0; x < universeLines.ElementAt(y).Length; x++)
                {
                    var symbol = universeLines.ElementAt(y)[x];

                    if (symbol == '#')
                    {
                        var offsetX = indexesForColumnExpansion
                            .Where(expX => expX < x)
                            .Sum(expX => expansionRate - 1);

                        var offsetY = indexesForRowExpansion
                            .Where(expY => expY < y)
                            .Sum(expY => expansionRate - 1);
                        galaxies.Add(new(galaxies.Count, new Point(x + offsetX, y + offsetY)));
                    }
                }
            }

            return galaxies;
        }

        private static IEnumerable<int> GetIndexesForColumnExpansion(string[] universeLines)
        {
            var firstGalaxy = universeLines[0];
            var indexesForExpansion = new List<int>();

            for (int galaxyIdx = 0; galaxyIdx < firstGalaxy.Length; galaxyIdx++)
            {
                var expand = true;

                foreach (var innerUniverseLine in universeLines)
                {
                    if (innerUniverseLine[galaxyIdx] != '.')
                    {
                        expand = false;
                        break;
                    }
                }

                if (expand)
                    indexesForExpansion.Add(galaxyIdx);
            }

            return indexesForExpansion;
        }

        private static IEnumerable<int> GetIndexesForRowExpansion(string[] universeLines)
        {
            var indexesForExpansion = new List<int>();

            for (int galaxyIdx = 0; galaxyIdx < universeLines.Length; galaxyIdx++)
            {
                var universeLine = universeLines[galaxyIdx];

                if (universeLine.All(u => u == '.'))
                    indexesForExpansion.Add(galaxyIdx);
            }

            return indexesForExpansion;
        }

        private static IEnumerable<long> GetShortestPaths(IEnumerable<Galaxy> universe)
        {
            var shortestPaths = new List<long>();

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

        public static async Task<long> GetSumOfShortestPathsBetweenGalaxies()
        {
            var universe = await ReadUniverse();

            var shortestPaths = GetShortestPaths(universe);

            long shortestPathsSum = shortestPaths.Sum();
            return shortestPathsSum;
        }
    }
}
