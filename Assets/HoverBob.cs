using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverBob : MonoBehaviour
{

    float hoverAmp = 0.3f;
    float hoverFreq = 1f;
    float heightOffset = 1f;
    float offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = Random.Range(0, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, transform.parent.position + Vector3.up * (Mathf.Sin((Time.time + Random.Range(0, 1f)) * hoverFreq + offset) * hoverAmp + heightOffset), 0.5f * Time.deltaTime);
    }
}
