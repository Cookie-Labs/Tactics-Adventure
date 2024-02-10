using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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
        dmg = Random.Range(1, 10); // ������
        targetDir = trap.data.targetDir; // Ÿ�� ����
        if (trap.data.isWait) // ��ٸ��� Ÿ�� Ʈ�� ����
            curWait = trap.data.wait;

        SetCardName(trap.data.name);
        SetUI(SetUIText());
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnTrap(trap);
        DODestroy();
    }

    public override void DoCard()
    {
        if (curWait == 0)
            Atk();

        spawnManager.playerCard.Move(pos);
    }

    public override void DoTurnCard()
    {
        trap.DORotate(); // Ʈ�� ��� ������

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

    public override void Damaged(int _amount)
    {
        ChangeDmg(Mathf.Max(0, dmg - _amount));

        if (dmg <= 0)
            Die();

        DODamaged();
    }

    public void ChangeDmg(int _amount)
    {
        dmg = _amount;

        SetUI(SetUIText());
    }

    public void Atk()
    {
        Card[] neighborCard = FindNeighbors(targetDir);

        foreach (Card card in neighborCard)
            card.Damaged(dmg);
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
            for (int i = 0; i < trap.data.wait - curWait; i++)
                s += "<sprite=6> ";
            s = s.TrimEnd();
        }
        else // Ʈ���� ȸ�� ���̶��
            s = $"<sprite=0>{dmg}";

        return s;
    }
}