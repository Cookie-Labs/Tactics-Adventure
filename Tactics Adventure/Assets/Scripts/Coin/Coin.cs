using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class Coin : MonoBehaviour, IPoolObject
{
    public CoinType type;

    private Animator anim;

    private GameManager gameManager;

    public void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        anim = GetComponent<Animator>();
        gameManager = GameManager.Instance;
    }

    public void OnGettingFromPool()
    {
    }

    public void UpdateAnim(int money)
    {
        int typeID = (int)type; // �� Ÿ�� int������ ��ȯ

        // �� ������ �Ѿ�� ���
        if(money > gameManager.coinUnit[typeID])
        {
            type++; // ���� ����
            UpdateAnim(money); // �Լ� �����

            return;
        }

        anim.SetInteger("Change", typeID); // �ش� �� �ִϸ��̼� ����
    }
}

public enum CoinType { Bronze = 0, Silver, Gold }