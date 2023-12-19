using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    public static class PipeMaze
    {
        private record Tile(char Symbol, Point Coordinate);

        private static async Task<IEnumerable<Tile>> ReadPipeDiagram()
        {
            var pipeLines = await File.ReadAllLinesAsync("./Day10/PipeDiagram.txt");

            var tiles = new List<Tile>();
            for (int y = 0; y < pipeLines.Length; y++)
            {
                for (int x = 0; x < pipeLines[y].Length; x++)
                {
                    tiles.Add(new Tile(pipeLines[y][x], new Point(x, y)));
                }
            }

            return tiles;
        }

        private static int GetSteps(IEnumerable<Tile> tiles)
        {
            var startTile = tiles.Where(t => t.Symbol == 'S').Single();

            var currentTile = startTile;

            var tilesInLoop = new HashSet<Tile>();
            var steps = 0;

            Tile previousTile = currentTile;

            do
            {
                if (currentTile != null)
                    tilesInLoop.Add(currentTile);

                Tile? nextTile = null;
                if (currentTile.Symbol == 'S')
                {
                    if (nextTile == null)
                        nextTile = GetValidWestTile(tiles, currentTile.Coordinate);

                    if (nextTile == null)
                        nextTile = GetValidNorthTile(tiles, currentTile.Coordinate);

                    if (nextTile == null)
                        nextTile = GetValidEastTile(tiles, currentTile.Coordinate);

                    if (nextTile == null)
                        nextTile = GetValidSouthTile(tiles, currentTile.Coordinate);
                }

                if (((char[])['7', 'J', '-']).Contains(currentTile.Symbol) && nextTile == null)
                {
                    var leftTile = GetValidWestTile(tiles, currentTile.Coordinate);
                    if (leftTile != previousTile)
                        nextTile = leftTile;
                }

                if (((char[])['L', 'F', '-']).Contains(currentTile.Symbol) && nextTile == null)
                {
                    var leftTile = GetValidEastTile(tiles, currentTile.Coordinate);
                    if (leftTile != previousTile)
                        nextTile = leftTile;
                }

                if (((char[])['7', 'F', '|']).Contains(currentTile.Symbol) && nextTile == null)
                {
                    var leftTile = GetValidSouthTile(tiles, currentTile.Coordinate);
                    if (leftTile != previousTile)
                        nextTile = leftTile;
                }

                if (((char[])['L', 'J', '|']).Contains(currentTile.Symbol) && nextTile == null)
                {
                    var leftTile = GetValidNorthTile(tiles, currentTile.Coordinate);
                    if (leftTile != previousTile)
                        nextTile = leftTile;
                }

                previousTile = currentTile;
                currentTile = nextTile;
                steps++;
            } while (currentTile != startTile);

            var points = tilesInLoop.Select(t => t.Coordinate).ToList();
            points.Add(tilesInLoop.First().Coordinate);

            var area = Math.Abs(
                points
                    .Take(points.Count - 1)
                    .Select((p, i) => (points[i + 1].X - p.X) * (points[i + 1].Y + p.Y))
                    .Sum() / 2
            );

            //After finding area, using Pick's theorem to get interior integer points (enclosed tiles)
            var enclosedTiles = area + 1 - (tilesInLoop.Count / 2);
            return enclosedTiles;
        }

        private static Tile? GetValidEastTile(IEnumerable<Tile> tiles, Point currCoord) =>
            EvalNeighborPipeTile(tiles, currCoord, ['-', 'J', '7', 'S'], 1);

        private static Tile? GetValidWestTile(IEnumerable<Tile> tiles, Point currCoord) =>
            EvalNeighborPipeTile(tiles, currCoord, ['-', 'F', 'L', 'S'], -1);

        private static Tile? GetValidNorthTile(IEnumerable<Tile> tiles, Point currCoord) =>
            EvalNeighborPipeTile(tiles, currCoord, ['|', 'F', '7', 'S'], 0, -1);

        private static Tile? GetValidSouthTile(IEnumerable<Tile> tiles, Point currCoord) =>
            EvalNeighborPipeTile(tiles, currCoord, ['|', 'L', 'J', 'S'], 0, 1);

        private static Tile? EvalNeighborPipeTile(
            IEnumerable<Tile> tiles,
            Point currCoord,
            char[] allowedNeighbors,
            int offsetX = 0,
            int offsetY = 0
        )
        {
            var neighborTile = tiles
                .Where(
                    t =>
                        t.Coordinate.X == currCoord.X + offsetX
                        && t.Coordinate.Y == currCoord.Y + offsetY
                )
                .FirstOrDefault();

            if (neighborTile != null && allowedNeighbors.Contains(neighborTile.Symbol))
                return neighborTile;

            return null;
        }

        public static async Task<double> GetStepsFarthestFromStarting()
        {
            var pipeTiles = await ReadPipeDiagram();
            var steps = GetSteps(pipeTiles);
            var count = pipeTiles.Where(t => t.Symbol != '.').Count() - steps;
            var farthestPointAway = Math.Ceiling((double)steps / 2);
            return steps;
        }
    }
}
