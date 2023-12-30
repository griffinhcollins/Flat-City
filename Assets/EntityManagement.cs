using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManagement : MonoBehaviour
{
    List<Transform> levelModules;
    List<WandererBehaviour> wanderers = new List<WandererBehaviour>();

    float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        levelModules = new List<Transform>();


        foreach (Transform module in transform)
        {
            levelModules.Add(module);
        }
    }

    public void AddWanderer(WandererBehaviour newWanderer)
    {
        wanderers.Add(newWanderer);
    }


    Transform GetRandomModule()
    {
        return levelModules[Random.Range(0, levelModules.Count)];
    }

    void UpdateWanderers()
    {
        if (wanderers.Count == 0) { return; }
        foreach (WandererBehaviour wanderer in wanderers)
        {
            wanderer.TryNewPath(GetRandomModule().position);
        }
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        if ((Time.time - startTime) > 5)
        {
            UpdateWanderers();
            startTime = Time.time;  
        }
    }
}
