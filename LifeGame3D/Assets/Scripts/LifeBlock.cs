using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeBlock : MonoBehaviour {
    public GameObject Prefab;
    private Vector3 pos;
    private GameObject obj;
    private bool Put;
    void Start()
    {
        obj = (GameObject)Instantiate(Prefab, pos, new Quaternion());
        Put = false;
    }
    void Update()
    {
        if (Put)
        {
            
        }
    }
}
