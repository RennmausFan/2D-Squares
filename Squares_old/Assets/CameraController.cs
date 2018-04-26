using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float speed;

	// Update is called once per frame
	void Update () {
		//Move Camera by dragging (press right click)
        if (Input.GetMouseButton(1))
        {
            float x = Input.GetAxis("Mouse X") * speed * Time.deltaTime;
            float y = Input.GetAxis("Mouse Y") * speed * Time.deltaTime;
            transform.position -= new Vector3(x, y, 0);
        }
	}
}
