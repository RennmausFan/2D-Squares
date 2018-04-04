using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TeamName { Enemies, Allies };

public class RoundManager : MonoBehaviour {

    public static RoundManager Instance;

    public static int round;

    public static List<Unit> currentTeam;

    public static Unit currentUnit;

    [SerializeField]
    private GameObject popUp;

    public TeamName startTeamName;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {

        //Setup currentTeam and currentUnit
        if (startTeamName == TeamName.Allies)
        {
            currentTeam = UnitManager.allies;
            currentUnit = UnitManager.allies[0];
        }
        else if (startTeamName == TeamName.Enemies)
        {
            currentTeam = UnitManager.enemies;
            currentUnit = UnitManager.enemies[0];
        }
        RoundStart();
	}
	
    //Start a round
    void RoundStart()
    {
        round++;
        print("Round: " + round + " start!");
        GameObject go = Instantiate(popUp, Vector3.zero, Quaternion.identity);
        go.GetComponentInChildren<Text>().text = "ROUND " + round + " START!";
        MaskGenerator.Instance.GenerateMask(currentUnit);
    }

    //End a round
    void RoundEnd()
    {
        print("Round: " + round + " end!");

        //Swap teams
        SwapTeams();

        //Reset every unit turns and attacks
        foreach (Unit unit in UnitManager.allUnits)
        {
            unit.turns = unit.maxTurns;
            unit.attacks = unit.maxAttacks;
            unit.buffs.OnRoundEnd();
        }
    }

    //Swap teams
    private void SwapTeams()
    {
        if (currentTeam == UnitManager.allies)
        {
            currentTeam = UnitManager.enemies;
            currentUnit = UnitManager.enemies[0];
            MaskGenerator.Instance.GenerateMask(currentUnit);
        }
        else
        {
            currentTeam = UnitManager.allies;
            currentUnit = UnitManager.allies[0];
            MaskGenerator.Instance.GenerateMask(currentUnit);
        }
    }

    //Method triggered by "Turn over" button
    public void EndTurn()
    {
        //If currentTeam equals start team, the other team needs to take their turns
        if (startTeamName == TeamName.Allies && currentTeam == UnitManager.allies || startTeamName == TeamName.Enemies && currentTeam == UnitManager.enemies)
        {
            SwapTeams();
        }
        else
        {
            RoundEnd();
            RoundStart();
        }
    }

	// Update is called once per frame
	void Update () {

        //Highlight current unit and reset the highlight for every other unit
        foreach (Unit unit in UnitManager.allUnits)
        {
            if (currentUnit == unit)
            {
                unit.Highlight();
            }
            else
            {
                unit.ResetHighlight();
            }
        }

        //If currentUnit cant act select a new Unit from that team that can act (if theres one)
        if (!currentUnit.canAct)
        {
            foreach (Unit unit in currentTeam)
            {
                if (unit.canAct)
                {
                    currentUnit = unit;
                    break;
                }
            }
        }

        //If current team cant act swap team or if round is over call RoundEnd()
        if (!UnitManager.TeamCanAct(currentTeam))
        {
            //If currentTeam equals start team, the other team needs to take their turns
            if(startTeamName == TeamName.Allies && currentTeam == UnitManager.allies || startTeamName == TeamName.Enemies && currentTeam == UnitManager.enemies)
            {
                SwapTeams();
            }
            else
            {
                RoundEnd();
                RoundStart();
            }
        }
    }
}
