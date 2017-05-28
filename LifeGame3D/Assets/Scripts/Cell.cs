using System.Linq;
using Boo.Lang;
using UnityEngine;

namespace Assets.Scripts
{
    public class Cell : MonoBehaviour
    {
        [SerializeField]private Mesh Mesh;
        [SerializeField]private Material Material;
        [SerializeField]private int X,Y,Z;
        [SerializeField]private int DepopulationMax;
        [SerializeField]private int CongestionMin;
        private V3 _position { get; set; }
        private Transform _parent;
        private static int _cellCount{ get; set; }
        private Cell[] _arounds{ get; set; }
        private bool _isActive{ get; set; }
        private bool _hasAllAround{ get; set; }
        private bool _doThisHaveName{ get; set; }
        private static List<Cell> _cells{ get; set; }
        private static V3[] _directions{ get; set; }

        private static void DirectionsInitialize()
        {
            _directions=new V3[26];
            var count = 0;
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    for (var k = -1; k <= 1; k++)
                    {
                        if (i == 0 && j == 0 && k == 0) continue;
                        _directions[count] = new V3(i, j, k);
                        count++;
                    }
                }
            }
        }

        protected virtual void Start()
        {
            if (_cells == null)
            {
                _cells=new List<Cell>();
            }
            _cellCount++;
            //Debug.Log("_cellCount++");
            if (_parent == null)
            {
                _parent = GameObject.Find("Parent").transform;
            }
            if (_directions == null)
            {
                //Debug.Log("BeforeDirectionInitialize");
                DirectionsInitialize();
            }
            if (_position == null)
            {
               // Debug.Log("BeforePositionInitialize");
                _position = new V3(X, Y, Z);
                _isActive = true;
                transform.parent = _parent.transform;
                transform.position = _position.Pos;
            }
            if (!_doThisHaveName)
            {
                //Debug.Log("GetId");
                gameObject.name = _position.Id;
                _doThisHaveName = true;
            }
            gameObject.AddComponent<MeshFilter>().mesh=Mesh;
            gameObject.AddComponent<MeshRenderer>().material = Material;
            gameObject.GetComponent<MeshRenderer>().enabled = _isActive;
            _cells.Add(this);
            if (_hasAllAround || !_isActive) return;
            //Debug.Log("StartCoroutine");
            //StartCoroutine(YieldWaitForSecond());
            AroundInitilize();
        }

        private static bool _lookAround;
        private bool _doNextAction;
        private bool _endAction;
        private static int _lookCount;

        protected virtual void Update()
        {
            if (!_lookAround)
            {
                //Debug.Log("!_lookAround");
                if (!_endAction)
                {
                    //Debug.Log("!endAction");
                    _doNextAction = NextActionBool();
                    _lookCount++;
                    _endAction = true;
                }
                if (_lookCount != _cellCount || _lookAround) return;
                //Debug.Log("合計が等しくなった？" +_lookCount+"=="+_cellCount);
                _lookAround = true;
                _lookCount = 0;
                EndActionBoolChange(false);

            }
            else
            {
                //Debug.Log("LookAround");
                if (!_endAction)
                {
                    //Debug.Log("!endAction2");
                    if (_doNextAction)
                    {
                        //StartCoroutine(YieldWaitForSecond());
                        BornOrDie();
                        if (!_hasAllAround && _isActive)
                        {
                            AroundInitilize();
                        }
                        _doNextAction = false;
                    }
                    _lookCount++;
                    _endAction = true;
                }
                //Debug.Log(_lookCount + ":" + _cellCount);
                if (_lookCount != _cellCount || !_lookAround) return;
                //Debug.Log(_lookAround+"=="+_cellCount);
                _lookCount = 0;
                _lookAround = !_lookAround;
                EndActionBoolChange(false);
            }
        }

        private static void EndActionBoolChange(bool change)
        {
            foreach (var cell in _cells)
            {
                cell._endAction = change;
            }
        }

        private bool NextActionBool()
        {
            var aroundCount = AroundCount();
            if (aroundCount >= CongestionMin || aroundCount <= DepopulationMax)
            {
                return _isActive;
            }
            return !_isActive;
        }

        private void BornOrDie()
        {
            _isActive = !_isActive;
            gameObject.GetComponent<MeshRenderer>().enabled = _isActive;
        }
        
        /*private static IEnumerator YieldWaitForSecond()
        {
            Debug.Log("WaitForSecond");
            yield return new WaitForSeconds(0.5f);
        }*/

        private void AroundInitilize()
        {
            _arounds=new Cell[26];
            for (var i = 0; i < _directions.Length; i++)
            {
                var nextPos = _directions[i] + _position;
                var getName = nextPos.Id;
                var nextCell = GameObject.Find(getName);
                if (nextCell)
                {
                    _arounds[i] = nextCell.GetComponent<Cell>();
                    _arounds[i].GetOneCell(this);
                }
                else
                {
                    _arounds[i]=new GameObject(getName).AddComponent<Cell>().Initialize(this,nextPos);
                }
            }
            _hasAllAround = true;
        }

        private Cell Initialize(Cell originCell,V3 newPos)
        {
            Mesh = originCell.Mesh;
            Material = originCell.Material;
            _isActive = false;
            _doNextAction =false;
            _endAction = false;
            _position = newPos;
            X = newPos.X;
            Y = newPos.Y;
            Z = newPos.Z;
            _parent = originCell._parent;
            transform.parent = _parent.transform;
            transform.position = _position.Pos;
            DepopulationMax = originCell.DepopulationMax;
            CongestionMin = originCell.CongestionMin;
            GetOneCell(originCell);
            return this;
        }

        private void GetOneCell(Cell cell)
        {
            if(_hasAllAround)return;
            if (_arounds == null)
            {
                _arounds=new Cell[26];
            }
            for (var i = 0; i < _arounds.Length; i++)
            {
                if (_arounds[i] != null) continue;
                _arounds[i] = cell;
                break;
            }
        }

        private int AroundCount()
        {
            return _arounds.Where(cell => cell != null).Count(cell => cell._isActive);
        }
    }
    public class V3
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public Vector3 Pos { get; set; }
        public string Id { get; set; }
        public V3(int x,int y,int z)
        {
            X = x;
            Y = y;
            Z = z;
            Pos=new Vector3(x,y,z);
            Id = x + y.ToString() + z;
        }

        public static V3 operator +(V3 a,V3 b)
        {
            return new V3(a.X+b.X,a.Y+b.Y,a.Z+b.Z);
        }
    }
}
