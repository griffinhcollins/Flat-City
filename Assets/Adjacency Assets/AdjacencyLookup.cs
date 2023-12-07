using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AdjacencyLookup
{

    public static Dictionary<int, Dictionary<int, AdjacencySettings>> AllTilePresets;
    public static Dictionary<int, AdjacencySettings> AllAdjSettings;
    public static Dictionary<int, AdjacencySettings> OpenAdjSettings;
    public static Dictionary<int, AdjacencySettings> WallAdjSettings;

    public static AdjacencySettings Lookup(int ID)
    {
        return AllAdjSettings[ID];
    }

    public static Dictionary<int, AdjacencySettings> TileSettingsLookup(int ID)
    {
        return AllTilePresets[ID];
    }

    public static AdjacencySettings RandomAdjacency()
    {
        return AllAdjSettings[Random.Range(0, AllAdjSettings.Count)];
    }

    public static AdjacencySettings RandomAdjacency(Dictionary<int, AdjacencySettings> set)
    {
        return set.ElementAt(Random.Range(0, set.Count)).Value;
    }

    public static Dictionary<int, AdjacencySettings> RandomCorridor(int direction)
    {
        if (direction <= 1)
        {
            return new Dictionary<int, AdjacencySettings> {
            { 0, RandomAdjacency() },
            { 1, RandomAdjacency() },
            { 2, RandomAdjacency(WallAdjSettings) },
            { 3, RandomAdjacency(WallAdjSettings) }
        };
        }
        else
        {
            return new Dictionary<int, AdjacencySettings> {
            { 0, RandomAdjacency(WallAdjSettings) },
            { 1, RandomAdjacency(WallAdjSettings) },
            { 2, RandomAdjacency() },
            { 3, RandomAdjacency() }
        };
        }
        
    }

    public static Dictionary<int, AdjacencySettings> RandomDeadEnd(int direction)
    {
        IEnumerable<int> wallDirs = new List<int> { 0, 1, 2, 3 }.Except<int>(new List<int> { direction });
        Dictionary<int, AdjacencySettings> wallDict = new();
        foreach (int wallDir in wallDirs)
        {
            wallDict.Add(wallDir, RandomAdjacency(WallAdjSettings));
        }
        wallDict.Add(direction, RandomAdjacency(OpenAdjSettings));
        return wallDict;
    }

    public static Dictionary<int, AdjacencySettings> RandomAdjacencySet()
    {
        return new Dictionary<int, AdjacencySettings> {
            { 0, RandomAdjacency() },
            { 1, RandomAdjacency() },
            { 2, RandomAdjacency() },
            { 3, RandomAdjacency() }
        };
    }

    public static Dictionary<int, AdjacencySettings> RandomSpawnTile()
    {
        return new Dictionary<int, AdjacencySettings> {
            { 0, RandomAdjacency(OpenAdjSettings) },
            { 1, RandomAdjacency(OpenAdjSettings) },
            { 2, RandomAdjacency(OpenAdjSettings) },
            { 3, RandomAdjacency(OpenAdjSettings) }
        };
    }

    public static int ReverseDirection(int dir)
    {
        return dir - 2 * (dir % 2) + 1;
    }

}
