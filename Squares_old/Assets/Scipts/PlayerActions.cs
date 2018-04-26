using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerActions : MonoBehaviour {

    [SerializeField]
    private TileManager tileManager;

    [SerializeField]
    private GameObject statsDisplayer;

    public static bool unitIsMoving;

    private Unit currentUnit;
    private Vector3Int mousePos;
    private Vector3Int lastMousePos;
    private TileBase mouseTile;

    private Unit lastUnit;
    private GameObject lastDisplayer;
    private Path currentAttackPath;
	
	void Update ()
    {
        //Return if - Not in state play or a unit is moving
        if (unitIsMoving || RoundManager.state != GameCycle.Play)
        {
            return;
        }

        currentUnit = RoundManager.currentUnit;
        mousePos = TileManager.mousePos;

        /**
        if (Input.GetKeyDown(KeyCode.F))
        {
            MaskGenerator.Instance.GenerateMaskPurple(currentUnit);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            TileManager.Instance.purpleMaskMap.ClearAllTiles();
        }
        **/

        //On click
        if (Input.GetMouseButtonDown(0))
        {
            OnClick_GreenMask();
            OnClick_RedMask();
            OnClick_Unit();
        }

        //If mouse hovers over different tile than last update
        if (mousePos != lastMousePos)
        {
            lastMousePos = mousePos;

            //Reset last unit
            if (lastUnit != null && !lastUnit.isDead)
            {
                lastUnit.healthBar.SetActive(true);
            }

            OnHover_PathToLocation();
            OnHover_PathToAttackPosition();
            OnHover_HandleStatsDisplayer();
            
        }

        //Destroy lastStatsDisplayer if unit is dead
        if (lastUnit != null && lastDisplayer != null)
        {
            if (lastUnit.isDead)
            {
                Destroy(lastDisplayer);
            }
        }
    }

    private void OnClick_GreenMask()
    {
        if (tileManager.masksMap.GetTile(mousePos) == tileManager.greenMask)
        {
            currentUnit.WalkPath(PathEngine.Instance.GetPath(currentUnit, mousePos));
        }
    }

    private void OnClick_RedMask()
    {
        if (tileManager.masksMap.GetTile(mousePos) == tileManager.redMask)
        {
            List<Vector3Int> attackPositions = currentUnit.GetAttackPositions();
            bool canAttack = false;
            if (attackPositions == null)
            {
                return;
            }
            foreach (Vector3Int v in attackPositions)
            {
                if (currentUnit.GetPos() + v == mousePos)
                {
                    currentUnit.Attack(mousePos);
                    canAttack = true;
                }
            }
            if (currentAttackPath != null && !canAttack)
            {
                currentUnit.WalkPath(currentAttackPath);
            }
        }
    }

    private void OnClick_Unit()
    {
        if (tileManager.CheckForUnit(TileManager.mousePos))
        {
            Unit unitAtPos = tileManager.GetUnitAtPosition(TileManager.mousePos);
            if (unitAtPos.team == RoundManager.currentTeam && unitAtPos.canAct)
            {
                RoundManager.currentUnit = unitAtPos;
                currentUnit = unitAtPos;
                MaskGenerator.Instance.GenerateMasks(currentUnit);
            }
        }
    }

    private void OnHover_PathToLocation()
    {
        if (tileManager.masksMap.GetTile(mousePos) == tileManager.greenMask)
        {
            Path path = PathEngine.Instance.GetPath(currentUnit, mousePos);
            if (path != null)
            {
                PathEngine.Instance.DisplayPath(path, currentUnit);
            }
        }

    }

    private void OnHover_PathToAttackPosition()
    {
        //Generate Path to attack position
        if (tileManager.masksMap.GetTile(mousePos) == tileManager.redMask)
        {
            List<Vector3Int> attackPositions = currentUnit.GetAttackPositions();
            if (attackPositions == null)
            {
                return;
            }
            foreach (Vector3Int v in attackPositions)
            {
                Path attackPath = PathEngine.Instance.GetPath(currentUnit, mousePos + v);
                if (attackPath != null && tileManager.masksMap.GetTile(mousePos + v))
                {
                    PathEngine.Instance.DisplayPath(attackPath, currentUnit);
                    currentAttackPath = attackPath;
                }
            }
        }
    }

    private void OnHover_HandleStatsDisplayer()
    {
        //Destroy lastStatsDisplayer if tilePos changes
        if (lastDisplayer != null)
        {
            Destroy(lastDisplayer);
        }
        //Spawn statsdisplayer if unit is on tile
        if (tileManager.CheckForUnit(mousePos))
        {
            Unit unit = tileManager.GetUnitAtPosition(mousePos);
            GameObject go = StatsDisplayer.NewStatsDisplayer(statsDisplayer, unit);
            unit.healthBar.SetActive(false);
            lastDisplayer = go;
            lastUnit = unit;
        }
    }
}
