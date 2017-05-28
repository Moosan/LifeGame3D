using UnityEngine;
namespace LifeGame
{
    public class LifePrefab : MonoBehaviour
    {
        public GameObject lifeManager;
        public GameObject Prefab;
        public Vector3[] PresetPos;
        private Vector3 pos;
        private void Start()
        {
            for(int i = 0; i < PresetPos.Length; i++)
            {
                PresetPos[i] = new Vector3((int)PresetPos[i].x,(int)PresetPos[i].y,(int)PresetPos[i].z);
            }
            objs = new GameObject[PresetPos.Length];
            end = false;
        }
        private GameObject[] objs;
        private bool end;
        public void Update()
        {
            if (LifeManager.IsLifeManagerInitialized&&!end)
            {
                for(int i = 0; i < PresetPos.Length; i++)
                {
                    GameObject obj=Instantiate(Prefab,PresetPos[i],new Quaternion());
                    objs[i] = obj;
                    var life = new LifeObject(PresetPos[i]);
                    life.Set();
                }
                end = true;
            }
            if (lifeManager.GetComponent<LifeManager>().Ok)
            {
                foreach(var obj in objs)
                {
                    Destroy(obj);
                }
                Destroy(gameObject);
                Destroy(this);
            }
        }
    }
}