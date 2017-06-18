using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cell
{
    public class CellPrefab : MonoBehaviour
    {
        public GameObject cell;
        public Vector3[] Positions;
        // Use this for initialization
        void Awake()
        {
            foreach (var pos in Positions)
            {
                var prefab=(GameObject)Instantiate(cell, pos, new Quaternion());
                var cel = prefab.GetComponent<Cell>();
                cel.SetPos(pos);
            }
            Destroy(this);
        }
    }
}
