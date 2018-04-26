using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class TileManager : MonoBehaviour {

    public static TileManager Instance;

    public Tilemap baseMap;

    public Tilemap unitsMap;

    public Tilemap masksMap;

    public Tilemap overlayMap;

    public Tilemap arrowMap;

    public Tilemap prepMap;

    public Tilemap pinkMaskMap;

    public Tilemap purpleMaskMap;

    public TileBase greenMask;
    public TileBase redMask;
    public TileBase pinkMask;
    public TileBase purpleMask;

    public static Vector3Int mousePos;

    void Awake()
    {
        Instance = this;
    }

    void Update ()
    {
        mousePos = GetMouseCellPos(baseMap);
    }
    
    //Converts the mouse pos in cell space
    public static Vector3Int GetMouseCellPos(Tilemap map)
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3Int tilePos = map.WorldToCell(mousePosWorld);
        return tilePos;
    }

    //Get if at a position is a specific tile
    public bool CheckForTile(Vector3Int pPos, TileBase pTile)
    {
        TileBase tileToCompare = baseMap.GetTile(pPos);
        if (pTile == tileToCompare)
        {
            return true;
        }
        return false;
    }

    //Get if at a position is one of the specific tiles
    public bool CheckForTiles(Vector3Int pPos, TileBase[] pTiles)
    {
        TileBase tileToCompare = baseMap.GetTile(pPos);

        foreach (TileBase tile in pTiles)
        {
            if (tile == tileToCompare)
            {
                return true;
            }
        }
        return false;
    }

    public static bool ArrayContainsTile(TileBase tile, TileBase[] tiles)
    {
        foreach (TileBase t in tiles)
        {
            if (tile == t)
            {
                return true;
            }
        }
        return false;
    }

    //Get if there is a unit at a specific position
    public bool CheckForUnit(Vector3Int pPos)
    {
        Vector3 pos = baseMap.GetCellCenterWorld(pPos);
        foreach (Unit unit in UnitManager.allUnits)
        {
            if (unit.transform.position == pos && !unit.isDead)
            {
                return true;
            }
        }
        return false;
    }

    //Get Unit at a specific position
    public Unit GetUnitAtPosition(Vector3Int pPos)
    {
        Vector3 pos = baseMap.GetCellCenterWorld(pPos);
        foreach (Unit unit in UnitManager.allUnits)
        {
            if (unit.transform.position == pos && !unit.isDead)
            {
                return unit;
            }
        }
        return null;
    }
}