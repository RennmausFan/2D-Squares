using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float speed;

    public bool isMoving;

    public float moveTime;

    public static CameraController Instance;

    void Awake()
    {
        Instance = this;    
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(MoveCameraTo(RoundManager.currentUnit.transform.position, moveTime));
        }
        if (isMoving)
        {
            return;
        }
		//Move Camera by dragging (press right click)
        if (Input.GetMouseButton(1))
        {
            float x = Input.GetAxis("Mouse X") * speed * Time.deltaTime;
            float y = Input.GetAxis("Mouse Y") * speed * Time.deltaTime;
            transform.position -= new Vector3(x, y, 0);
        }
	}
    
    public void MoveCameraTo(Vector3 destination)
    {
        StartCoroutine(MoveCameraTo(destination, moveTime));
    }

    //Move this unit over time
    private IEnumerator MoveCameraTo(Vector3 destination, float overTime)
    {
        isMoving = true;
        Vector3 start = transform.position;
        float startTime = Time.time;
        destination = new Vector3(destination.x, destination.y, start.z);
        while (Time.time < startTime + overTime)
        {
            transform.position = Vector3.Lerp(start, destination, (Time.time - startTime) / overTime);
            yield return null;
        }
        transform.position = destination;
        isMoving = false;
    }
}
