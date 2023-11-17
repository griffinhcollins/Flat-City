using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class AdjacencySettings
{
    int ID;
    Color32 colour;
    HashSet<AdjacencySettings> allowedConnections;
    
    public AdjacencySettings(int ID_, Color32 colour_)
    {
        ID = ID_;
        colour = colour_;
    }

    public void AddConnection(AdjacencySettings validNeighbour)
    {
        allowedConnections.Add(validNeighbour);
    }

    public void AddConnections(Collection<AdjacencySettings> validNeighbours)
    {
        foreach (AdjacencySettings validNeighbour in validNeighbours)
        {
            allowedConnections.Add(validNeighbour);
        }
    }

    public bool CheckValidConnection(AdjacencySettings checkNeighbour)
    {
        return allowedConnections.Contains(checkNeighbour);
    }

    public Color32 getColour()
    {
        return colour;
    }

    public int getID()
    {
        return ID;
    }
    

}
