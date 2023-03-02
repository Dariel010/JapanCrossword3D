using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LocalSaveAndSettings
{
    public LevelData[] localLevelData;
    public int currentLevelPlay, showMistakesSeconds, soundLevel;
    public bool showBorderSettings;
    // COLOR KLETOK PLAYER
    public int[] colorBlackPlayer, colorOtherPlayer, colorNumber;
     
}