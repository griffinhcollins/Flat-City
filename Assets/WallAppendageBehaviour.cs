using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAppendageBehaviour : MonoBehaviour
{
    float maxVolume = 0.8f;
    float minVolume = 0.05f;
    FastIKFabric IK;
    AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        IK = GetComponentInChildren<FastIKFabric>();
        IK.target = References.player;
        audio = GetComponent<AudioSource>();
        audio.time = Random.Range(0, audio.clip.length);
    }

    // Update is called once per frame
    void Update()
    {
        float speed = Mathf.Clamp(IK.GetSpeed(),0,20);
        audio.volume = Mathf.Lerp(minVolume, maxVolume, speed / 20);
    }
}
