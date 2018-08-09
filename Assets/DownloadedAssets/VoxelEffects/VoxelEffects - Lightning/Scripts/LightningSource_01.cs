using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningSource_01 : MonoBehaviour {

    public GameObject prefabLightning;
    public GameObject lightSource;
    public float fadeSpeed = 1.0F;
    public float randomTimeRange;

    private float amplitude;
    private float fadeMax;
    private float fadeMin;


    public Light lightPoint;


    public float spawnTime = 3f;

    private bool lightningOn;

    void Awake()
    {
        amplitude = lightPoint.intensity;
        fadeMax = amplitude;
        fadeMin = 0;
        lightPoint.intensity = fadeMin;

        spawnTime = spawnTime + Random.Range(0, randomTimeRange);
    }

    void Start()
    {
        
        InvokeRepeating("LightningPrefab", spawnTime, spawnTime);
    }

    void FixedUpdate()
    {
        if (lightningOn == true)
        {
        lightPoint.intensity = Mathf.Lerp(lightPoint.intensity, fadeMax + Random.Range(0, fadeMax), fadeSpeed * Time.deltaTime); // Fade UP
        }

        if (lightningOn == false)
        {
        lightPoint.intensity = Mathf.Lerp(lightPoint.intensity, fadeMin, fadeSpeed * 2*Time.deltaTime);  // Fade Down
        }

    }

    void LightningPrefab()
    {

        var newLightning = GameObject.Instantiate(prefabLightning, transform.position, transform.rotation);
        lightningOn = true;

        Invoke("LightningOff", prefabLightning.GetComponent<ParticleSystem>().duration -0.5f);  

        Destroy(newLightning, prefabLightning.GetComponent<ParticleSystem>().duration); // Destroy Lightning prefab

    }

    void LightningOff()
    {
        lightningOn = false;

    }

}
