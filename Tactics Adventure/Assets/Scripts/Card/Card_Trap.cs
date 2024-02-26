using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class Card_Trap : Card
{
    [Title("�ڽ� ����")]
    public int dmg;
    public int curWait; // isWait�̶�� ���
    public Direction[] targetDir;

    // �ڽ� ������Ʈ
    private Trap trap;

    public override void SetCard()
    {
        trap = spawnManager.SpawnTrap_Ran(objTrans); // Ʈ�� ��ȯ

        // ���� ���� (�Ŀ� ���̵� ����)
        dmg = UnityEngine.Random.Range(1, 10); // ������
        targetDir = new Direction[trap.data.targetDir.Length];
        Array.Copy(trap.data.targetDir, targetDir, trap.data.targetDir.Length); // Ÿ�� ����
        if (trap.data.isWait) // ��ٸ��� Ÿ�� Ʈ�� ����
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
        // ��ǥ Ÿ�� �ٲٱ� (�� ������ ���� ��� x)
        int length = targetDir.Length;
        if (length < 4)
        {
            for(int i = 0; i < length; i++)
            {
                // ȸ�� ����
                targetDir[i]++;

                // ���� L(enum ��)�̸� T(enum ù)�� ���ƿ���
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
        string s = ""; // �� ���ڿ� �غ�

        // Ʈ���� ��ٸ��� ���̶��
        if (trap.data.isWait)
        {
            for (int i = 0; i < curWait; i++)
                s += "<sprite=5> ";
            for (int i = 0; i < trap.waitCount - curWait; i++)
                s += "<sprite=6> ";
            s = s.TrimEnd();
        }
        else // Ʈ���� ȸ�� ���̶��
            s = $"<sprite=0>{dmg}";

        return s;
    }
}