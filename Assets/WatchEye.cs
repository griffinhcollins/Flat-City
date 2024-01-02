using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.AI;

public class WatchEye : MonoBehaviour
{



    Transform playerCam;
    Transform playerBody;
    Transform target;

    bool canSeePlayer = false;
    float giveUpCount;
    float lastSawPlayer = -1;

    float lookSpeed = 0.5f;
    float randomLookTimer = 2;
    float randomLookDuration = 2;
    float randomLookMaxAngle = 30;
    Vector3 randomLookLocation;

    float lookRange = 10;
    float viewAngle = 90;

    // Whether this eye is in a socket
    public bool clamped;
    NavMeshAgent myAgent;

    Vector3 RestDir()
    {

        if (myAgent != null)
        {
            return myAgent.velocity;
        }
        else
        {
            // For wallappendage eye
            return Vector3.down;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        myAgent = transform.root.GetComponent<NavMeshAgent>();
        Light spotLight = GetComponentInChildren<Light>();
        if (spotLight is not null)
        {
            lookRange = spotLight.range;
        }
        playerCam = References.player.parent.GetComponentInChildren<Camera>().transform;
        playerBody = playerCam.root.GetComponentInChildren<MoveControl>().transform;
    }

    bool IsPlayerInRange()
    {
        return (playerCam.transform.position - transform.position).sqrMagnitude < lookRange * lookRange && (Mathf.Abs(Vector3.Angle(playerCam.transform.position - transform.position, transform.forward)) <= viewAngle/2);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerCheck();
        if (canSeePlayer)
        {
            LookAtTarget();

        }
        else
        {
            LookRandomly();
        }
    }

    void PlayerCheck()
    {
        if (IsPlayerInRange())
        {
            // The player is within the viewcone, but may be hidden behind a wall
            RaycastHit hitinfo;
            Transform pupil = transform.GetChild(0);
            Physics.Raycast(pupil.position, playerBody.position - pupil.position, out hitinfo, lookRange);
            if (hitinfo.collider.transform == playerBody)
            {
                SeenPlayer();
            }
            else
            {
                canSeePlayer = false;
            }
        }
    }


    void SeenPlayer()
    {
        canSeePlayer = true;
        lastSawPlayer = Time.time;
        target = playerCam;
        if (myAgent != null)
        {
            myAgent.SetDestination(playerBody.position);
        }
    }

    void LookAtTarget()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, GetClampedPlayerLookRot(), lookSpeed * Time.deltaTime * 10);

    }

    Quaternion GetClampedPlayerLookRot()
    {
        Vector3 lookVector = target.transform.position - transform.position;

        float angleOvershoot = randomLookMaxAngle - Vector3.Angle(target.transform.position - transform.position, RestDir());

        if (clamped && angleOvershoot > 0)
        {
            lookVector = Vector3.RotateTowards(lookVector, RestDir(), angleOvershoot, 0);
        }

        return Quaternion.LookRotation(lookVector);
    }


    void LookRandomly()
    {
        randomLookTimer += Time.deltaTime;
        if (randomLookTimer >= randomLookDuration)
        {
            randomLookTimer = 0;
            randomLookLocation = 3*RestDir() + 2 * new Vector3(Random.Range(-1,1f), Random.Range(-1, 1f), Random.Range(-1, 1f)) + transform.position;
        }
        Quaternion rot = Quaternion.LookRotation(randomLookLocation - transform.position);

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, lookSpeed * Time.deltaTime * 10);


    }

}
