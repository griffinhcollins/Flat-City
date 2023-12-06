using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilePlacer : MonoBehaviour
{
    [SerializeField]
    GameObject tileObj;

    [SerializeField]
    GameObject open1Obj;
    [SerializeField]
    GameObject openAdjObj;
    [SerializeField]
    GameObject openOppObj;
    [SerializeField]
    GameObject open3Obj;
    [SerializeField]
    GameObject open4Obj;


    int numTiles = 120;
    float scale = 10;
    Dictionary<Vector2, TileBlock> tiles;


    Vector2 GetWeightedRandomTile(HashSet<Vector2> coordCandidates, HashSet<Vector2> used)
    {
        // Finds two candidate tiles in the coordinate candidates and uses the currently existing tiles to pick one
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

        return c1OpenSpace < c2OpenSpace ? c1 : c2;
    }

    void GenerateTiles()
    {

        tiles = new Dictionary<Vector2, TileBlock>();
        // Base case add the starting tile
        GameObject rootTileObj = Instantiate(tileObj, Vector3.zero, Quaternion.identity, transform);
        rootTileObj.name = Vector2.zero.ToString();
        TileBlock rootTile = rootTileObj.GetComponent<TileBlock>();
        rootTile.Initialise();
        rootTile.AssignSettings(AdjacencyLookup.TileSettingsLookup(0)); // Tile 0 is always open on all sides with anything
        tiles[Vector2.zero] = rootTile;
        HashSet<Vector2> availableSlots = new HashSet<Vector2>(GetValidNeighbourCoords(Vector2.zero, rootTile));
        HashSet<Vector2> usedSlots = new HashSet<Vector2> { Vector2.zero };
        while (tiles.Count < numTiles && availableSlots.Count > 0)
        {
            Vector2 newTileCoords = GetWeightedRandomTile(availableSlots, usedSlots);
            availableSlots.Remove(newTileCoords);
            Dictionary<int, AdjacencySettings> newSlotSettings = new Dictionary<int, AdjacencySettings>();

            // Check if any of the neighbouring adjacencies forbid a tile here
            bool valid = true;
            HashSet<AdjacencySettings> existingNeighbourAdjacencies = new();
            List<bool> openings = new List<bool> { false, false, false, false }; // index direction, set true if there exists an open neighbour in that direction
            for (int i = 0; i < 4; i++)
            {
                Vector2 neighbourCoords = newTileCoords + DirToVector2(i);
                if (usedSlots.Contains(neighbourCoords))
                {
                    AdjacencySettings neighbourAdjacency = tiles[neighbourCoords].getAdjacency(AdjacencyLookup.ReverseDirection(i)).getSettings();
                    valid &= neighbourAdjacency.GetAllAllowedConnections().Count > 0;
                    if (!valid)
                    {
                        break;
                    }
                    else
                    {
                        openings[i] = true;
                        existingNeighbourAdjacencies.Add(neighbourAdjacency);
                    }
                }
            }
            if (!valid)
            {
                continue;
            }

            // Now that we know we're putting a tile down, we can decide what it will be
            if (existingNeighbourAdjacencies.Count == 1)
            {
                // Free reign from whatever our only neighbour's adjacency is
                newSlotSettings = existingNeighbourAdjacencies.First().GetRandomValidTileSettings();

            }
            else
            {
                // Check for corridors
                if (openings == new List<bool>{ true, true, false, false })
                {
                    newSlotSettings = AdjacencyLookup.TileSettingsLookup(2); // 2 is x-axis corridor
                }
                else if (openings == new List<bool> { false, false, true, true })
                {
                    newSlotSettings = AdjacencyLookup.TileSettingsLookup(3); // 2 is x-axis corridor
                }
                else
                {
                    // Just put down something random
                    newSlotSettings = AdjacencyLookup.RandomAdjacencySet();
                }
            }


            GameObject newTileObj = Instantiate(tileObj, new Vector3(newTileCoords.x, 0, newTileCoords.y) * scale, Quaternion.identity, transform);
            newTileObj.name = newTileCoords.ToString();
            TileBlock newtile = newTileObj.GetComponent<TileBlock>();
            newtile.Initialise();
            newtile.AssignSettings(newSlotSettings);
            tiles[newTileCoords] = newtile;
            usedSlots.Add(newTileCoords);
            foreach (Vector2 potentialNewEmptySlot in GetValidNeighbourCoords(newTileCoords, newtile))
            {
                if (!usedSlots.Contains(potentialNewEmptySlot))
                {
                    availableSlots.Add(potentialNewEmptySlot);
                }
            }
        }
        if (availableSlots.Count > 0)
        {
            Debug.Log("Max Size Reached");
        }
        else
        {
            Debug.Log("No more valid spots");
        }



        // Now set up correct gameobjs at all the locations
        foreach (Vector2 coord in tiles.Keys)
        {
            TileBlock tile = tiles[coord];
            List<Vector2> openNeighbours = NeighbourTiles(coord);

            switch (openNeighbours.Count)
            {
                case 0:
                    break;
                case 1:
                    Vector2 opendir = openNeighbours[0] - coord;
                    Vector3 openFaceDirection = new Vector3(opendir.x, 0, opendir.y);
                    GameObject tileDeadEnd = Instantiate(open1Obj, tile.transform.position + Vector3.up * 2, Quaternion.FromToRotation(Vector3.right, openFaceDirection), tile.transform);
                    break;
                case 2:
                    Vector2 open1 = openNeighbours[0] - coord;
                    Vector3 vec3open1 = new Vector3(open1.x, 0, open1.y);
                    Vector2 open2 = openNeighbours[1] - coord;
                    Vector3 vec3open2 = new Vector3(open2.x, 0, open2.y);
                   
                    bool adjacent = Vector2.Dot(open1, open2) == 0;
                    if (adjacent)
                    {
                        Vector3 rightFaceDir = Quaternion.AngleAxis(90, Vector3.up) * vec3open1 == vec3open2 ? vec3open2 : vec3open1;
                        GameObject tileCorner = Instantiate(openAdjObj, tile.transform.position + Vector3.up * 2, Quaternion.FromToRotation(Vector3.right, rightFaceDir), tile.transform);
                    }
                    else
                    {
                        // Corridor
                        
                        GameObject tileCorridor = Instantiate(openOppObj, tile.transform.position + Vector3.up * 2, Quaternion.FromToRotation(Vector3.right, vec3open1), tile.transform);

                    }
                    break;
                case 3:
                    // Find the missing direction
                    List<Vector2> allDirs = GetNeighbourCoords(coord);
                    Vector2 wallDirV2 = allDirs.Except(openNeighbours).First() - coord;
                    Vector3 wallDirection = new Vector3(wallDirV2.x, 0, wallDirV2.y);
                    GameObject tileWall = Instantiate(open3Obj, tile.transform.position + Vector3.up * 2, Quaternion.FromToRotation(Vector3.right, wallDirection), tile.transform);
                    break;
                case 4:
                    GameObject tileOpenSpace = Instantiate(open4Obj, tile.transform.position + Vector3.up * 2, Quaternion.identity, tile.transform);
                    break;
                default:
                    Debug.LogError("Failed to load tile with " + openNeighbours.Count + " open sides");
                    break;
            }
        }
    }

    List<Vector2> NeighbourTiles(Vector2 centre)
    {
        List<Vector2> neighbourCoords = GetNeighbourCoords(centre);
        List<Vector2> existingTiles = new List<Vector2>();
        foreach (Vector2 coord in neighbourCoords)
        {
            if (tiles.ContainsKey(coord))
            {
                existingTiles.Add(coord);
            }
        }
        return existingTiles;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitAdjacencySettings();
        GenerateTiles();
    }

    void ResetMaze()
    {
        foreach (TileBlock tile in tiles.Values)
        {
            GameObject.Destroy(tile.gameObject);
        }
        tiles = tiles = new Dictionary<Vector2, TileBlock>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            ResetMaze();
            GenerateTiles();
        }
    }

    static void InitAdjacencySettings()
    {

        

        AdjacencySettings green = new AdjacencySettings(0, Color.green, new List<int> { 1, 2, 3 });
        AdjacencySettings red = new AdjacencySettings(1, Color.red, new List<int> { 0, 1 });
        AdjacencySettings blue = new AdjacencySettings(2, Color.blue, new List<int> { 0 });
        AdjacencySettings magenta = new AdjacencySettings(3, Color.magenta, new List<int> { });

        AdjacencyLookup.AllAdjSettings = new Dictionary<int, AdjacencySettings>
        {
            // Green connects to random sets or corridors in either direction (if a perpendicular corridor is attempted, 
            {0, green},
            // Red connects to corridors
            {1, red},
            // Blue connects to dead ends
            {2, blue},
            // Magenta connects to nothing
            {3, magenta},
            // Magenta connects to nothing
            {4, magenta},
            // Magenta connects to nothing
            {5, magenta}
        };

        // Set up open and closed lookups
        AdjacencyLookup.OpenAdjSettings = new();
        AdjacencyLookup.WallAdjSettings = new();
        foreach (int adjID in AdjacencyLookup.AllAdjSettings.Keys)
        {
            AdjacencySettings value = AdjacencyLookup.AllAdjSettings[adjID];
            if (value.GetAllAllowedConnections().Count > 0)
            {
                // Open
                AdjacencyLookup.OpenAdjSettings.Add(adjID, value);
            }
            else
            {
                // Closed
                AdjacencyLookup.WallAdjSettings.Add(adjID, value);

            }
        }


        // Now set up the tile presets lookup
        AdjacencyLookup.AllTilePresets = new Dictionary<int, Dictionary<int, AdjacencySettings>>
        {
            {0, AdjacencyLookup.RandomSpawnTile()}, // Always open on all 4 sides
            {1, AdjacencyLookup.RandomAdjacencySet() }, // Completely random set
            {2, AdjacencyLookup.RandomCorridor(1)}, // Corridor in the x axis
            {3, AdjacencyLookup.RandomCorridor(2)} // Corridor in the z axis
        };



    }



    List<Vector2> GetValidNeighbourCoords(Vector2 centre, TileBlock tile)
    {
        List<Vector2> allNeighbours = GetNeighbourCoords(centre);
        List<Vector2> validNeighbours = new List<Vector2>();
        for (int i = 0; i < 4; i++)
        {
            if (tile.getAdjacency(i).hasConnections())
            {
                validNeighbours.Add(allNeighbours[i]);
            }
        }
        return validNeighbours;
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
