using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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
    bool[] stepping;
    float[] startedStepTimes;
    float[] stepLengthTimes;
    Vector3[] lastStablePositions;
    Vector3[] nextStepPositions;

    LinkedList<int> legsToMove;

    float forwardOffset = 1f;

    float legDist = 0.5f;
    float sinkInDepth = 0.0f;
    float minStepLength = 0.8f;
    float maxStepLength = 0.9f;
    float stepThreshold = 1f;

    float minStepTime = 0.14f;
    float maxStepTime = 0.2f;

    int counter;

    NavMeshAgent navAgent;


    Rigidbody insectBody;
    // Start is called before the first frame update
    void Start()
    {
        navAgent = transform.parent.GetComponent<NavMeshAgent>();
        insectBody = GetComponent<Rigidbody>();
        legsToMove = new LinkedList<int>();
        counter = 0;
        lastStablePositions = new Vector3[6];
        nextStepPositions = new Vector3[6];
        legs = new FastIKFabric[6] { FR_leg, MR_leg, BR_leg, FL_leg, ML_leg, BL_leg };
        poles = new Transform[6] { FR_pole, MR_pole, BR_pole, FL_pole, ML_pole, BL_pole };
        stepping = new bool[6] { false, false, false, false, false, false };
        startedStepTimes = new float[6];
        stepLengthTimes = new float[6];
        CreateTargets();
        SetPoles();
        InitLegs();

    }


    void IterateSteps()
    {
        for (int i = 0; i < legs.Length; i++)
        {
            if (stepping[i])
            {
                float current = Time.time;
                float stepFractionCompleted = (current - startedStepTimes[i]) / stepLengthTimes[i];
                targets[i].position = Vector3.Lerp(lastStablePositions[i], nextStepPositions[i], stepFractionCompleted) + Vector3.up * stepFractionCompleted*(1 - stepFractionCompleted);
                if (stepFractionCompleted >= 1)
                {
                    legsToMove.Remove(i);
                    stepping[i] = false;
                    lastStablePositions[i] = targets[i].position;
                }
            }
        }
    }


    int NumSteppingCurrently()
    {
        int sum = 0;
        foreach (bool stepCheck in stepping)
        {
            if (stepCheck)
            {
                sum++;
            }
        }
        return sum;
    }

    void Step(int legIndex)
    {
        if (stepping[legIndex] || NumSteppingCurrently() > 0)
        {
            return;
        }
        stepping[legIndex] = true;
        startedStepTimes[legIndex] = Time.time;
        // Find the distance between the previous foot down location and the rest location

        Vector3 displacement = LegDisplacement(legIndex);
        stepLengthTimes[legIndex] = Random.Range(minStepTime, maxStepTime);
        if (displacement.sqrMagnitude > maxStepLength * maxStepLength)
        {
            displacement = displacement.normalized;
        }
        // Find the point above where the next footstep should go
        Vector3 legPos = (poles[legIndex].position - transform.position) * legDist + transform.position;
        Vector3 newStepRayStart = legPos + displacement;
        RaycastHit hitInfo;
        Physics.Raycast(newStepRayStart, Vector3.down, out hitInfo, 10, LayerMask.GetMask("Ground"));

        nextStepPositions[legIndex] = hitInfo.point + sinkInDepth * Vector3.down;
            //+ forwardOffset/20 * transform.up * navAgent.velocity.sqrMagnitude;

    }

    Vector3 LegDisplacement(int legIndex)
    {
        return (CalculateLegRestPos(legIndex) - lastStablePositions[legIndex]);
    }


    void InitLegs()
    {

        for (int i = 0; i < legs.Length; i++)
        {
            legs[i].Init();
        }
    }

    void SetPoles()
    {
        for (int i = 0; i < legs.Length; i++)
        {
            legs[i].pole = poles[i];
        }
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

        LegsToRestPos();
    }

    void LegsToRestPos()
    {
        for (int i = 0; i < legs.Length; i++)
        {
            targets[i].position = CalculateLegRestPos(i);
            legs[i].target = targets[i];
            Debug.Log(targets[i]);
            lastStablePositions[i] = targets[i].position;

            Debug.Log("Leg " + i.ToString() + ": " + legs[i].target.ToString());
        }
    }

    Vector3 CalculateLegRestPos(int legIndex)
    {
        Vector3 legPos = (poles[legIndex].position - transform.position) * legDist;
        Vector3 legTarget = Vector3.ProjectOnPlane(legPos, Vector3.up);
        Vector3 rayAngle = legTarget * 1.2f - legPos;
        RaycastHit hitInfo;
        Physics.Raycast(legPos + transform.position, rayAngle, out hitInfo, 10, LayerMask.GetMask("Ground"));
        return hitInfo.point + sinkInDepth * rayAngle + forwardOffset * navAgent.velocity;
    }

    private void FixedUpdate()
    {
        //insectBody.velocity = transform.up * Input.GetAxis("Vertical") * 3;
        //insectBody.MoveRotation(transform.rotation * Quaternion.AngleAxis(Input.GetAxis("Horizontal") ,Vector3.forward));
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 6; i++)
        {
            if ((!legsToMove.Contains(i)) && LegDisplacement(i).sqrMagnitude > stepThreshold * stepThreshold)
            {
                legsToMove.AddLast(i);

            }
        }

        if (NumSteppingCurrently() <= 6 && legsToMove.Count > 0)
        {
            Step(legsToMove.First.Value);
        }


        IterateSteps();
    }

}
