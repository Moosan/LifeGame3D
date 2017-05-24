using UnityEngine;

public class CameraController : MonoBehaviour {
    public float MoveSpeed;
	void Update () {
        var d = Time.deltaTime;
        var speed = d * MoveSpeed;
        var right = Input.GetKey(KeyCode.RightArrow);
        var left = Input.GetKey(KeyCode.LeftArrow);
        if(!(right&&left)){
            if(right)
            {
                transform.position += transform.right * speed;
            }
            if(left)
            {
                transform.position += -transform.right * speed;
            }
        }
        var up = Input.GetKey(KeyCode.UpArrow);
        var down = Input.GetKey(KeyCode.DownArrow);
        if (!(up && down))
        {
            if (up)
            {
                transform.position += transform.forward*speed;
            }
            if (down)
            {
                transform.position += -transform.forward * speed;
            }
        }
    }
}
