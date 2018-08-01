using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInitialiser : MonoBehaviour {

    private SelectedPlayer selectedPlayer;
    [SerializeField] private GameObject estoc;
    [SerializeField] private GameObject lilith;
    [SerializeField] private GameObject crag;
    [SerializeField] private AIManager aiManager;

	// Use this for initialization
	void Start ()
    {
        selectedPlayer = GameObject.FindGameObjectWithTag("Character Selector").GetComponent<SelectedPlayer>();

        if (selectedPlayer.estoc)
            SpawnEstoc();

        if (selectedPlayer.lilith)
            SpawnLilith();

        if (selectedPlayer.crag)
            SpawnCrag();
	}

    void SpawnEstoc ()
    {
        estoc.SetActive(true);
        lilith.SetActive(false);
        crag.SetActive(false);
        Camera.main.GetComponent<Camera_Follow>().target = estoc.transform;
    }

    void SpawnLilith ()
    {
        estoc.SetActive(false);
        lilith.SetActive(true);
        crag.SetActive(false);
        Camera.main.GetComponent<Camera_Follow>().target = lilith.transform;
    }

    void SpawnCrag()
    {
        estoc.SetActive(false);
        lilith.SetActive(false);
        crag.SetActive(true);
        Camera.main.GetComponent<Camera_Follow>().target = crag.transform;
    }
}
