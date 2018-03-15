using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {

    public static List<Unit> allUnits = new List<Unit>();

    public static List<Unit> allies = new List<Unit>();
    public static List<Unit> enemies = new List<Unit>();

    // Use this for initialization
    void Start () {
        Unit[] units = FindObjectsOfType<Unit>();
        foreach (Unit unit in units)
        {
            if (unit.tag == "Ally")
            {
                allies.Add(unit);
            }
            else if (unit.tag == "Enemy")
            {
                enemies.Add(unit);
            }
            allUnits.Add(unit);
        }
    }

   void Update()
    {
        if (allies.Count == 0)
        {
            print("The Enemies win!");
        }
        if (enemies.Count == 0)
        {
            print("You win!");
        }
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
