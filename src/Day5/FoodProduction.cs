using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day5
{
    public static class FoodProduction
    {
        private record Almanac(
            IEnumerable<long> Seeds,
            IEnumerable<(long Start, long RangeLength)> SeedRanges,
            IEnumerable<Map> Maps
        );

        private record Map(string MapName, IEnumerable<MapRange> Ranges);

        private record MapRange(
            long DestinationRangeStart,
            long SourceRangeStart,
            long RangeLength
        );

        private static async Task<Almanac> ReadAlmanacFromFile()
        {
            var almanacLines = await File.ReadAllLinesAsync("./Day5/Almanac.txt");
            IEnumerable<long> seeds = new List<long>();

            var maps = new List<Map>();

            for (int lineIdx = 0; lineIdx < almanacLines.Length; lineIdx++)
            {
                var line = almanacLines[lineIdx];

                if (lineIdx == 0)
                    seeds = line.Split(": ")[1].Split(' ').Select(long.Parse);

                if (line.Contains("-to-"))
                {
                    var mapName = line.Replace(" map:", "");
                    lineIdx++;
                    line = almanacLines[lineIdx];
                    var mapRanges = new List<MapRange>();

                    while (line != "")
                    {
                        var ranges = line.Split(' ').Select(long.Parse).ToArray();
                        mapRanges.Add(new(ranges[0], ranges[1], ranges[2]));

                        lineIdx++;
                        if (lineIdx == almanacLines.Length)
                            line = "";
                        else
                            line = almanacLines[lineIdx];
                    }

                    maps.Add(new Map(mapName, mapRanges));
                }
            }

            var seedRanges = new List<(long, long)>();

            for (int seedIdx = 0; seedIdx < seeds.Count(); seedIdx++)
            {
                long start = 0;
                long length = 0;

                start = seeds.ElementAt(seedIdx);
                seedIdx++;
                length = seeds.ElementAt(seedIdx);
                seedRanges.Add(new(start, length));
            }

            return new Almanac(seeds, seedRanges, maps);
        }

        private static IEnumerable<long> GetMappedSeedToLocation(Almanac almanac)
        {
            var mappedVals = new List<(long Src, long Dest)>();
            var mappedLocations = new List<(long Src, long Dest)>();

            foreach (var seed in almanac.Seeds)
            {
                var mappedSeedToSoil = GetMappedDestination(seed, almanac, "seed-to-soil");

                var mappedSoilToFertilizer = GetMappedDestination(
                    mappedSeedToSoil.Dest,
                    almanac,
                    "soil-to-fertilizer"
                );

                var mappedFertilizerToWater = GetMappedDestination(
                    mappedSoilToFertilizer.Dest,
                    almanac,
                    "fertilizer-to-water"
                );

                var mappedWaterToLight = GetMappedDestination(
                    mappedFertilizerToWater.Dest,
                    almanac,
                    "water-to-light"
                );

                var mappedLightToTemperature = GetMappedDestination(
                    mappedWaterToLight.Dest,
                    almanac,
                    "light-to-temperature"
                );

                var mappedTemperatureToHumidity = GetMappedDestination(
                    mappedLightToTemperature.Dest,
                    almanac,
                    "temperature-to-humidity"
                );

                var mappedHumidityToLocation = GetMappedDestination(
                    mappedTemperatureToHumidity.Dest,
                    almanac,
                    "humidity-to-location"
                );

                mappedLocations.Add(mappedHumidityToLocation);
            }

            return mappedLocations.Select(l => l.Dest);
        }

        private static IEnumerable<long> GetMappedSeedRangesToLocation(Almanac almanac)
        {
            var mappedVals = new List<(long Src, long Dest)>();
            var mappedLocations = new List<MapRange>();

            var mappedSeedToSoil = GetMappedDestination(
                almanac.SeedRanges.ToList(),
                almanac,
                "seed-to-soil"
            );

            var mappedSoilToFertilizer = GetMappedDestination(
                mappedSeedToSoil.Select(m => (m.DestinationRangeStart, m.RangeLength)).ToList(),
                almanac,
                "soil-to-fertilizer"
            );

            var mappedFertilizerToWater = GetMappedDestination(
                mappedSoilToFertilizer
                    .Select(m => (m.DestinationRangeStart, m.RangeLength))
                    .ToList(),
                almanac,
                "fertilizer-to-water"
            );

            var mappedWaterToLight = GetMappedDestination(
                mappedFertilizerToWater
                    .Select(m => (m.DestinationRangeStart, m.RangeLength))
                    .ToList(),
                almanac,
                "water-to-light"
            );

            var mappedLightToTemperature = GetMappedDestination(
                mappedWaterToLight.Select(m => (m.DestinationRangeStart, m.RangeLength)).ToList(),
                almanac,
                "light-to-temperature"
            );

            var mappedTemperatureToHumidity = GetMappedDestination(
                mappedLightToTemperature
                    .Select(m => (m.DestinationRangeStart, m.RangeLength))
                    .ToList(),
                almanac,
                "temperature-to-humidity"
            );

            var mappedHumidityToLocation = GetMappedDestination(
                mappedTemperatureToHumidity
                    .Select(m => (m.DestinationRangeStart, m.RangeLength))
                    .ToList(),
                almanac,
                "humidity-to-location"
            );

            mappedLocations.AddRange(mappedHumidityToLocation);

            return mappedLocations
                .OrderBy(l => l.DestinationRangeStart)
                .Select(l => l.DestinationRangeStart);
        }

        private static Map GetMap(Almanac almanac, string mapName) =>
            almanac.Maps.Where(m => m.MapName == mapName).First();

        private static (long Src, long Dest) GetMappedDestination(
            long src,
            Almanac almanac,
            string mapName
        )
        {
            var map = GetMap(almanac, mapName);

            (long, long)? mappedSrc = null;

            foreach (var range in map.Ranges)
            {
                if (
                    src >= range.SourceRangeStart
                    && src <= (range.SourceRangeStart + range.RangeLength - 1)
                )
                {
                    for (var rangeIdx = 0; rangeIdx < range.RangeLength; rangeIdx++)
                    {
                        if (src == range.SourceRangeStart + rangeIdx)
                        {
                            if (mappedSrc != null)
                                throw new Exception("Match where value is already mapped");

                            mappedSrc = new(src, range.DestinationRangeStart + rangeIdx);
                        }
                    }
                }
            }

            if (mappedSrc == null)
                mappedSrc = new(src, src);

            return mappedSrc.Value;
        }

        private static List<MapRange> GetMappedDestination(
            List<(long Start, long Length)> srcRanges,
            Almanac almanac,
            string mapName
        )
        {
            var map = GetMap(almanac, mapName);

            var finalMappedSrcRanges = new List<MapRange>();

            foreach (var srcRange in srcRanges)
            {
                var mappedSrcRanges = new List<MapRange>();

                foreach (var range in map.Ranges)
                {
                    var mSrcStart = range.SourceRangeStart;
                    var iSrcStart = srcRange.Start;
                    var mDestStart = range.DestinationRangeStart;

                    var mSrcLimit = mSrcStart + range.RangeLength;
                    var iSrcLimit = iSrcStart + srcRange.Length;

                    if (mSrcStart == iSrcStart && iSrcLimit == mSrcLimit)
                    {
                        mappedSrcRanges.Add(new(mDestStart, iSrcStart, srcRange.Length));
                        break;
                    }
                    else if (mSrcStart == iSrcStart && iSrcLimit > mSrcLimit)
                    {
                        mappedSrcRanges.Add(new(mDestStart, iSrcStart, mSrcLimit - iSrcStart));
                        mappedSrcRanges.Add(new(mSrcLimit, mSrcLimit, iSrcLimit - mSrcLimit));
                        break;
                    }
                    else if (mSrcStart == iSrcStart && iSrcLimit < mSrcLimit)
                    {
                        mappedSrcRanges.Add(new(mDestStart, iSrcStart, iSrcLimit - iSrcStart));
                        break;
                    }
                    else if (mSrcStart < iSrcStart && iSrcLimit == mSrcLimit)
                    {
                        var offset = iSrcStart - mSrcStart;
                        mappedSrcRanges.Add(
                            new(mDestStart + offset, iSrcStart, iSrcLimit - iSrcStart)
                        );
                        mappedSrcRanges.Add(new(mSrcStart, mSrcStart, iSrcStart - mSrcStart));
                        break;
                    }
                    else if (mSrcStart > iSrcStart && iSrcLimit == mSrcLimit)
                    {
                        mappedSrcRanges.Add(new(mDestStart, mSrcStart, range.RangeLength));
                        mappedSrcRanges.Add(new(iSrcStart, iSrcStart, mSrcStart - iSrcStart));
                        break;
                    }
                    else if (mSrcStart < iSrcStart && iSrcLimit < mSrcLimit)
                    {
                        var offset = iSrcStart - mSrcStart;
                        mappedSrcRanges.Add(new(mDestStart + offset, iSrcStart, srcRange.Length));
                        break;
                    }
                    else if (
                        mSrcStart > iSrcStart
                        && mSrcStart > iSrcLimit
                        && iSrcLimit < mSrcLimit
                    )
                    {
                        mappedSrcRanges.Add(new(mDestStart, mSrcStart, iSrcLimit - mSrcStart));
                        mappedSrcRanges.Add(new(iSrcStart, iSrcStart, mSrcStart - iSrcStart));
                        break;
                    }
                    else if (
                        mSrcStart < iSrcStart
                        && iSrcStart > mSrcLimit
                        && iSrcLimit > mSrcLimit
                    )
                    {
                        var offset = iSrcStart - mSrcStart;

                        mappedSrcRanges.Add(
                            new(mDestStart + offset, iSrcStart, mSrcLimit - iSrcStart)
                        );
                        mappedSrcRanges.Add(new(mSrcLimit, mSrcLimit, iSrcLimit - mSrcLimit));
                        break;
                    }

                    if (mSrcStart <= iSrcStart && iSrcLimit <= mSrcLimit)
                    {
                        var diff = iSrcStart - mSrcStart;
                        mappedSrcRanges.Add(new(mDestStart + diff, iSrcStart, srcRange.Length));
                        break;
                    }
                    else if (
                        mSrcStart <= iSrcStart
                        && iSrcStart < mSrcLimit
                        && iSrcLimit >= mSrcLimit
                    )
                    {
                        var diff = iSrcStart - mSrcStart;
                        mappedSrcRanges.Add(
                            new(mDestStart + diff, iSrcStart, mSrcLimit - iSrcStart)
                        );
                        mappedSrcRanges.Add(new(mSrcLimit, mSrcLimit, iSrcLimit - mSrcLimit));
                        break;
                    }
                    else if (mSrcStart >= iSrcStart && iSrcLimit >= mSrcLimit)
                    {
                        mappedSrcRanges.Add(new(mDestStart, mSrcStart, range.RangeLength));
                        mappedSrcRanges.Add(new(iSrcStart, iSrcStart, mSrcStart - iSrcStart));
                        mappedSrcRanges.Add(new(mSrcLimit, mSrcLimit, iSrcLimit - mSrcLimit));
                        break;
                    }
                    else if (
                        mSrcStart >= iSrcStart
                        && mSrcStart < iSrcLimit
                        && iSrcLimit <= mSrcLimit
                    )
                    {
                        mappedSrcRanges.Add(new(mDestStart, mSrcStart, iSrcLimit - mSrcStart));
                        mappedSrcRanges.Add(new(iSrcStart, iSrcStart, mSrcStart - iSrcStart));
                        break;
                    }
                }

                if (mappedSrcRanges.Count == 0)
                    mappedSrcRanges.Add(new(srcRange.Start, srcRange.Start, srcRange.Length));

                finalMappedSrcRanges.AddRange(mappedSrcRanges);
            }

            //if (mappedSrc == null)
            //    mappedSrc = new(src, src);

            return finalMappedSrcRanges;
        }

        public static async Task<long> GetLowestLocationNumber()
        {
            var almanac = await ReadAlmanacFromFile();

            //var mappedSeeds = GetMappedSeedToLocation(almanac);
            var mappedSeeds = GetMappedSeedRangesToLocation(almanac);
            return mappedSeeds.Min();
        }
    }
}
