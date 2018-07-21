using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCam : MonoBehaviour {

    public RawImage rawimage;

    void Start()
    {
        WebCamTexture webcamTexture = new WebCamTexture();
        rawimage = GetComponent<RawImage>();
        rawimage.texture = webcamTexture;
        rawimage.material.mainTexture = webcamTexture;
        webcamTexture.Play();
    }
}
