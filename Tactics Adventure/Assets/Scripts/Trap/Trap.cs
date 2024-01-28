using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using DG.Tweening;

public abstract class Trap : MonoBehaviour, IPoolObject
{
    public TrapData data;

    protected Animator anim;

    public virtual void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        anim = GetComponent<Animator>();
    }

    public virtual void OnGettingFromPool()
    {
    }

    public void SetTrap()
    {
        transform.parent.parent.GetComponent<Card>().SetTurnCard(); // 모든 트랩 턴제 카드 설정

        if (data.isWait)
            RanWait();
    }

    public void RanWait()
    {
        data.wait = Random.Range(2, 5); // 난이도 설정
    }

    public void DORotate()
    {
        Vector3 target = transform.eulerAngles + new Vector3(0, 0, 90f);
        transform.DOLocalRotate(target, 0.4f).SetEase(Ease.OutBack);
    }
}

public enum TrapType { Spike1, Spike2, Spike3, Spike4, Flame, Suriken}