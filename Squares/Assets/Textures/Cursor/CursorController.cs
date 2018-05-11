using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour {

    public static CursorController Instance;

    [SerializeField]
    private SpriteRenderer spriteRend;

    public float speed;

    private bool firstTime = true;

	void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (RoundManager.state != GameCycle.Play)
        {
            spriteRend.enabled = false;
        }
        else
        {
            if (firstTime)
            {
                firstTime = false;
                transform.position = RoundManager.currentUnit.transform.position;
            }
            spriteRend.enabled = true;
        }
    }

    public void MoveToCurrentPlayer()
    {
        StartCoroutine(MoveObject(transform.position, RoundManager.currentUnit.transform.position, speed));
    }

    public void MoveTo(Vector3 target)
    {
        StartCoroutine(MoveObject(transform.position, target, speed));
    }

    public void MoveTo(Vector3 target, float overTime)
    {
        StartCoroutine(MoveObject(transform.position, target, overTime));
    }

    //This is used to move the cursor to a new position over a certain time
    IEnumerator MoveObject(Vector3 start, Vector3 target, float overTime)
    {
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {
            transform.position = Vector3.Lerp(start, target, (Time.time - startTime) / overTime);
            yield return null;
        }
        transform.position = target;
    }
}
