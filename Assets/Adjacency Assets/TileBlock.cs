using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBlock : MonoBehaviour
{
    [SerializeField]
    Adjacency posX;
    [SerializeField]
    Adjacency negX;
    [SerializeField]
    Adjacency posZ;
    [SerializeField]
    Adjacency negZ;

    Dictionary<int, Adjacency> adjacencies;

    public void Initialise()
    {
        adjacencies = new Dictionary<int, Adjacency> { { 0, posX }, { 1, negX }, { 2, posZ }, { 3, negZ } };
    }

    public void AssignSettings(Dictionary<int, AdjacencySettings> orderedSettings)
    {
        for (int i = 0; i < 4; i++)
        {
            adjacencies[i].AssignSettings(orderedSettings[i]);
        }
    }


    public Adjacency getAdjacency(int direction)
    {
        return adjacencies[direction];
    }



}
