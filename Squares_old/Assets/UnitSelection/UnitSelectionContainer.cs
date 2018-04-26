using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnitSelectionContainer : MonoBehaviour {

    public Unit unit;

    [SerializeField]
    private GameObject selection1, selection2;

    public Color selected, unselected;

    [SerializeField]
    private Animator anim;

    [SerializeField]
    private GameObject showText;

    [SerializeField]
    private GameObject showStats;

    [SerializeField]
    private Text describtion;

    [SerializeField]
    private Text className;

    [SerializeField]
    private Text statTurns;

    [SerializeField]
    private Text statAttacks;

    [SerializeField]
    private Text statAtk;

    [SerializeField]
    private Text statDef;

    [SerializeField]
    private Text statHealth;

    void Update()
    {
       if (UnitSelection.selectedContainer != this)
       {
            Undo_Highlight();
       }
    }

    public void Highlight()
    {
        selection1.GetComponent<Image>().color = selected;
        selection2.GetComponent<Image>().color = selected;
        UnitSelection.selectedContainer = this;
    }

    public void Undo_Highlight()
    {
        selection1.GetComponent<Image>().color = unselected;
        selection2.GetComponent<Image>().color = unselected;
    }

    void Awake()
    {
        statHealth.text = unit.maxHealth.ToString();
        statDef.text = unit.def.ToString();
        statAtk.text = unit.atk.ToString();
        statTurns.text = unit.turns.ToString();
        statAttacks.text = unit.attacks.ToString();
        className.text = unit.unitClass.ToString();
        describtion.text = unit.describtion;
    }

    public void HideText()
    {
        print("flipBAck");
        anim.Play("FlipCardBack");
    }

    public void HideStats()
    {
        print("flip");
        anim.Play("FlipCard");
    }
}
