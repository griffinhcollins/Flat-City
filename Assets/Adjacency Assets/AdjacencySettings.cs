using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AdjacencySettings
{
    int ID;
    Color colour;
    HashSet<int> allowedIDs;
    HashSet<AdjacencySettings> allowedConnections;
    
    public AdjacencySettings(int ID_, Color colour_)
    {
        ID = ID_;
        colour = colour_;
    }

    public AdjacencySettings(int ID_, Color colour_, ICollection<int> allowedIDs_)
    {
        ID = ID_;
        colour = colour_;
        allowedIDs = new HashSet<int>(allowedIDs_);
        allowedConnections = new();
    }

    public void AddConnection(AdjacencySettings validNeighbour)
    {
        allowedIDs.Add(validNeighbour.getID());
        allowedConnections.Add(validNeighbour);
    }

    public void AddConnections(ICollection<AdjacencySettings> validNeighbours)
    {
        foreach (AdjacencySettings validNeighbour in validNeighbours)
        {
            AddConnection(validNeighbour);
        }
    }

    public bool CheckValidConnection(AdjacencySettings checkNeighbour)
    {
        allowedConnections = GetAllAllowedConnections();
        return allowedConnections.Contains(checkNeighbour);
    }

    public HashSet<AdjacencySettings> GetAllAllowedConnections()
    {
        foreach (int ID in allowedIDs)
        {
            allowedConnections.Add(AdjacencyLookup.Lookup(ID));
        };

        return allowedConnections;
    }

    public AdjacencySettings GetRandomAllowedConnection()
    {
        allowedConnections = GetAllAllowedConnections();
        if (allowedConnections.Count == 0)
        {
            return null;
        }
        return allowedConnections.ElementAt(Random.Range(0, allowedConnections.Count));
    }


    public Color getColour()
    {
        return colour;
    }

    public int getID()
    {
        return ID;
    }
    

}
