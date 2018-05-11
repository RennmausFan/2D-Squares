using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {

    public static List<Unit> allUnits = new List<Unit>();

    public static List<Unit> allies = new List<Unit>();
    public static List<Unit> enemies = new List<Unit>();

    public static Unit enemyShogun;
    public static Unit allyShogun;

    // Use this for initialization
    public static void OnPlay()
    {
        /*
        allUnits.Clear();
        allies.Clear();
        enemies.Clear();
        
        Unit[] units = FindObjectsOfType<Unit>();
        foreach (Unit unit in units)
        {
            if (unit.tag == "Ally")
            {
                if (unit.unitClass == CharClasses.Shogun)
                {
                    allyShogun = unit;
                }
                allies.Add(unit);
                unit.team = allies;
                unit.teamName = Team.Allies;
            }
            else if (unit.tag == "Enemy")
            {
                if (unit.unitClass == CharClasses.Shogun)
                {
                    enemyShogun = unit;
                }
                enemies.Add(unit);
                unit.team = enemies;
                unit.teamName = Team.Enemies;
            }
            allUnits.Add(unit);
            
        }
        */
    }

    public static void SetIsVisibleAllUnits()
    {
        foreach(Unit u in allUnits)
        {
            u.SetIsVisible();
        }
    }

    public static void SpawnUnit(GameObject prefab, Vector3 pos, Team team)
    {
        GameObject go = Instantiate(prefab, pos, Quaternion.identity);
        Unit unit = go.GetComponent<Unit>();
        unit.transform.parent = GameObject.FindWithTag("Units").transform;
        if (team == Team.Allies)
        {
            if (unit.unitClass == CharClasses.Shogun)
            {
                allyShogun = unit;
            }
            unit.tag = "Ally";
            unit.teamName = Team.Allies;
            unit.team = allies;
            allies.Add(unit);
        }
        else if (team == Team.Enemies)
        {
            if (unit.unitClass == CharClasses.Shogun)
            {
                enemyShogun = unit;
            }
            unit.tag = "Enemy";
            unit.teamName = Team.Enemies;
            unit.team = enemies;
            enemies.Add(unit);
        }
        allUnits.Add(unit);
    }

    //Returns if team can act or not
    public static bool TeamCanAct(List<Unit> pTeam)
    {
       foreach (Unit unit in pTeam)
       {
            if (unit.canAct)
            {
                return true;
            }
       }
       return false;
    }
}
