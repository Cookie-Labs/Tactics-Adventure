using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameManager : Singleton<GameManager>
{
    [Title("변수 (저장O)")]
    public PlayerType playerType;

    [Title("변수 (저장X)")]
    public Stage stage;
    public Level level;
}

public enum Stage { Grass = 0, Cave, Swarm, Forest }
public enum Level { Easy = 0, Normal, Hard }