using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    //-1 == Infinite
    public int timer = -1;

    public bool isStackable = false;

    public string name;

    public string description;

    public int atk, def;

    public int turns, attacks;

    //Buff-Presets

    public static Buff SmallMoralBuff = new Buff("Small Moral Buff", 1, 1, 0, 0, -1);
    public static Buff HugeMoralBuff = new Buff("Huge Moral Buff", 2, 2, 0, 0, -1);

    public static Buff SmallMoralDebuff = new Buff("Small Moral Debuff", -1, -1, 0, 0, -1);
    public static Buff HugeMoralDebuff = new Buff("Huge Moral Debuff", -2, -2, 0, 0, -1);

    public Buff(string pName, int pAtk, int pDef, int pTurns, int pAttacks, int pTimer)
    {
        name = pName;
        atk = pAtk;
        def = pDef;
        turns = pTurns;
        attacks = pAttacks;
        timer = pTimer;
    }
}

public class BuffManager{

    List<Buff> buffs = new List<Buff>();

    Unit owner;

    public int defaultAtk, defaultDef;

    public int defaultTurns, defaultAttacks;

    //Set the owner of the BuffManager
    public BuffManager(Unit pUnit)
    {
        owner = pUnit;

        //Get defaultValues
        defaultAtk = owner.atk;
        defaultDef = owner.def;
        defaultTurns = owner.turns;
        defaultAttacks = owner.attacks;
    }

    //Is triggered everytime a round ends
    public void OnRoundEnd()
    {
        foreach (Buff b in buffs)
        {
            if (b.timer == 0)
            {
                RemoveBuff(b);
            }
            b.timer -= 1;
        }
    }

    //Update the values of the owner
    public void ApplyBuffs()
    {
        //Set values to default
        owner.atk = defaultAtk;
        owner.def = defaultDef;
        owner.turns = defaultTurns;
        owner.attacks = defaultAttacks;

        //Apply each buff
        foreach (Buff b in buffs)
        {
            owner.atk += b.atk;
            owner.def += b.def;
            owner.turns += b.turns;
            owner.attacks += b.attacks;
        }
    }

    //Add a Buff (update stats)
    public void AddBuff(Buff pBuff)
    {
        if (!pBuff.isStackable)
        {
            if (buffs.Contains(pBuff))
            {
                return;
            }
        }
        buffs.Add(pBuff);
        ApplyBuffs();    
    }

    //Remove a buff (update stats)
    public void RemoveBuff(Buff pBuff)
    {
        buffs.Remove(pBuff);
        ApplyBuffs();
    }
}
