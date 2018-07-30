using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_ColorChange : MonoBehaviour
{

    public Color color1;
    public Color color2;
    public float duration = 3.0F;

    public Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
    }

    void FixedUpdate()
    {
        
        float t = Mathf.PingPong(Time.time, duration + Random.RandomRange(2.0f, 3.0f)) / duration;
        cam.backgroundColor = Color.Lerp(color1, color2, t);
    }
}
