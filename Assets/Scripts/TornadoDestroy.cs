using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoDestroy : MonoBehaviour {

    public float waitTimer;
    [SerializeField] private GameObject tornadoBase;

    // Use this for initialization
    void Awake()
    {
        StartCoroutine(DestroySelf(waitTimer));
    }

    IEnumerator DestroySelf (float waitTimer)
    {
        yield return new WaitForSeconds(waitTimer);
        foreach (Transform child in tornadoBase.transform)
        {
            foreach (Transform enemy in child.transform)
            {
                if (enemy.gameObject.activeSelf == true)
                {
                    enemy.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    enemy.transform.parent = null;
                }
            }
        }
        Destroy(this.gameObject, 1);
    }
}
