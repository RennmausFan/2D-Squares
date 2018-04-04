using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsDisplayer : MonoBehaviour {

    public Unit unit;

    public static Vector3 offset = new Vector3(0, 0, 0);

    [SerializeField]
    private Text className;

    [SerializeField]
    private Healthbar healthBar;

    [SerializeField]
    private Slider moralBar;

    [SerializeField]
    private Text moralCount;

    [SerializeField]
    private Text statTurns;

    [SerializeField]
    private Text statAttacks;

    [SerializeField]
    private Text statAtk;

    [SerializeField]
    private Text statDef;

    public int moralMin, moralMax;

    //Custom Start method
	void Initialize() {

        className.text = unit.unitClass.ToString();

        statTurns.text = unit.turns.ToString();
        //Debuff
        if (unit.turns < unit.buffs.defaultTurns)
        {
            statTurns.color = Color.red;
        }
        //Buff
        else if (unit.turns > unit.buffs.defaultTurns)
        {
            statTurns.color = Color.green;
        }

        statAttacks.text = unit.attacks.ToString();
        //Debuff
        if (unit.attacks < unit.buffs.defaultAttacks)
        {
            statAttacks.color = Color.red;
        }
        //Buff
        else if (unit.attacks > unit.buffs.defaultAttacks)
        {
            statAttacks.color = Color.green;
        }

        statAtk.text = unit.atk.ToString();
        //Debuff
        if (unit.atk < unit.buffs.defaultAtk)
        {
            statAtk.color = Color.red;
        }
        //Buff
        else if (unit.atk > unit.buffs.defaultAtk)
        {
            statAtk.color = Color.green;
        }

        statDef.text = unit.def.ToString();
        //Debuff
        if (unit.def < unit.buffs.defaultDef)
        {
            statDef.color = Color.red;
        }
        //Buff
        else if (unit.def > unit.buffs.defaultDef)
        {
            statDef.color = Color.green;
        }

        healthBar.unit = unit;

        //Moral
        float moral = unit.moral;
        string text = "";
        if (moral > 0)
        {
            text += "+";
        }
        text += unit.moral.ToString();
        moralCount.text = text;

        float moralRange = Mathf.Abs(moralMin) + Mathf.Abs(moralMax);
        moralBar.value = 0.5f + (moral / moralRange);
    }

    public static GameObject NewStatsDisplayer(GameObject prefab, Unit pUnit)
    {
        GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        StatsDisplayer script = go.GetComponent<StatsDisplayer>();
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = pUnit.transform.position + offset;
        script.unit = pUnit;
        script.Initialize();
        return go;
    }
}
