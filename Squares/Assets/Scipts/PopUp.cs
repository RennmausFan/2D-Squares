using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour {

    public float duration;

    private  float timer;

    private Text textComp;

    void Awake()
    {
        textComp = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > duration)
        {
            Destroy(transform.parent.gameObject);
        }
        Color newColor = textComp.color;
        float ratio = timer / duration;
        newColor.a = Mathf.Lerp(1, 0, ratio);
        textComp.color = newColor;
        timer += Time.deltaTime;
    }

    public static void SpawnPopUp(GameObject prefab, string text, float duration, Color color)
    {
        GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        PopUp script = go.GetComponentInChildren<PopUp>();
        script.textComp.text = text;
        script.textComp.color = color;
        script.duration = duration;
    }
}
