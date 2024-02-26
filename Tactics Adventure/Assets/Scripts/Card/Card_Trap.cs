using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class Card_Trap : Card
{
    [Title("자식 변수")]
    public int dmg;
    public int curWait; // isWait이라면 사용
    public Direction[] targetDir;

    // 자식 컴포넌트
    private Trap trap;

    public override void SetCard()
    {
        trap = spawnManager.SpawnTrap_Ran(objTrans); // 트랩 소환

        // 변수 설정 (후에 난이도 설정)
        dmg = UnityEngine.Random.Range(1, 10); // 데미지
        targetDir = new Direction[trap.data.targetDir.Length];
        Array.Copy(trap.data.targetDir, targetDir, trap.data.targetDir.Length); // 타겟 방향
        if (trap.data.isWait) // 기다리는 타입 트랩 설정
            curWait = trap.waitCount;

        SetCardName(trap.data.name);
        SetUI(SetUIText());
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnTrap(trap);
        DODestroy();
    }

    public override IEnumerator DoCard()
    {
        if (curWait == 0)
            yield return Atk();

        yield return spawnManager.playerCard.Move(pos);
    }

    public override void DoTurnCard()
    {
        trap.DORotate();
        // 목표 타겟 바꾸기 (전 방향은 굳이 계산 x)
        int length = targetDir.Length;
        if (length < 4)
        {
            for(int i = 0; i < length; i++)
            {
                // 회전 진행
                targetDir[i]++;

                // 만약 L(enum 끝)이면 T(enum 첫)로 돌아오기
                if (targetDir[i] > Direction.L)
                    targetDir[i] = Direction.T;
            }
        }
    }

    public override IEnumerator Damaged(int _amount)
    {
        ChangeDmg(Mathf.Max(0, dmg - _amount));

        DODamaged();
        yield return new WaitForSeconds(0.1f);

        if (dmg <= 0)
        {
            Die();
        }
    }

    public void ChangeDmg(int _amount)
    {
        dmg = _amount;

        SetUI(SetUIText());
    }

    public IEnumerator Atk()
    {
        Card[] neighborCard = FindNeighbors(targetDir);

        SetAnim(trap.anim, AnimID.Atk);
        float maxAnim = animTime;

        foreach (Card card in neighborCard)
        {
            card.StartCoroutine(card.Damaged(dmg));
            maxAnim = Mathf.Max(maxAnim, card.animTime);
        }
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(maxAnim);
    }

    public void Die()
    {
        dmg = 0;
        spawnManager.ChangeCard(this, CardType.Empty);
    }

    public string SetUIText()
    {
        string s = ""; // 빈 문자열 준비

        // 트랩이 기다리는 형이라면
        if (trap.data.isWait)
        {
            for (int i = 0; i < curWait; i++)
                s += "<sprite=5> ";
            for (int i = 0; i < trap.waitCount - curWait; i++)
                s += "<sprite=6> ";
            s = s.TrimEnd();
        }
        else // 트랩이 회전 형이라면
            s = $"<sprite=0>{dmg}";

        return s;
    }
}