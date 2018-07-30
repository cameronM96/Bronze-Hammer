using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedPlayer : MonoBehaviour {

    public bool estoc = true;
    public bool lilith;
    public bool crag;

    private static bool created = false;

	// Use this for initialization
	void Start ()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
	}

    public void EstocSelected ()
    {
        estoc = true;
        lilith = false;
        crag = false;
    }

    public void LilithSelected()
    {
        estoc = false;
        lilith = true;
        crag = false;
    }

    public void CragSelected()
    {
        estoc = false;
        lilith = false;
        crag = true;
    }
}
