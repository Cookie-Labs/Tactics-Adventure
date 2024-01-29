using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Card_Trap : Card
{
    [Title("자식 변수")]
    public int dmg;
    public int curWait; // isWait이라면 사용
    public Direction[] targetDir;

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
        targetDir = trap.data.targetDir; // 타겟 방향
        if (trap.data.isWait) // 기다리는 타입 트랩 설정
            curWait = trap.data.wait;

        SetCardName(trap.data.name);
        SetUI(SetUIText());
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnTrap(trap);
    }

    public override void DoCard()
    {
        if (curWait == 0)
            Atk();

        spawnManager.PlayerCardMove(this);
    }

    public override void Anim(AnimID id)
    {
    }

    public override void DoTurnCard()
    {
        trap.DORotate(); // 트랩 모양 돌리기

        // 목표 타겟 바꾸기 (전 방향은 굳이 계산 x)
        int length = targetDir.Length;
        if (length < 4)
        {
            for(int i = 0; i < length; i++)
            {
                // 회전 진행
                targetDir[i]++;

                // 만약 R(enum 끝)이면 T(enum 첫)로 돌아오기
                if (targetDir[i] > Direction.R)
                    targetDir[i] = Direction.T;
            }
        }
    }

    public override void Damaged(int _amount)
    {
        dmg -= _amount;

        if(dmg <= 0)
        {
            dmg = 0;
            spawnManager.DeSpawnCard(this);
        }

        SetUIText();
    }

    public void Atk()
    {
        Card[] neighborCard = FindNeighbors(targetDir);

        foreach (Card card in neighborCard)
            card.Damaged(dmg);
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