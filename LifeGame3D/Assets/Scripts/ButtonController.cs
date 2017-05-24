using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LifeGame
{
    public class ButtonController : MonoBehaviour
    {
        public GameObject GameObject;
        public void ButtonPush()
        {
            GameObject.GetComponent<LifeManager>().GameStart();
            Destroy(gameObject);
        }
    }
}
