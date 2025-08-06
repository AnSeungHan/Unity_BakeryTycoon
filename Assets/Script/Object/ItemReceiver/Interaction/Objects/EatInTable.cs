using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EatInTable : Interaction
{
    private bool            isAtivate = false;
    [SerializeField]
    private Transform       stackItemPos;
    [SerializeField]
    private GameObject[]    activateViewProp;
    [SerializeField]
    private GameObject[]    disableViewProp;
    [SerializeField]
    private MoneyStacker    moneyStacker;
    [SerializeField]
    private GameObject      unlockEffectPrefab;
    [SerializeField]
    private Item            trashPrefab;

    private List<Item>      aboveTableitem = new List<Item>(3);
    private Item            aboveTableTrash;

    protected new void Awake()
    {
        base.Awake();

        GameManager.Instance.Regist_Interaction(this);
    }

    protected void Start()
    {
        foreach (GameObject elem in activateViewProp)
            elem.SetActive(false);

        workerTriggerCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.NameToLayer("Worker") != other.gameObject.layer)
            return;

        if (aboveTableTrash)
            StartCoroutine(TrashReleaseRoutin(other.gameObject.GetComponent<Unit>()));

        NotifyAvailble();
    }

    IEnumerator TrashReleaseRoutin(Unit _Unit)
    {
        if (null == _Unit)
            yield break;

        aboveTableTrash.Stack(_Unit, _Unit.transform, new Vector3(0f, 2f, 0f), true);
        aboveTableTrash.StackSoundPlay();
        taskOccupiedPoss[0] = false;
        aboveTableTrash = null;

        NotifyAvailble();
    }

    public bool CheckAvailable()
    {
        if (!isAtivate || aboveTableTrash)
            return false;

        if (taskOccupiedPoss[0])
            return false;

        return true;
    }

    public override bool Regist_MoveTaskRequest(string _InteractionTag, AIUnit _Requestor)
    {
        if (!isAtivate || aboveTableTrash)
        {
            GameManager.Instance.Add_WaitUnit(_InteractionTag, _Requestor);
            return false;
        }

        Transform vaildTaskPos = Check_IsVaildExecuteTaskPos();

        if (null == vaildTaskPos)
        {
            GameManager.Instance.Add_WaitUnit(_InteractionTag, _Requestor);
            return false;
        }

        taskExecuteUnits.AddLast(_Requestor);
        _Requestor.BindTaskTarget(vaildTaskPos, this);

        return true;
    }

    public override void Regist_ProcessTaskRequest(AIUnit _Requestor)
    {
        StartCoroutine(WaitSheetRoutin());
    }

    private void NotifyAvailble()
    {
        /*if (0 >= taskWaitUnits.Count)
        {
            GameManager.Instance.TableCanUseReport(this);      
        }

        // 대기 중인 자들 Task Bind    
         LinkedList<AIUnit> waitCopy = new LinkedList<AIUnit>(taskWaitUnits);
         if (0 < waitCopy.Count)
         {
            waitCopy.First.Value.BindTask();
            taskWaitUnits.RemoveFirst();
         }*/

        if (0 < taskExecuteUnits.Count)
            return;

        GameManager.Instance.TableCanUseReport(this);

        /*if (GameManager.Instance.Pull_WaitUnit
            (
                typeof(EatInTable).Name,
                1,
                out var list
            ))
        {
            taskExecuteUnits = list;

            StartCoroutine(WaitSheetRoutin());
        }*/
    }

    public override void Unlock()
    {
        isAtivate = true;

        new Builder(unlockEffectPrefab)
            .Set_Position(transform.position + new Vector3(0f, 2f, 0f))
            .Set_DeleteTime(4f)
            .Bulild();

        SoundManager.Instance.PlaySFX("OpenObject");

        foreach (GameObject elem in activateViewProp)
            elem.SetActive(true);

        foreach (GameObject elem in disableViewProp)
            elem.SetActive(false);

        workerTriggerCollider.enabled = true;

        NotifyAvailble();
    }

    IEnumerator WaitSheetRoutin()
    {
        if (0 == taskExecuteUnits.Count)
            yield break;

        if (null == taskExecuteUnits.First || null == taskExecuteUnits.First.Value)
        {
            taskExecuteUnits.RemoveFirst();
            yield break;
        }

        AIUnit unit = taskExecuteUnits.First.Value;
        if (unit.IsMoveTask())
            yield break;

        yield return new WaitForSeconds(1.5f);

        // 태이블에 아이템 올려두기
        for (int i = 0; i < unit.MaxItemStackCount; ++i)
        {
            Item item = unit.ReleaseItem();

            if (null == item)
                break;

            item.Stack(this, stackItemPos, new Vector3(0f, item.StackOffsetY * i, 0f));
            aboveTableitem.Add(item);

            yield return new WaitForSeconds(0.2f);
        }

        // 잠시 대기(식사)
        Animator     anim     = unit.GetComponent<Animator>();
        NavMeshAgent navAgent = unit.GetComponent<NavMeshAgent>();
        anim.SetBool("IsEat", true);
        navAgent.enabled = false;

        unit.transform.position = unit.transform.position + new Vector3(0f, 0.5f, 0f);
        int breadprice = 0;
        for (int i = aboveTableitem.Count - 1; i >= 0; --i)
        {
            yield return new WaitForSeconds(1.5f);

            breadprice += aboveTableitem[i].Price;
            ObjectPool.Instance.ReturnObject(aboveTableitem[i].name, aboveTableitem[i].gameObject);
            //Destroy(aboveTableitem[i].gameObject);
        }

        aboveTableitem.Clear();

        anim.SetBool("IsEat", false);
        navAgent.enabled = true;

        // 쓰레기 만들기
        if (null == trashPrefab.gameObject)
            Debug.Log("Err trashPrefab");

        GameObject obj = new Builder(trashPrefab.gameObject)
            .Set_Position(stackItemPos.position)
            .Bulild();


        if (null != obj)
            aboveTableTrash = obj.GetComponent<Item>();

        Transform dest = unit.Destination;

        unit.CheckTaskCompletion();
        ReleasTask(dest);
        taskExecuteUnits.RemoveFirst();

        // 돈 만들기
        moneyStacker.CreateItem(breadprice);

        // 대기 인원 불러 오기
        // NotifyAvailble();
    }
}
