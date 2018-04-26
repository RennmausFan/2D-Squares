using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public static bool showAttackPattern;
    public static bool showDangerZone;

    public void EndTurn()
    {
        RoundManager.Instance.EndTurn();
    }

    public void ShowAttackPattern(bool show)
    {
        if (RoundManager.state != GameCycle.Play)
        {
            return;
        }
        showAttackPattern = show;
        MaskGenerator.Instance.GenerateMasks(RoundManager.currentUnit);
    }

    public void ShowDangerZone(bool show)
    {
        if (RoundManager.state != GameCycle.Play)
        {
            return;
        }
        showDangerZone = show;
        if (show)
        {
            MaskGenerator.Instance.GenerateDangerZone();
        }
        else
        {
            TileManager.Instance.purpleMaskMap.ClearAllTiles();
        }
    }
}
