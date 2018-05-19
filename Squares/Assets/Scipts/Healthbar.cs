using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Unit unit;

    public float gapSize;
    public float height;
    public float borderOffset;

    public Color fullColor_Ally;
    public Color fullColor_Enemy;
    public Color emptyColor;

    [SerializeField]
    private GameObject healthTile;

    [SerializeField]
    private GameObject healthText;

    private GameObject[] tiles;

    private Vector3 spawnPoint;

    private int count;

    private float totalWidth;
    private float totalGapSize;
    private float tileWidth;

    // Use this for initialization
    void Start()
    {
        //Setup stuff
        count = unit.maxHealth;
        tiles = new GameObject[count];
        RectTransform healthtile = healthTile.GetComponent<RectTransform>();
        RectTransform healthbar = GetComponent<RectTransform>();

        //Setup messurements
        totalWidth = healthbar.rect.width - 2* borderOffset;
        totalGapSize = gapSize * (count - 1);
        tileWidth = (totalWidth / count) - (totalGapSize / count);
        spawnPoint = new Vector3(-totalWidth / 2, 0, 0);

        //Instantiate tiles
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
            scale.x = tileWidth / tileRect.rect.width;
            scale.y = height / tileRect.rect.height;
            tileRect.localScale = scale;

            //Parent tile and add it to the array
            tileRect.SetParent(transform, false);
            tiles[i] = newTile;

            //Color
            if (unit.teamName == Team.Allies)
            {
                tiles[i].GetComponent<Image>().color = fullColor_Ally;
            }
            else
            {
                tiles[i].GetComponent<Image>().color = fullColor_Enemy;
            }
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
            if (unit.teamName == Team.Allies)
            {
                tiles[i].GetComponent<Image>().color = fullColor_Ally;
            }
            else
            {
                tiles[i].GetComponent<Image>().color = fullColor_Enemy;
            }
        }
        for (int p = health; p < tiles.Length; p++)
        {
            tiles[p].GetComponent<Image>().color = emptyColor;
        }
        //Update text
        if (healthText != null)
        {
            healthText.GetComponent<Text>().text = unit.health + " / " + unit.maxHealth;
        }
    }
}
