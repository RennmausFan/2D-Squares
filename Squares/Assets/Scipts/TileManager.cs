using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class TileManager : MonoBehaviour {

    public static TileManager Instance;

    [Header("Tilemaps")]
    public Tilemap baseMap;
    public Tilemap unitsMap;
    public Tilemap masksMap;
    public Tilemap overlayMap;
    public Tilemap arrowMap;
    public Tilemap prepMap;
    public Tilemap pinkMaskMap;
    public Tilemap purpleMaskMap;
    public Tilemap fogAllies;
    public Tilemap fogEnemies;

    [Header("Tiles")]
    public TileBase greenMask;
    public TileBase redMask;
    public TileBase pinkMask;
    public TileBase purpleMask;
    public TileBase fog;

    public static Vector3Int mousePos;

    void Awake()
    {
        SetAllTileMapsToActive(true);
        Instance = this;
    }

    void Update ()
    {
        mousePos = GetMouseCellPos(baseMap);
    }

    //Enable or disable a specific tileMap
    private void SetTileMapActive(Tilemap pTileMap, bool state)
    {
        pTileMap.gameObject.SetActive(state);
        //pTileMap.RefreshAllTiles();
    }

    //Enable or disable all tilemaps
    private void SetAllTileMapsToActive(bool state)
    {
        SetTileMapActive(baseMap, state);
        SetTileMapActive(unitsMap, state);
        SetTileMapActive(masksMap, state);
        SetTileMapActive(overlayMap, state);
        SetTileMapActive(arrowMap, state);
        SetTileMapActive(prepMap, state);
        SetTileMapActive(pinkMaskMap, state);
        SetTileMapActive(purpleMaskMap, state);
        SetTileMapActive(fogAllies, state);
        SetTileMapActive(fogEnemies, state);
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

    //Get if at a position is a specific tile
    public bool CheckForTile(Vector3Int pPos, TileBase pTile, Tilemap pMap)
    {
        TileBase tileToCompare = pMap.GetTile(pPos);
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

    //Get if at a position is one of the specific tiles
    public bool CheckForTiles(Vector3Int pPos, TileBase[] pTiles, Tilemap pMap)
    {
        TileBase tileToCompare = pMap.GetTile(pPos);

        foreach (TileBase tile in pTiles)
        {
            if (tile == tileToCompare)
            {
                return true;
            }
        }
        return false;
    }

    //Search if tile exists in an TileBase-Array
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

    //Fill up the current fog layer with fog (based on the tiles on the baseMap)
    public void SetupFogBaseLayer(Team pTeam)
    {
        //Select fog map
        Tilemap fogmap;
        if (pTeam == Team.Allies)
        {
            fogmap = fogAllies;
        }
        else
        {
            fogmap = fogEnemies;
        }
        //Compress baseMap and get allTiles
        baseMap.CompressBounds();
        TileBase[] allTiles = baseMap.GetTilesBlock(baseMap.cellBounds);
        //Copy setting of the baseMap to fog layer
        fogmap.size = baseMap.size;
        fogmap.origin = baseMap.origin;
        fogmap.ResizeBounds();
        //Set fog for every tile (that is not null)
        for (int i = 0; i < allTiles.Length; i++)
        {
            if (allTiles[i] != null)
            {
                allTiles[i] = fog;
            }
        }
        //Apply tiles to fog map
        fogmap.SetTilesBlock(fogmap.cellBounds, allTiles);
    }

    //Fill a tilemap with a specific tile
    public static void FillTileMap(Tilemap pTilemap, TileBase pTile, bool pReplaceNull)
    {
        TileBase[] allTiles = pTilemap.GetTilesBlock(pTilemap.cellBounds);
        for (int i=0; i<allTiles.Length; i++)
        {
            //If ReplaceNull == false, null tiles are ignored 
            if (!pReplaceNull && allTiles[i] == null)
            {
                return;
            }
            allTiles[i] = pTile;
        }
        pTilemap.SetTilesBlock(pTilemap.cellBounds, allTiles);
    }
}