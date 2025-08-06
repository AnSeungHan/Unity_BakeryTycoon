using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Strategy
{
    protected bool isCompleteTask = false;

    public bool IsCompleteTask { get { return isCompleteTask; } }

    public virtual bool BindTask()
    {
        // ���� ���ε�� : ���ε� �� �׺� �޽� ��� ã�� �Ǵ� ��ȣ�ۿ� ������Ʈ�� ��ȣ�ۿ� ��û��

        return false;
    }

    public virtual bool CheckTaskCompletion()
    {
        // Task �Ϸ� Ȯ�ο�

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