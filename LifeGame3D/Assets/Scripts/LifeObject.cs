using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LifeGame
{
    public class LifeObject 
    {
        private Vector3 pos;
        public void Set()
        {
            LifeManager.StartPosAdd(pos);
        }
        public LifeObject(Vector3 pos)
        {
            this.pos = pos;
        }
    }
}