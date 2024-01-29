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
        trap = spawnManager.SpawnRanTrap(objTrans); // Ʈ�� ��ȯ

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
        trap.DORotate(); // Ʈ�� ��� ������

        // ��ǥ Ÿ�� �ٲٱ� (�� ������ ���� ��� x)
        int length = targetDir.Length;
        if (length < 4)
        {
            for(int i = 0; i < length; i++)
            {
                // ȸ�� ����
                targetDir[i]++;

                // ���� R(enum ��)�̸� T(enum ù)�� ���ƿ���
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
        string s = ""; // �� ���ڿ� �غ�

        // Ʈ���� ��ٸ��� ���̶��
        if (trap.data.isWait)
        {
            for (int i = 0; i < curWait; i++)
                s += "<sprite=4> ";
            for (int i = 0; i < trap.data.wait - curWait; i++)
                s += "<sprite=5> ";
            s = s.TrimEnd();
        }
        else // Ʈ���� ȸ�� ���̶��
            s = $"<sprite=0>{dmg}";

        return s;
    }
}