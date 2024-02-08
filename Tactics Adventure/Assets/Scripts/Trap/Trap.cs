using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using DG.Tweening;

public abstract class Trap : MonoBehaviour, IPoolObject
{
    public TrapData data;
    private int curZ;

    protected Animator anim;

    public virtual void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        anim = GetComponent<Animator>();
    }

    public virtual void OnGettingFromPool()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        curZ = 0;
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
        Vector3 target = new Vector3(0, 0, -(curZ + 90));
        transform.DOLocalRotate(target, 0.5f).SetEase(Ease.OutBack).OnComplete(() => transform.rotation = Quaternion.Euler(target));
        curZ = (curZ + 90) % 360;
    }
}

public enum TrapType { Spike1, Spike2, Spike3, Spike4, Flame, Suriken}