using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShotType {
    damageAndKnockback,
    damageOnly,
    knockbackOnly,
}

public enum WinCondition {
    goal,
    points,
    lastFishStanding,
}

public class GameModeManager : ScriptableObject {

    public static GameModeManager instance;

    public string gameModeName = "Default Game Mode";
    public ShotType shotType = ShotType.damageAndKnockback;
    public WinCondition winCondition = WinCondition.lastFishStanding;
    public int numberOfRounds = 5;

    public int minFishCount = 3;
    public int maxFishCount = 4;

    public bool teams = false;
    public bool autobalance = false;

    public int matchWaitSeconds = 3;

    public virtual bool CheckWinCondition () {
        switch (winCondition) {
            default:
                break;
            case WinCondition.goal:

                break;
            case WinCondition.lastFishStanding:

                break;
            case WinCondition.points:

                break;
        }

        return true;
    }
}
