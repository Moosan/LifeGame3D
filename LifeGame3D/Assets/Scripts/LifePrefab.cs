using UnityEngine;
namespace LifeGame
{
    public class LifePrefab : MonoBehaviour
    {
        public GameObject Prefab;
        public Vector3 CenterPos;
        public Vector3[] PresetPos;
        private Vector3 pos;
        private Vector3[] positions;
        public Axis DefaltMoveDirection;
        public Axis Axis;
        public bool moveFoward;
        public void Start()
        {
            Vector3 position = transform.position;
            pos = new Vector3((int)position.x, (int)position.y, (int)position.z);
            //向きを変えれるようにしたつもりなんだけどなんも変わんない辛い
            var len = PresetPos.Length;
            if (DefaltMoveDirection != Axis)
            {
                Vector3 rotateAxis = DefaltMoveDirection == Axis.x ? (DefaltMoveDirection == Axis.y ? (Axis == Axis.x ? Vector3.forward : Vector3.left) : (Axis == Axis.x ? Vector3.down : Vector3.right)) : (Axis == Axis.y ? Vector3.back : Vector3.up);
                for (int i = 0; i < len; i++)
                {
                    PresetPos[i] = Quaternion.Euler(90 * rotateAxis) * (PresetPos[i] - CenterPos);
                }
            }
            if (!moveFoward)
            {
                for (int i = 0; i < len; i++)
                {
                    PresetPos[i] = -PresetPos[i];
                }
            }
            positions = new Vector3[len];
            for (int i = 0; i < len; i++)
            {
                positions[i] = pos + PresetPos[i];
            }
        }
        public void Update()
        {
            if (LifeManager.IsLifeManagerInitialized)
            {
                LifeManager.PutLife(positions, Prefab);
                Destroy(gameObject);
                Destroy(this);
            }
        }
    }
    public enum Axis
    {
        x, y, z
    }
}