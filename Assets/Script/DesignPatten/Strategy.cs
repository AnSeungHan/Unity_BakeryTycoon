using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Strategy
{
    protected bool isCompleteTask = false;

    public bool IsCompleteTask { get { return isCompleteTask; } }

    public virtual bool BindTask()
    {
        // 전략 바인드용 : 바인드 시 네비 메쉬 경로 찾기 또는 상호작용 오브젝트에 상호작용 요청함

        return false;
    }

    public virtual bool CheckTaskCompletion()
    {
        // Task 완료 확인용

        return false;
    }
}

public abstract class UnitStrategy<T> : Strategy
    where T : Unit
{
    protected T             ownerUnit;
    protected Rigidbody     rigidbody;
    protected Animator      animator;

    public UnitStrategy(T _Owner)
    {
        ownerUnit = _Owner;
        rigidbody = _Owner.GetComponent<Rigidbody>();
        animator  = _Owner.GetComponent<Animator>();
    }
}