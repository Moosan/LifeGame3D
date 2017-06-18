using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cell
{
    public class GameStart : MonoBehaviour
    {
        private void Awake()
        {
            Cell.GameStart = false;
        }
        public void PushButton()
        {
            Cell.Game_Start();
        }
    }
}
