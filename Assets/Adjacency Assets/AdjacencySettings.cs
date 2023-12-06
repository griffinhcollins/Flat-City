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
    }


    public void AddConnection(int ID)
    {
        allowedIDs.Add(ID);
    }

    public void AddConnections(ICollection<int> validNeighbourIDs)
    {
        foreach (int ID in validNeighbourIDs)
        {
            AddConnection(ID);
        }
    }

    public bool CheckValidConnection(int ID)
    {
        allowedIDs = GetAllAllowedConnections();
        return allowedIDs.Contains(ID);
    }

    public HashSet<int> GetAllAllowedConnections()
    {
        foreach (int ID in allowedIDs)
        {
            AddConnection(ID);
        }
        return allowedIDs;
    }

    public Dictionary<int, AdjacencySettings> GetRandomValidTileSettings()
    {
        allowedIDs = GetAllAllowedConnections();
        if (allowedIDs.Count == 0)
        {
            return null;
        }
        int ID_used = allowedIDs.ElementAt(Random.Range(0, allowedIDs.Count));
        return AdjacencyLookup.TileSettingsLookup(ID_used);
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
