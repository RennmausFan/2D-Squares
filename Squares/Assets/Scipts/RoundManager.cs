using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public enum Team { Enemies, Allies }

public enum GameCycle { Preparation, Play, End };

public class RoundManager : MonoBehaviour {

    [SerializeField]
    private GameObject popUp;

    public static RoundManager Instance;

    public static int round;

    public static List<Unit> currentTeam;

    public static Unit currentUnit;

    public Team startTeam;

    public static GameCycle state = GameCycle.Preparation;

    //Prepartion
    [SerializeField]
    private TileBase prepEnemy;

    [SerializeField]
    private TileBase prepAlly;

    private Team currentTeamPrep;
    private int countAllies, countEnemies;

    void Awake()
    {
        Instance = this;
        currentTeamPrep = startTeam;
        countAllies = MapManager.currentMap.alliesTeamSize;
        countEnemies = MapManager.currentMap.enemiesTeamSize;
    }
	
    //Triggered when the game hits the state "play" the first time
    void OnGameStart()
    {
        //Setup currentTeam and currentUnit
        if (startTeam == Team.Allies)
        {
            currentTeam = UnitManager.allies;
            currentUnit = UnitManager.allies[0];
        }
        else if (startTeam == Team.Enemies)
        {
            currentTeam = UnitManager.enemies;
            currentUnit = UnitManager.enemies[0];
        }
        //CameraController.Instance.MoveCameraTo(currentUnit.transform.position);
        RoundStart();
    }

    //Start a round
    void RoundStart()
    {
        round++;
        print("Round: " + round + " start!");
        PopUp.SpawnPopUp(popUp, "ROUND " + round + " START!", 1.5f, Color.red);
        MaskGenerator.Instance.GenerateMasks(currentUnit);
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
            unit.attacked = 0;
            unit.walked = 0;
            unit.buffs.OnRoundEnd();
            unit.canAct = true;
        }
    }

    //Swap teams
    public void SwapTeams()
    {
        if (currentTeam == UnitManager.allies)
        {
            currentTeam = UnitManager.enemies;
            currentUnit = UnitManager.enemies[0];
            MaskGenerator.Instance.GenerateMasks(currentUnit);
        }
        else
        {
            currentTeam = UnitManager.allies;
            currentUnit = UnitManager.allies[0];
            MaskGenerator.Instance.GenerateMasks(currentUnit); 
        }
        if (UIManager.showDangerZone)
        {
            MaskGenerator.Instance.GenerateDangerZone();
        }
        CameraController.Instance.MoveCameraTo(currentUnit.transform.position);
    }

    public void EndTurn()
    {
        if (CameraController.Instance.isMoving || state != GameCycle.Play)
        {
            return;
        }
        //If currentTeam equals start team, the other team needs to take their turns
        if (startTeam == Team.Allies && currentTeam == UnitManager.allies || startTeam == Team.Enemies && currentTeam == UnitManager.enemies)
        {
            SwapTeams();
        }
        else
        {
            RoundEnd();
            RoundStart();
        }
    }

    //Depending in which state the game is do specific update tasks
    void Update()
    {
        if (state == GameCycle.Preparation)
        {
            Update_Preparation();
        }
        if (state == GameCycle.Play)
        {
            //Game ends
            if (UnitManager.enemies.Count == 0)
            {
                print("Allies won!");
                state = GameCycle.End;
            }
            else if (UnitManager.allies.Count == 0)
            {
                print("Enemies won!");
                state = GameCycle.End;
            }
            else if (UnitManager.allyShogun.isDead)
            {
                print("Enemies won!");
                state = GameCycle.End;
            }
            else if (UnitManager.enemyShogun.isDead)
            {
                print("Allies won!");
                state = GameCycle.End;
            }
            if (state != GameCycle.End) { Update_Play(); }
        }
        if (state == GameCycle.End)
        {
            Update_End();
        }
    }

    void Update_Preparation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Tilemap map = TileManager.Instance.prepMap;
            Vector3Int tilePos = TileManager.mousePos;
            Vector3 worldTilePos = map.GetCellCenterWorld(tilePos);
            TileBase tile = TileManager.Instance.prepMap.GetTile(tilePos);

            if (currentTeamPrep == Team.Allies && tile == prepAlly && countAllies != 0)
            {
                int calc = MapManager.currentMap.alliesTeamSize - countAllies;
                UnitManager.SpawnUnit(UnitSelection.allySelection[calc], worldTilePos, Team.Allies);
                map.SetTile(tilePos, null);
                countAllies--;
            }
            else if (currentTeamPrep == Team.Enemies && tile == prepEnemy && countEnemies != 0)
            {
                int calc = MapManager.currentMap.enemiesTeamSize - countEnemies;
                UnitManager.SpawnUnit(UnitSelection.enemySelection[calc], worldTilePos, Team.Enemies);
                map.SetTile(tilePos, null);
                countEnemies--;
            }
        }
        if (countAllies == 0)
        {
            if (startTeam == Team.Allies)
            {
                currentTeamPrep = Team.Enemies;
            }
            else
            {
                state = GameCycle.Play;
                UnitManager.OnPlay();
                OnGameStart();
            }
        }
        if (countEnemies == 0)
        {
            if (startTeam == Team.Enemies)
            {
                currentTeamPrep = Team.Allies;
            }
            else
            {
                state = GameCycle.Play;
                UnitManager.OnPlay();
                OnGameStart();
            }
        }
    }

    void Update_Play()
    {
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
                    MaskGenerator.Instance.GenerateMasks(currentUnit);
                    break;
                }
            }
        }

        //If current team cant act swap team or if round is over call RoundEnd()
        if (!UnitManager.TeamCanAct(currentTeam))
        {
            //If currentTeam equals start team, the other team needs to take their turns
            if (startTeam == Team.Allies && currentTeam == UnitManager.allies || startTeam == Team.Enemies && currentTeam == UnitManager.enemies)
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

    void Update_End()
    {
        SceneManager.LoadScene("mainmenu");
    }
}
