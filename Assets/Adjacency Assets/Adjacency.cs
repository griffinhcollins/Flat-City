using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adjacency : MonoBehaviour
{
    // Each tileblock has four adjacencies: one for each orthogonal direction
    // The adjacency has associated settings which determine what neighbours are allowed
    AdjacencySettings settings;
    // The adjacency also stores the neighbouring tile in its corresponding direction
    TileBlock neighbour;
    // For ease, also store the direction this adjacency points in
    // 0 is posX, 1 is negX, 2 is posZ, 3 is negZ
    int direction;

    public void Initialise(int direction_, AdjacencySettings settings_)
    {
        direction = direction_;
        AssignSettings(settings_);
    }

    public void AssignSettings(AdjacencySettings settings_)
    {
        settings = settings_;
        UpdateColour();
    }

    public AdjacencySettings getSettings()
    {
        return settings;
    }

    public void AssignNeighbour(TileBlock neighbour_)
    {
        neighbour = neighbour_;
    }

    public TileBlock getNeighbour()
    {
        return neighbour;
    }

    public int getDirection()
    {
        return direction;
    }

    private void UpdateColour()
    {
        gameObject.GetComponent<Renderer>().material.color = settings.getColour();

    }

}
