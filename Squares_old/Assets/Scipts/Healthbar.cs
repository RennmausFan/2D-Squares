using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Unit unit;

    public float gapScale;

    public Color fullColor;
    public Color emptyColor;

    [SerializeField]
    private GameObject healthTile;

    [SerializeField]
    private GameObject healthText;

    private GameObject[] tiles;

    private Vector3 spawnPoint;

    private int count;

    private float totalWidth;
    private float gapSize;
    private float tileWidth;

    // Use this for initialization
    void Start()
    {
        count = unit.maxHealth;

        RectTransform rectTransform = healthTile.GetComponent<RectTransform>();
        totalWidth = rectTransform.rect.width * rectTransform.localScale.x;

        gapSize = (totalWidth * gapScale) * count / (count - 1);
        tileWidth = (totalWidth / count) - (totalWidth * gapScale);
        spawnPoint = new Vector3(-totalWidth / 2, 0, 0);

        tiles = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            //Instantiate tile
            GameObject newTile = Instantiate(healthTile);
            RectTransform tileRect = newTile.GetComponent<RectTransform>();
            Vector3 pos = spawnPoint;

            //Calc position
            if (count != 1)
            {
                pos.x += tileWidth * i + gapSize * i;
            }
            tileRect.anchoredPosition = pos;

            //Rescale tile
            Vector3 scale = tileRect.localScale;
            scale.x = scale.x / count - gapScale;
            tileRect.localScale = scale;

            //Parent tile and add it to the array
            tileRect.SetParent(transform, false);
            tiles[i] = newTile;
        }

    }

    // Update is called once per frame
    void Update()
    {
        //Set health
        int health = unit.health;
        if(health <= 0)
        {
            return;
        }
        for (int i = 0; i < health; i++)
        {
            tiles[i].GetComponent<Image>().color = fullColor;
        }
        for (int p = health; p < tiles.Length; p++)
        {
            tiles[p].GetComponent<Image>().color = emptyColor;
        }
        //Update text
        healthText.GetComponent<Text>().text = unit.health + " / " + unit.maxHealth;
    }
}
