using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilePlacer : MonoBehaviour
{
    [SerializeField]
    GameObject tileObj;

    int numTiles = 1200;
    float scale = 30;
    Dictionary<Vector2, TileBlock> tiles;


    Vector2 GetWeightedRandomTile(HashSet<Vector2> coordCandidates, HashSet<Vector2> used)
    {
        Vector2 c1 = coordCandidates.ElementAt(Random.Range(0, coordCandidates.Count));
        Vector2 c2 = coordCandidates.ElementAt(Random.Range(0, coordCandidates.Count));

        int c1OpenSpace = 0;
        int c2OpenSpace = 0;

        foreach (Vector2 c1Neighbour in GetNeighbourCoords(c1))
        {
            c1OpenSpace += used.Contains(c1Neighbour) ? 1 : 0;
        }
        foreach (Vector2 c2Neighbour in GetNeighbourCoords(c2))
        {
            c2OpenSpace += used.Contains(c2Neighbour) ? 1 : 0;
        }

        return c1OpenSpace > c2OpenSpace ? c1 : c2;
    }

    void GenerateTiles()
    {

        tiles = new();
        // Base case add the starting tile
        GameObject rootTileObj = Instantiate(tileObj, Vector3.zero, Quaternion.identity, transform);
        TileBlock rootTile = rootTileObj.GetComponent<TileBlock>();
        rootTile.Initialise();
        rootTile.AssignSettings(AdjacencyLookup.RandomAdjacencySet());
        tiles[Vector2.zero] = rootTile;
        HashSet<Vector2> emptySlots = new HashSet<Vector2>(GetNeighbourCoords(Vector2.zero));
        HashSet<Vector2> usedSlots = new HashSet<Vector2> { Vector2.zero };

        while (tiles.Count < numTiles)
        {
            Vector2 newTileCoords = GetWeightedRandomTile(emptySlots, usedSlots);
            emptySlots.Remove(newTileCoords);
            Dictionary<int, AdjacencySettings> newSlotSettings = new Dictionary<int, AdjacencySettings>();
            for (int i = 0; i < 4; i++)
            {
                Vector2 neighbourCoords = newTileCoords + DirToVector2(i);
                if (usedSlots.Contains(neighbourCoords))
                {
                    newSlotSettings[i] = tiles[neighbourCoords].getAdjacency(AdjacencyLookup.ReverseDirection(i)).getSettings().GetRandomAllowedConnection();

                }
                else
                {
                    newSlotSettings[i] = AdjacencyLookup.RandomAdjacency();
                }
            }
            GameObject newTileObj = Instantiate(tileObj, new Vector3(newTileCoords.x, 0, newTileCoords.y) * scale, Quaternion.identity, transform);
            TileBlock newtile = newTileObj.GetComponent<TileBlock>();
            newtile.Initialise();
            newtile.AssignSettings(newSlotSettings);
            tiles[newTileCoords] = newtile;
            usedSlots.Add(newTileCoords);
            foreach (Vector2 potentialNewEmptySlot in GetNeighbourCoords(newTileCoords))
            {
                if (!usedSlots.Contains(potentialNewEmptySlot))
                {
                    emptySlots.Add(potentialNewEmptySlot);
                }
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        InitAdjacencySettings();
        GenerateTiles();
    }

    static void InitAdjacencySettings()
    {

        AdjacencySettings green = new AdjacencySettings(0, Color.green, new List<int> { 0, 2, 3 });
        AdjacencySettings red = new AdjacencySettings(1, Color.red, new List<int> { 1 });
        AdjacencySettings blue = new AdjacencySettings(2, Color.blue, new List<int> { 0, 3 });
        AdjacencySettings magenta = new AdjacencySettings(3, Color.magenta, new List<int> { 0, 2 });

        AdjacencyLookup.AdjSettings = new Dictionary<int, AdjacencySettings>
        {
            // Green connects to anything
            {0, green},
            // Red connects to itself
            {1, red},
            // Blue connects to Magenta and Green
            {2, blue},
            // Magenta connects to anything but itself
            {3, magenta}
        };
    }

    HashSet<AdjacencySettings> GetAvailableSettings(Vector2 pos, int dir)
    {
        TileBlock neighbour = tiles[pos + DirToVector2(dir)];
        if (neighbour is not null)
        {
            return neighbour.getAdjacency(AdjacencyLookup.ReverseDirection(dir)).getSettings().GetAllAllowedConnections();
        }
        else
        {
            return null;
        }
    }


    List<Vector2> GetNeighbourCoords(Vector2 centre)
    {
        return new List<Vector2> { centre + Vector2.right, centre + Vector2.left, centre + Vector2.up, centre + Vector2.down };
    }

    Vector2 DirToVector2(int dir)
    {
        switch (dir)
        {
            case 0:
                return new Vector2(1, 0);
            case 1:
                return new Vector2(-1, 0);
            case 2:
                return new Vector2(0, 1);
            case 3:
                return new Vector2(0, -1);
            default:
                Debug.LogError("DirToV2 received an invalid direction");
                return new Vector2(0, -1);
        }
    }

}
