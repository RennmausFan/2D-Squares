using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class TileManager : MonoBehaviour {

    public Tilemap baseMap;

    public Tilemap unitsMap;

    public Tilemap masksMap;

    public Tilemap overlayMap;

    [SerializeField]
    private TileBase debugTile;

    public TileBase greenMask;

    public int sizeX, sizeY;

    void Start () {
       //Get bounds from base map in cell size
       sizeX = baseMap.cellBounds.size.x;
       sizeY = baseMap.cellBounds.size.y;
    }

    // Update is called once per frame
    void Update () {
		
	}

    public bool CheckForTile(int pX, int pY, TileBase pTile)
    {
        Vector3Int pos = new Vector3Int(pX, pY, 0);
        TileBase tileToCompare = baseMap.GetTile(pos);
        if (pTile == tileToCompare)
        {
            return true;
        }
        return false;
    }

    public bool CheckForTiles(int pX, int pY, TileBase[] pTiles)
    {
        Vector3Int pos = new Vector3Int(pX, pY, 0);
        TileBase tileToCompare = baseMap.GetTile(pos);

        foreach (TileBase tile in pTiles)
        {
            if (tile == tileToCompare)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckForTile(Vector3Int pPos, TileBase pTile)
    {
        TileBase tileToCompare = baseMap.GetTile(pPos);
        if (pTile == tileToCompare)
        {
            return true;
        }
        return false;
    }

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

    public bool CheckForUnit(int pX, int pY)
    {
        Vector3 pos = baseMap.GetCellCenterWorld(new Vector3Int(pX, pY, 0));
        foreach (Unit unit in UnitManager.allUnits)
        {
            if (unit.transform.position == pos)
            {
                return true;
            }
        }
        return false;
    }

    public Unit GetUnitAtPosition(int pX, int pY)
    {
        Vector3 pos = baseMap.GetCellCenterWorld(new Vector3Int(pX, pY, 0));
        foreach (Unit unit in UnitManager.allUnits)
        {
            if (unit.transform.position == pos)
            {
                return unit;
            }
        }
        return null;
    }

    public bool CheckForUnit(Vector3Int pPos)
    {
        Vector3 pos = baseMap.GetCellCenterWorld(pPos);
        foreach (Unit unit in UnitManager.allUnits)
        {
            if (unit.transform.position == pos)
            {
                return true;
            }
        }
        return false;
    }

    public Unit GetUnitAtPosition(Vector3Int pPos)
    {
        Vector3 pos = baseMap.GetCellCenterWorld(pPos);
        foreach (Unit unit in UnitManager.allUnits)
        {
            if (unit.transform.position == pos)
            {
                return unit;
            }
        }
        return null;
    }
}
