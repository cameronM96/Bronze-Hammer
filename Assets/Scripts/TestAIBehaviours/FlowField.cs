using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField : MonoBehaviour {

    public bool enableFlowField;
    public Vector3[,] field;
    private GameObject[,] arrows;
    public int cols, rows;
    public int resolution;
    public GameObject arrowPrefab;
    private float startingValue = 0;
    private bool initialised = false;
    public bool updateOn;
    public float updateArrows = 0.2f;
    float timer = 0;

    private void Start()
    {
        cols = Mathf.RoundToInt(GetComponent<Renderer>().bounds.size.x) / resolution;
        rows = Mathf.RoundToInt(GetComponent<Renderer>().bounds.size.z) / resolution;
        field = new Vector3[cols, rows];
        arrows = new GameObject[cols, rows];
        Initialise(startingValue);
        initialised = true;
    }

    private void Update()
    {
        if (updateOn)
        {
            timer += Time.deltaTime;

            if (timer >= updateArrows)
            {
                startingValue += 0.1f;
                Initialise(startingValue);
                timer = 0;
            }
        }
    }

    void Initialise (float startvalue)
    {
        float xoff = startvalue;
        int xpos = 0;
        for (int i = 0; i < cols; i++)
        {
            float zoff = startvalue;
            int zpos = 0;
            for (int j = 0; j < rows; j++)
            {
                // Set a Perlin Noise vector3
                float theta = Map(Mathf.PerlinNoise(xoff, zoff), 0, 1, 0, Mathf.PI * 2);
                field[i, j] = new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta));

                // Instantiate a sprite
                GameObject arrow;
                if (!initialised)
                {
                    arrow = Instantiate(arrowPrefab);
                    arrow.transform.parent = this.transform;
                }
                else
                {
                    arrow = arrows[i, j];
                }


                int column = (xpos * resolution) - 100 + (resolution / 2);
                int row = (zpos * resolution) - 75 + (resolution / 2);
                arrow.transform.position = new Vector3(column, 0, row);
                arrow.transform.rotation = Quaternion.LookRotation(field[i,j],Vector3.up);
                arrows[i, j] = arrow;
                //arrow.transform.parent = this.transform;

                zoff += 0.1f;
                zpos++;
            }
            xoff += 0.1f;
            xpos++;
        }
    }

    public Vector3 LookUp(Vector3 lookup)
    {
        int column = Mathf.RoundToInt(Mathf.Clamp((lookup.x + 100) / resolution, 0, cols - 1));
        int row = Mathf.RoundToInt(Mathf.Clamp((lookup.z + 75) / resolution, 0, rows - 1));
        return field[column, row];
    }

    static float Map(float value, float min, float max, float minSpeed, float MaxSpeed)
    {
        return (value - min) * (MaxSpeed - minSpeed) / (max - min) + minSpeed;
    }
}
