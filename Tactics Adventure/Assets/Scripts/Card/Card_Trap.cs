using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Trap : Card
{
    // ����
    public int dmg;
    public int curWait; // isWait�̶�� ���

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
        if (trap.data.isWait) // ��ٸ��� Ÿ�� Ʈ�� ����
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
