using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsectBehaviour : MonoBehaviour
{

    [SerializeField]
    Transform FR_pole;
    [SerializeField]
    Transform MR_pole;
    [SerializeField]
    Transform BR_pole;
    [SerializeField]
    Transform FL_pole;
    [SerializeField]
    Transform ML_pole;
    [SerializeField]
    Transform BL_pole;


    [SerializeField]
    FastIKFabric FR_leg;
    [SerializeField]
    FastIKFabric MR_leg;
    [SerializeField]
    FastIKFabric BR_leg;
    [SerializeField]
    FastIKFabric FL_leg;
    [SerializeField]
    FastIKFabric ML_leg;
    [SerializeField]
    FastIKFabric BL_leg;


    Transform FR_target;
    Transform MR_target;
    Transform BR_target;
    Transform FL_target;
    Transform ML_target;
    Transform BL_target;

    // 0: FR, 1: MR, 2: BR, 3: FL, 4: ML, 5: BL
    Transform[] targets;
    Transform[] poles;
    FastIKFabric[] legs;


    // Start is called before the first frame update
    void Start()
    {
        legs = new FastIKFabric[6] { FR_leg, MR_leg, BR_leg, FL_leg, ML_leg, BL_leg };
        poles = new Transform[6] { FR_pole, MR_pole, BR_pole, FL_pole, ML_pole, BL_pole };
        CreateTargets();
    }

    void CreateTargets()
    {
        FR_target = new GameObject("FR_target").transform;
        MR_target = new GameObject("MR_target").transform;
        BR_target = new GameObject("BR_target").transform;
        FL_target = new GameObject("FL_target").transform;
        ML_target = new GameObject("ML_target").transform;
        BL_target = new GameObject("BL_target").transform;

        targets = new Transform[6] { FR_target, MR_target, BR_target, FL_target, ML_target, BL_target };

        for (int i = 0; i < 6; i++)
        {
            RaycastHit hitInfo;
            Debug.Log(Physics.Raycast(poles[i].position, Vector3.down, out hitInfo, 10, LayerMask.GetMask("Ground")));
            Debug.Log(hitInfo.point);
            targets[i].position = hitInfo.point;
            legs[i].target = targets[i]; 
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
