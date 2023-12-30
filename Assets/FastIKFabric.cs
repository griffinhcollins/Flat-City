using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FastIKFabric : MonoBehaviour
{

    int chainLength = 4;

    public Transform target;
    public Transform pole;


    int maxIterations = 10;

    float distanceThreshold = 0.001f;

    float snapbackStrength = 1f;

    float snapSpeed = 0.3f;

    float[] boneLengths;
    float totalLength;
    Transform[] bones;
    Vector3[] positions;
    Vector3[] startSuccDirections;
    Quaternion[] startBoneRotations;
    Quaternion startTargetRotation;
    Quaternion startRootRotation;


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        ResolveIK();
    }

    private void ResolveIK()
    {
        if (target == null)
        {
            //Debug.Log("no target");
            return;
        }


        if (boneLengths.Length != chainLength)
        {
            Init();
        }

        

        positions = GetBonePositions();

        // The base has a set rotation, either identity or set by its parent
        // Look at this later, seems a bit sus to me
        Quaternion rootRotation = bones[0].parent != null ? bones[0].parent.rotation : Quaternion.identity;
        Quaternion rootRotDiff = rootRotation * Quaternion.Inverse(startRootRotation);

        int posLength = positions.Length;
        //bones[0] is the location of the base of the arm
        if ((target.position - positions[0]).sqrMagnitude >= totalLength * totalLength)
        {
            // The target is out of reach, just point directly at it
            Vector3 direction = (target.position - positions[0]).normalized;
            // Start at 1 because we never move the base of the arm
            for (int i = 1; i < posLength; i++)
            {
                positions[i] = positions[i - 1] + direction * boneLengths[i - 1];
            }
        }
        else
        {
            // The target is within reach, we will use the fabric algorithm
            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                // Are we within our threshold of distance?
                if ((positions[posLength - 1] - target.position).sqrMagnitude <= distanceThreshold * distanceThreshold)
                {
                    break;
                }

                // Back goes first, starting at the last bone (which is the end of the arm)
                // For the last bone, set it directly
                positions[posLength - 1] = target.position;
                // For the rest, fabric!
                // Note the i > 0 below. This means that we don't touch the root bone (the base of the arm)
                for (int i = posLength - 2; i > 0; i--)
                {
                    positions[i] = positions[i + 1] + boneLengths[i] * (positions[i] - positions[i + 1]).normalized;
                }

                // Now forward, like backward but forward instead
                // For the first bone, set it directly (as first bone is the root of the arm, cannot move)
                // Except we never touched the root bone above, so it's already where it should be
                // For the rest, fabric!
                for (int i = 1; i < posLength; i++)
                {
                    positions[i] = positions[i - 1] + boneLengths[i-1] * (positions[i] - positions[i - 1]).normalized;
                }
            }
        }

        // Rotate towards pole
        if (pole != null)
        {
            // 1 to poslength - 2: not moving root or tip of arm
            for (int i = 1; i < posLength - 1; i++)
            {
                // Create a plane perpendicular to the connecting line of parent and child, intersecting parent
                Plane plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);
                Vector3 projectedPole = plane.ClosestPointOnPlane(pole.position);
                Vector3 projectedBone = plane.ClosestPointOnPlane(positions[i]);
                // How many degrees do we rotate this point around the axis of its child and parent to be closest to pole
                float angle = Vector3.SignedAngle(projectedBone - positions[i - 1], projectedPole - positions[i - 1], plane.normal);
                // Rotate around the vector pointing to this point from the last point (which is on the plane) and apply it
                positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i - 1]) + positions[i - 1];
            }
        }


        // Set rotations for all except tip
        for (int i = 0; i < posLength - 1; i++)
        {
            bones[i].rotation = Quaternion.Lerp(bones[i].rotation,(Quaternion.FromToRotation(startSuccDirections[i], positions[i + 1] - positions[i]) * startBoneRotations[i]),snapSpeed);
        }
        // set tip
        bones[posLength - 1].rotation = target.rotation * Quaternion.Inverse(startTargetRotation) * startBoneRotations[posLength - 1];

        SetBonePositions(positions);

    }

    Vector3[] GetBonePositions()
    {
        Vector3[] bonePos = new Vector3[bones.Length];
        for (int i = 0; i < bones.Length; i++)
        {
            bonePos[i] = bones[i].position;

        }

        return bonePos;
    }


    void SetBonePositions(Vector3[] posList)
    {
        for (int i = 0; i < bones.Length; i++)
        {
            bones[i].position = Vector3.Lerp(bones[i].position, posList[i], snapSpeed);
        }
    }

    public void Init()
    {
        totalLength = 0;
        Transform current = transform;
        bones = new Transform[chainLength + 1];
        positions = new Vector3[chainLength + 1];
        boneLengths = new float[chainLength]; // one smaller because the last bone is always length 0, it's just the end
        startSuccDirections = new Vector3[chainLength + 1];
        startBoneRotations = new Quaternion[chainLength + 1];
        Debug.Log(target);
        startTargetRotation = target.rotation;

        bool leaf = true;
        for (int i = bones.Length - 1; i >= 0; i--)
        {
            bones[i] = current;
            positions[i] = current.position;
            startBoneRotations[i] = current.rotation;
            if (leaf)
            {
                leaf = false;
                startSuccDirections[i] = target.position - positions[i]; // Last bone points directly at target
            }
            else
            {
                startSuccDirections[i] = positions[i + 1] - positions[i];
                boneLengths[i] = (positions[i] - positions[i + 1]).magnitude;
                totalLength += boneLengths[i];
            }
            current = current.parent;
        
        }

        startRootRotation = bones[0].rotation;


    }


    //private void OnDrawGizmos()
    //{
    //    Transform current = transform;
    //    for (int i = 0; i < chainLength && current != null && current.parent != null; i++)
    //    {

    //        float scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
    //        Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
    //        Handles.color = Color.green;
    //        Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
    //        current = current.parent;
    //    }
    //}

}
