using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WandererBehaviour : MonoBehaviour
{
    [SerializeField]
    EntityManagement manager;

    float startedPath;
    float currentPathTimeLimit;

    float maxPathTime = 30;
    float minPathTime = 10;

    NavMeshAgent agent;


    private void Awake()
    {
        manager.AddWanderer(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        startedPath = Time.time;
        currentPathTimeLimit = 10;
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(new Vector3(20, 2, -128));
    }


    public bool TryNewPath(Vector3 newDest)
    {
        if (newPathReady())
        {
            startedPath = Time.time;
            currentPathTimeLimit = Random.Range(minPathTime, maxPathTime);
            agent.SetDestination(newDest);
            return true;
        }
        else
        {
            return false;
        }
    }

    bool newPathReady()
    {
        return (Time.time - startedPath) > currentPathTimeLimit;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
