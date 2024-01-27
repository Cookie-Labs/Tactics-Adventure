using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Trap : Card
{
    // 변수
    public int dmg;
    public int curWait; // isWait이라면 사용

    // 자식 컴포넌트
    private Trap trap;

    public override void OnCreatedInPool()
    {
        base.OnCreatedInPool();
    }

    public override void OnGettingFromPool()
    {
        base.OnGettingFromPool();
    }

    public override void SetCard()
    {
        trap = spawnManager.SpawnRanTrap(objTrans); // 트랩 소환

        // 변수 설정 (후에 난이도 설정)
        dmg = Random.Range(1, 10); // 데미지
        if (trap.data.isWait) // 기다리는 타입 트랩 설정
            curWait = trap.data.wait;

        SetCardName(trap.data.name);
        SetUI(SetUIText());
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnTrap(trap);
    }

    public string SetUIText()
    {
        string s = ""; // 빈 문자열 준비

        // 트랩이 기다리는 형이라면
        if (trap.data.isWait)
        {
            for (int i = 0; i < curWait; i++)
                s += "<sprite=4> ";
            for (int i = 0; i < trap.data.wait - curWait; i++)
                s += "<sprite=5> ";
            s = s.TrimEnd();
        }
        else // 트랩이 회전 형이라면
            s = $"<sprite=0>{dmg}";

        return s;
    }
}
