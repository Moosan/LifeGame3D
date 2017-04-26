using UnityEngine;

public class Rotate : MonoBehaviour {

    public float RotateSpeed;
    private void Update()
    {
        transform.eulerAngles += new Vector3(0,RotateSpeed*Time.deltaTime,0);
    }
}
