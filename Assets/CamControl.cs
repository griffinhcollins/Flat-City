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

    float camZoomSpeed = 0.3f;
    float zoomedCamFOV = 30;
    float defaultCamFOV = 70;

    float bodyFollowTightness = 0.8f;
    float camHeight = 0.525f;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        FollowBody();
        UpdateRotation();
        UpdateZoom();

    }

    void FollowBody()
    {
        // doing it this way rather than parenting prevents stuttering
        Vector3 camTarget = playerBody.position + camHeight * Vector3.up;

        transform.position = Vector3.Lerp(transform.position, camTarget, bodyFollowTightness * 40 * Time.deltaTime);

    }

    private void FixedUpdate()
    {
    }

    void UpdateZoom()
    {
        if (Input.GetMouseButton(1))
        {
            playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, zoomedCamFOV, camZoomSpeed * 50 * Time.deltaTime);
        }
        else
        {
            playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, defaultCamFOV, camZoomSpeed * 50 * Time.deltaTime);

        }
    }

    void UpdateRotation()
    {

        Vector3 camRotate = new Vector3(-1*Input.GetAxisRaw("Mouse Y") * verticalSensitivity, 0, 0);
        Vector3 bodyRotate = new Vector3(0, Input.GetAxisRaw("Mouse X") * horizontalSensitivity, 0);

        playerCam.transform.Rotate(camRotate);
        transform.Rotate(bodyRotate);
        playerBody.Rotate(bodyRotate);

    }


}
