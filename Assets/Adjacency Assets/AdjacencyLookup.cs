using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AdjacencyLookup
{

    public static Dictionary<int, AdjacencySettings> AdjSettings;

    public static AdjacencySettings Lookup(int ID)
    {
        return AdjSettings[ID];
    }

    public static AdjacencySettings RandomAdjacency()
    {
        return AdjSettings[Random.Range(0, AdjSettings.Count)];
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

    public static int ReverseDirection(int dir)
    {
        return dir - 2 * (dir % 2) + 1;
    }

}
