using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    [SerializeField] Transform CameraPos;

    // Update is called once per frame
    void Update()
    {
        transform.position = CameraPos.position;
    }
}
