using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControl : MonoBehaviour
{

    Rigidbody rigidbody;
    float speed = 4;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMove();
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

        movedir *= Time.deltaTime * speed * (Input.GetKey(KeyCode.LeftShift) ? 1.3f : 1);

        transform.position += movedir;


    }
}
