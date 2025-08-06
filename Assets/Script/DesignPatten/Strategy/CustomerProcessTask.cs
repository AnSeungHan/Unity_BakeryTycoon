using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerProcessTask : UnitStrategy<Customer>
{
    public CustomerProcessTask(Customer _Owner)
        : base(_Owner)
    { }

    public override bool CheckTaskCompletion()
    {
        isCompleteTask = true;
        ownerUnit.CompleteTask();

        return true;
    }
}

// =======================================================

public class CustomerPickUpBread : CustomerProcessTask
{
    // 가판대에서 빵 stack

    public CustomerPickUpBread(Customer _Owner)
        : base(_Owner)
    { }

    public override bool BindTask()
    {       
        ownerUnit.BindTaskInfo(TASK_TYPE.BREAD);

        Interaction      prop  = ownerUnit.InteractionProp;
        MerchandiseStand stand = prop as MerchandiseStand;

        // 상호작용 대상이 가판대가 아니라면 가판대로 우선 이동
        if (!prop || !stand)
        {
            ownerUnit.BindPriorityStrategy(new CustomerMoveStand(ownerUnit));
            ownerUnit.BindTask();

            return false;
        }

        stand.Regist_ProcessTaskRequest(ownerUnit);
        animator.SetBool("IsHasTask", true);           

        return true;
    }

    public override bool CheckTaskCompletion()
    {
        if (ownerUnit.MaxItemStackCount <= ownerUnit.CurItemStackCount)
        {
            isCompleteTask = true;
            animator.SetBool("IsHasTask", false);

            ownerUnit.CompleteTask();

            return true;
        }

        return false;
    }
}

public class CustomerTakeOut : CustomerProcessTask
{
    public CustomerTakeOut(Customer _Owner)
        : base(_Owner)
    { }

    public override bool BindTask()
    {
        ownerUnit.BindTaskInfo(TASK_TYPE.TAKE_OUT);

        Interaction     prop    = ownerUnit.InteractionProp;
        CheckoutCounter counter = prop as CheckoutCounter;

        if (!prop || !counter)
        {
            ownerUnit.BindPriorityStrategy(new CustomerMoveCounter(ownerUnit));
            ownerUnit.BindTask();

            return false;
        }

        counter.Regist_ProcessTaskRequest(ownerUnit);
        animator.SetBool("IsHasTask", true);
        isCompleteTask = true;

        return true;
    }

    public override bool CheckTaskCompletion()
    {
        isCompleteTask = true;
        animator.SetBool("IsHasTask", false);
        animator.SetBool("IsStack"  , true);

        ownerUnit.CompleteTask();
        ownerUnit.BindPriorityStrategy(new CustomerMoveEntrance(ownerUnit));

        return true;
    }
}

public class CustomerWaitTable : CustomerProcessTask
{
    public CustomerWaitTable(Customer _Owner)
        : base(_Owner)
    { }

    public override bool BindTask()
    {
        ownerUnit.BindTaskInfo(TASK_TYPE.TABLE);

        Interaction     prop    = ownerUnit.InteractionProp;
        CheckoutCounter counter = prop as CheckoutCounter;

        if (!prop || !counter)
        {
            ownerUnit.BindPriorityStrategy(new CustomerMoveCounter(ownerUnit));
            ownerUnit.BindTask();

            return false;
        }

        counter.Regist_ProcessTaskRequest(ownerUnit);
        animator.SetBool("IsHasTask", true);
        isCompleteTask = true;

        return true;
    }

    public override bool CheckTaskCompletion()
    {
        isCompleteTask = true;
        animator.SetBool("IsHasTask", false);

        ownerUnit.CompleteTask();

        return true;
    }
}

public class CustomerUseTable : CustomerProcessTask
{
    public CustomerUseTable(Customer _Owner)
        : base(_Owner)
    { }

    public override bool BindTask()
    {
        ownerUnit.BindTaskInfo(TASK_TYPE.TABLE);

        Interaction prop  = ownerUnit.InteractionProp;
        EatInTable  table = prop as EatInTable;

        if (!prop || !table)
        {
            ownerUnit.BindPriorityStrategy(new CustomerMoveTable(ownerUnit));
            ownerUnit.BindTask();

            return false;
        }

        table.Regist_ProcessTaskRequest(ownerUnit);
        animator.SetBool("IsHasTask", true);
        isCompleteTask = true;

        return true;
    }

    public override bool CheckTaskCompletion()
    {
        isCompleteTask = true;
        ownerUnit.CompleteTask();
        animator.SetBool("IsHasTask", false);

        ownerUnit.BindPriorityStrategy(new CustomerMoveEntrance(ownerUnit));

        return true;
    }
}