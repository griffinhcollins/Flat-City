using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{

    [SerializeField]
    Camera playerCam;
    [SerializeField]
    Transform playerBody;

    public float horizontalSensitivity;
    public float verticalSensitivity;



    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRotation();
    }



    void UpdateRotation()
    {

        Vector3 camRotate = new Vector3(-1*Input.GetAxisRaw("Mouse Y") * verticalSensitivity, 0, 0);
        Vector3 bodyRotate = new Vector3(0, Input.GetAxisRaw("Mouse X") * horizontalSensitivity, 0);

        transform.Rotate(camRotate);
        playerBody.Rotate(bodyRotate);

    }


}
