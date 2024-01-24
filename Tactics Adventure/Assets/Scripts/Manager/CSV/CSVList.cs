using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public struct RelicData
{
    public string name, explanation;
    public Tier tier;
    public bool isHave;
    public int index;
}

[Serializable]
public class CSVList
{
    public RelicData[] relicDatas;

    #region Relic
    public RelicData FindRelic(int index)
    {
        return relicDatas[index];
    }

    #endregion
}