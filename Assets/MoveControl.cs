using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControl : MonoBehaviour
{

    Rigidbody rigidbody;
    float speed = 4f;

    [SerializeField]
    AudioSource walkFootsteps;
    [SerializeField]
    AudioSource runFootsteps;

    AudioSource curFootsteps;

    float walkVolume = 0.1f;


    private void Awake()
    {
        References.player = transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMove();
    }


    void StartFootsteps()
    {
        curFootsteps = Input.GetKey(KeyCode.LeftShift) ? runFootsteps : walkFootsteps;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            walkFootsteps.volume = 0;
            walkFootsteps.Stop();
        }
        else
        {
            runFootsteps.volume = 0;
            runFootsteps.Stop();
        }
        if (!curFootsteps.isPlaying)
        {
            curFootsteps.Play();
        }
        curFootsteps.volume = Mathf.Lerp(curFootsteps.volume, walkVolume, 0.8f);
    }

    void StopFootsteps()
    {
        runFootsteps.volume = 0;
        curFootsteps = walkFootsteps;
        curFootsteps.volume = Mathf.Lerp(curFootsteps.volume, 0, 0.8f);
    }


    void UpdateMove()
    {
        float forwards = Input.GetAxis("Vertical");
        float right = Input.GetAxis("Horizontal");
        Vector3 movedir = (forwards * transform.forward + right * transform.right);

        if (movedir.sqrMagnitude > 1)
        {
            movedir.Normalize();
        }

        if (movedir.sqrMagnitude == 0)
        {
            StopFootsteps();
        }
        else
        {
            StartFootsteps();
        }

        movedir *= speed * (Input.GetKey(KeyCode.LeftShift) ? 1.6f : 1);

        rigidbody.MovePosition(transform.position + movedir * Time.deltaTime);



    }
}
