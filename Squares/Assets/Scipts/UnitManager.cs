using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {

    public static LinkedList<Unit> allUnits = new LinkedList<Unit>();

    public static LinkedList<Unit> allies = new LinkedList<Unit>();
    public static LinkedList<Unit> enemies = new LinkedList<Unit>();

    // Use this for initialization
    void Start () {
        Unit[] units = FindObjectsOfType<Unit>();
        foreach (Unit unit in units)
        {
            if (unit.tag == "Ally")
            {
                allies.AddLast(unit);
            }
            else if (unit.tag == "Enemy")
            {
                enemies.AddLast(unit);
            }
            allUnits.AddLast(unit);
        }
        print("AllUnits: " + allUnits.Count);
        print("Allies: " + allies.Count);
        print("Enemies: " + enemies.Count);
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
}
