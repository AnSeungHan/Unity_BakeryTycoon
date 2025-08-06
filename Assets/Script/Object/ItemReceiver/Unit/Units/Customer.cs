using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : AIUnit
{
    [SerializeField]
    private TaskInfo    taskInfo;

    public int itemCnt;

    private bool onceBool = true;

    protected new void Start()
    {
        base.Start();       

        if (taskInfo)
        { 
            taskInfo.transform.parent = null;
            taskInfo.OwnerCustomer    = this;
            taskInfo.gameObject.SetActive(true);
        }

        onceBool = true;
        GameManager.Instance.Report_CustomerCount(iFeildID, false);
        animator.SetBool("IsReady", true);
    }

    public override void Clear()
    {
        base.Clear();

        //Destroy(gameObject);

        if (!onceBool)
            return;

        onceBool = false;

        //ObjectPool.Instance.ReturnObject(name, gameObject);
        DestroyImmediate(gameObject);

        GameManager.Instance.Report_CustomerCount(iFeildID, true);
    }

    /*protected new void Update()
    {
        base.Update();

        itemCnt = stackItems.Count;
    }*/

    protected new void OnDisable()
    {
        base.OnDisable();

        if (taskInfo)
            taskInfo.gameObject.SetActive(false);    
    }

    public override void MakeRandom()
    {
        int tot     = GameManager.Instance.TotalCustomerCount;
        int randomA = tot % 10;

        if (0 == randomA)
        {
            actiontype = 1;

            BindStrategy(new CustomerPickUpBread(this));
            BindStrategy(new CustomerWaitTable(this));
            BindStrategy(new CustomerUseTable(this));
        }
        else
        {
            actiontype = 0;

            BindStrategy(new CustomerPickUpBread(this));
            BindStrategy(new CustomerTakeOut(this));       
        }

        Debug.Log($"Check [Task] : {reservedTasks.Count} / {actiontype} / {iFeildID}");
             
        maxItemStackCount = Random.Range(1, 4);
        animator.SetBool("IsReady", true);

        stackItems.Clear();

        if (taskInfo)
        { 
            taskInfo.transform.parent = null;
            taskInfo.OwnerCustomer    = this;
            taskInfo.gameObject.SetActive(true);
        }
    }

    public override void StopMove(bool _IsStopMove)
    {
        if (animator.GetBool("IsMove"))
            navMeshAgent.isStopped = true;
    }

    public override bool StackItem(Item _Item)
    {
        bool result = base.StackItem(_Item);

        if (taskInfo)
            taskInfo.Set_BreadCount(maxItemStackCount, stackItems.Count);

        if (result)
            _Item.StackSoundPlay();

        return result;
    }

    public override Item ReleaseItem()
    {
        Item result = base.ReleaseItem();

        if (null != result)
            result.ReleaseSoundPlay();

        return result;
    }

    public void BindTaskInfo(TASK_TYPE _TaskType)
    {
        taskInfo.Set_TaskType(_TaskType);
    }
}
