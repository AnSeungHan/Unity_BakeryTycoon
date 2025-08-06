using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchandiseStand : Interaction
{
    private Transform       itemListPos;
    private List<Transform> stackItemPoss = new List<Transform>();
    private List<Item>      items         = new List<Item>();

    [SerializeField]
    private int             maxItemCount    = 24;
    private bool            isWork          = false;

    protected new void Awake()
    {
        base.Awake();

        itemListPos = transform.Find("ItemList");

        Transform stackPosChild = transform.Find("StackPos");
        if (null != stackPosChild && 0 < stackPosChild.childCount)
        {
            for (int i = 0; i < stackPosChild.childCount; ++i)
                stackItemPoss.Add(stackPosChild.GetChild(i));
        }

        GameManager.Instance.Regist_Interaction(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.NameToLayer("Worker") != other.gameObject.layer)
            return;

        if ("Player" == other.gameObject.tag)
            occupiedField.Action(true);

        Unit unit = other.gameObject.GetComponent<Unit>();
        if (null == unit)
            return;

        if (0 == stackItemPoss.Count)
            return;

        switch (other.gameObject.tag)
        {
            case "Player":
            {
                if (null == itemListPos || 0 >= stackItemPoss.Count)
                    return;

                if (0 < unit.CurItemStackCount)
                    StartCoroutine(ReleaseItemRoutin(unit));
            }
            break;
        }     
    }

    private void OnTriggerExit(Collider other)
    {
        if("Player" == other.gameObject.tag)
            occupiedField.Action(false);
    }

    public override void Regist_ProcessTaskRequest(AIUnit _Requestor)
    {
        if (!isWork)
            StartCoroutine(ProcessTask_PickUpItemRoutine());
    }

    IEnumerator ReleaseItemRoutin(Unit _TargetUnit)
    {
        // player 아이템 stand에 올려두기
        _TargetUnit.StopMove(true);

        while (items.Count < maxItemCount)
        {
            Item newItem = _TargetUnit.ReleaseItem();

            if (null == newItem)
                break;

            int       curIdx   = items.Count % stackItemPoss.Count;
            int       hight    = items.Count / stackItemPoss.Count;
            Transform stackPos = stackItemPoss[curIdx];

            items.Add(newItem);

            newItem.Stack(this, stackPos, new Vector3(0f, newItem.StackOffsetY * hight, 0f));

            yield return new WaitForSeconds(0.1f);
        }

        _TargetUnit.StopMove(false);

        // stand 아이템 customer에게 나눠주기
        if (!isWork)
            StartCoroutine(ProcessTask_PickUpItemRoutine());
    }

    IEnumerator ProcessTask_PickUpItemRoutine()
    {
        if (0 == items.Count)
            yield break;

        isWork = true;

        yield return new WaitForSeconds(0.1f);

        // 준비 완료된 Customer에게 item 주기
        LinkedList<AIUnit> units = new LinkedList<AIUnit>(taskExecuteUnits);
        int completCnt = 0;
        foreach (AIUnit elem in units)
        {
            if (elem.IsMoveTask())
                continue;

            for (int i = 0; i < items.Count; ++i)
            {
                int lastIdx = items.Count - 1;

                if (elem.StackItem(items[lastIdx]))
                    items.RemoveAt(lastIdx);

                yield return new WaitForSeconds(0.05f);
            }

            Transform dest = elem.Destination;
            if (elem.CheckTaskCompletion())
            {
                ReleasTask(dest);
                taskExecuteUnits.Remove(elem);
                ++completCnt;
            }

            if (0 == items.Count)
                 break;
        }

        // 대기 중인 자들 Task Bind    
        isWork = false;

        if (GameManager.Instance.Pull_WaitUnit
            (
                typeof(MerchandiseStand).Name,
                completCnt,
                out var newList
            ))
        {
            taskExecuteUnits = newList;

            StartCoroutine(ProcessTask_PickUpItemRoutine());
        }
    }
}
