﻿using System.Collections;
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

    public Tilemap preparationMap;

    [SerializeField]
    private TileBase debugTile;

    private GameObject lastStatsDisplayer;
    private Unit lastUnit;

    private Path currentAttackPath;

    [SerializeField]
    private GameObject statsDisplayer;

    public TileBase greenMask;
    public TileBase redMask;

    public static bool unitIsMoving;

    public int sizeX, sizeY;

    Vector3Int lastTilePos = new Vector3Int(10000, 10000, 10000);

    void Awake()
    {
        Instance = this;
    }

    void Start () {
       //Get bounds from base map in cell size
       sizeX = baseMap.cellBounds.size.x;
       sizeY = baseMap.cellBounds.size.y;
    }

    public static TileBase GetTileAtMousePos(Tilemap map)
    {
        //Get tile mouse is focusing on
        Vector3 mousePos = Input.mousePosition;
        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3Int tilePos = map.WorldToCell(mousePosWorld);
        TileBase tile = map.GetTile(tilePos);
        return tile;
    }

    public static Vector3Int GetTilePosFromMouse(Tilemap map)
    {
        //Get tile mouse is focusing on
        Vector3 mousePos = Input.mousePosition;
        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3Int tilePos = map.WorldToCell(mousePosWorld);
        return tilePos;
    }

    // Update is called once per frame
    void Update ()
    {
        if (unitIsMoving || RoundManager.state != GameCycle.Play)
        {
            return;
        }

        //Get tile mouse is focusing on
        Vector3 mousePos = Input.mousePosition;
        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3Int tilePos = masksMap.WorldToCell(mousePosWorld);

        Unit currentUnit = RoundManager.currentUnit;

        #region Move, Attack and Select Units
        if (Input.GetMouseButtonDown(0))
        {
            if (masksMap.GetTile(tilePos) == greenMask)
            {
                currentUnit.WalkPath(PathEngine.Instance.GetPath(currentUnit, tilePos));
            }
            else if (masksMap.GetTile(tilePos) == redMask)
            {
                //Check if unit is in range;
                Vector3Int[] attackPositions = currentUnit.attackPositions;
                bool canAttack = false;
                if (attackPositions == null)
                {
                    return;
                }
                foreach (Vector3Int v in attackPositions)
                {
                    if (currentUnit.GetPos() + v == tilePos)
                    {
                        currentUnit.Attack(GetUnitAtPosition(tilePos));
                        canAttack = true;
                    }
                }
                if (currentAttackPath != null && !canAttack)
                {
                    currentUnit.WalkPath(currentAttackPath);
                }
            }
            else if (CheckForUnit(tilePos))
            {
                Unit unitAtPos = GetUnitAtPosition(tilePos);
                if (unitAtPos.team == RoundManager.currentTeam)
                {
                    RoundManager.currentUnit = unitAtPos;
                    currentUnit = unitAtPos;
                    MaskGenerator.Instance.GenerateMask(currentUnit);
                }
            }
        }
        #endregion

        #region Display Path and Spawn StatsDisplayer
        if (tilePos != lastTilePos)
        {
            lastTilePos = tilePos;
            //Reset last unit
            if (lastUnit != null && !lastUnit.isDead)
            {
                lastUnit.healthBar.SetActive(true);
            }

            //Generate Path to location
            if (masksMap.GetTile(tilePos) == greenMask)
            {
                Path path = PathEngine.Instance.GetPath(currentUnit, tilePos);
                if (path != null)
                {
                    PathEngine.Instance.DisplayPath(path, currentUnit);
                }
            }

            //Generate Path to attack position
            if (masksMap.GetTile(tilePos) == redMask)
            {
                Vector3Int[] attackPositions = currentUnit.attackPositions;
                if (attackPositions == null)
                {
                    return;
                }
                foreach (Vector3Int v in attackPositions)
                {
                    Path attackPath = PathEngine.Instance.GetPath(currentUnit, tilePos + v);
                    if (attackPath != null && masksMap.GetTile(tilePos + v))
                    {
                        PathEngine.Instance.DisplayPath(attackPath, currentUnit);
                        currentAttackPath = attackPath;
                    }
                }
            }

            //Destroy lastStatsDisplayer if tilePos changes
            if (lastStatsDisplayer != null)
            {
                Destroy(lastStatsDisplayer);
            }
            //Spawn statsdisplayer if unit is on tile
            if (CheckForUnit(tilePos))
            {
                Unit unit = GetUnitAtPosition(tilePos);
                print(unit);
                GameObject go = StatsDisplayer.NewStatsDisplayer(statsDisplayer, unit);
                unit.healthBar.SetActive(false);
                lastStatsDisplayer = go;
                lastUnit = unit;
            }
        }
        #endregion

        //Destroy lastStatsDisplayer if unit is dead
        if (lastUnit != null && lastStatsDisplayer != null)
        {
            if (lastUnit.isDead)
            {
                Destroy(lastStatsDisplayer);
            }
        }
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
