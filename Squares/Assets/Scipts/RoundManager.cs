using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour {

    [SerializeField]
    private MaskGenerator maskGen;

    public static int round;

    public static LinkedList<Unit> currentTeam;

    public static Unit currentUnit;

	// Use this for initialization
	void Start () {
        currentTeam = UnitManager.allies;
        currentUnit = GameObject.FindWithTag("Ally").GetComponent<Unit>();
        maskGen.GenerateMask(currentUnit);
        round = 1;
	}
	
	// Update is called once per frame
	void Update () {

        //Check if turn of current team is over
        bool teamCanAct = false;
        //If currentUnit is out of turn
        if (currentUnit.turns <= 0)
        {
            foreach (Unit unit in currentTeam)
            {
                if (unit.canAct)
                {
                    currentUnit = unit;
                    maskGen.GenerateMask(currentUnit);
                    teamCanAct = true;
                }
            }
        }
        else
        {
            teamCanAct = true;
        }
        if (teamCanAct)
        {
            return;
        }

        print("Round Over!");
        //Swap teams
        if (currentTeam == UnitManager.allies)
        {
            currentTeam = UnitManager.enemies;
            currentUnit = GameObject.FindWithTag("Enemy").GetComponent<Unit>();
        }
        else
        {
            currentTeam = UnitManager.allies;
            currentUnit = GameObject.FindWithTag("Ally").GetComponent<Unit>();
        }

        //Round is over
        round++;
        foreach (Unit unit in UnitManager.allUnits)
        {
            unit.turns = unit.maxTurns;
        }
        maskGen.GenerateMask(currentUnit);
    }
}
