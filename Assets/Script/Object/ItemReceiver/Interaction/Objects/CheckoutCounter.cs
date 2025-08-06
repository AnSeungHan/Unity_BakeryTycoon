using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckoutCounter : Interaction
{
    private LinkedList<AIUnit> taskStoreExecuteUnits = new LinkedList<AIUnit>();
    [SerializeField]
    private MoneyStacker       moneyStacker;
    [SerializeField]
    private PaperBag           paperBagePrefab;
    [SerializeField]
    private Transform[]        packagingPoss;
    private bool               isCanSell         = false;
    private bool               isTakeOutTaskWork = false;
    private bool               isTableTaskWork   = false;

    protected new void Awake()
    {
        base.Awake();

        GameManager.Instance.Regist_Interaction(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.NameToLayer("Worker") != other.gameObject.layer)
            return;

        if ("Player" == other.gameObject.tag)
            occupiedField.Action(true);

        isCanSell = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (LayerMask.NameToLayer("Worker") != other.gameObject.layer)
            return;

        if (!isTakeOutTaskWork)
            StartCoroutine(TakeOutProcessTaskRoutine());
    }

    private void OnTriggerExit(Collider other)
    {
        if (LayerMask.NameToLayer("Worker") != other.gameObject.layer)
            return;

        if ("Player" == other.gameObject.tag)
            occupiedField.Action(false);

        isCanSell = false;
    }

    public override bool Regist_MoveTaskRequest(string _InteractionTag, AIUnit _Requestor)
    {
        int action = _Requestor.ActionType;

        switch (action)
        {
            case 0: // take out
            {
                _Requestor.BindTaskTarget(taskExecutePoss[action], new Vector3(0f, 0f, taskExecuteUnits.Count), this);
                taskExecuteUnits.AddLast(_Requestor);
            }
            return true;

            case 1: // in store
            {
                _Requestor.BindTaskTarget(taskExecutePoss[action], new Vector3(0f, 0f, taskStoreExecuteUnits.Count), this);
                taskStoreExecuteUnits.AddLast(_Requestor);
            }
            return true;
        }    

        return false;
    }

    public override void Regist_ProcessTaskRequest(AIUnit _Requestor)
    {
        switch (_Requestor.ActionType)
        {
            case 0: // take out
            {
                if (!isTakeOutTaskWork || !isCanSell)
                    StartCoroutine(TakeOutProcessTaskRoutine());
            }
            break;

            case 1:
            {
                if (null == taskStoreExecuteUnits.Find(_Requestor))
                    taskStoreExecuteUnits.AddLast(_Requestor);                   

                // 만약 대기자가 없고 태이블도 사용 가능하면 즉시 실행 
                if (0 < taskStoreExecuteUnits.Count && GameManager.Instance.TableCheck())
                {
                    StartCoroutine(WaitTableTask());
                }

            }
            break;
        }
    }

    IEnumerator WaitTableTask()
    {
        yield return new WaitForSeconds(1f);

        taskStoreExecuteUnits.First.Value.CheckTaskCompletion();
        taskStoreExecuteUnits.RemoveFirst();
        StartCoroutine(InStoreProcessTaskRoutine());
    }

    IEnumerator TakeOutProcessTaskRoutine()
    {
        // 포장 루틴

        if (!isCanSell)
            yield break;

        isTakeOutTaskWork = true;

        if (null != taskExecuteUnits.First)
        {
            AIUnit customer = taskExecuteUnits.First.Value;

            if (!customer.IsMoveTask())
            {
                // 포장지에 빵 넣기
                PaperBag item_bag = new Builder(paperBagePrefab.gameObject)
                   .Set_Parent(packagingPoss[0])
                   .Set_Position(packagingPoss[0].position)
                   .Set_Rotation(packagingPoss[0].rotation)
                   .Bulild()
                   .GetComponent<PaperBag>();

                int breadCount = 0;
                int breadprice = 0;
                while (true)
                {
                    Item item_bread = customer.ReleaseItem();
                    if (null == item_bread)
                        break;

                    breadprice += item_bread.Price;
                    item_bread.Stack(this, item_bag.transform, new Vector3(0f, 0.2f, 0f), true);
                    ++breadCount;

                    yield return new WaitForSeconds(0.1f);
                }

                // 손님에게 포장지 주기  
                yield return new WaitForSeconds(1f);
                item_bag.Stack(customer, customer.StackPos, new Vector3(0f, 0f, 0f));
                customer.StackItem(item_bag);

                // 손님에게 돈 받기
                moneyStacker.CreateItem(breadprice);

                yield return new WaitForSeconds(1f);
                customer.CheckTaskCompletion();
                taskExecuteUnits.RemoveFirst();

                // 뒷 손님들 땡기기
                LinkedList<AIUnit> taskExecuteUnitsCopy = new LinkedList<AIUnit>(taskExecuteUnits);
                if (0 < taskExecuteUnitsCopy.Count)
                {
                    Unit    targetUnit = taskExecuteUnitsCopy.First.Value;
                    Vector3 targetPos  = taskExecutePoss[0].position;

                    foreach (AIUnit elem in taskExecuteUnitsCopy)
                    { 
                        Animator anim = elem.GetComponent<Animator>();
                        anim.SetBool("IsProccessMove", true);
                    }

                    while (true)
                    {
                        foreach (AIUnit elem in taskExecuteUnitsCopy)
                        {
                            elem.transform.position = Vector3.MoveTowards(elem.transform.position, targetPos, 3.5f * Time.deltaTime);
                        }

                        if (Vector3.Distance(targetUnit.transform.position, targetPos) < 0.1f)
                        {
                            foreach (AIUnit elem in taskExecuteUnitsCopy)
                            {
                                Animator anim = elem.GetComponent<Animator>();
                                anim.SetBool("IsProccessMove", false);
                            }

                            break;
                        }

                        yield return null;
                    }
                }
            }
        };

        isTakeOutTaskWork = false;
    }

    public bool TableCanUseReport()
    {
        // 테이블에게 사용 가능하다고 수신받아 동작하는 로직

        if (0 >= taskStoreExecuteUnits.Count)
            return false;

        LinkedListNode<AIUnit> iter = taskStoreExecuteUnits.First;
        do
        {
            if (null == iter || null == iter.Value)
                taskStoreExecuteUnits.Remove(iter);

            iter = iter.Next;

        } while (null != iter);

        if (0 >= taskStoreExecuteUnits.Count)
            return false;

        if (taskStoreExecuteUnits.First.Value.IsMoveTask())
            return false;

        taskStoreExecuteUnits.First.Value.CheckTaskCompletion();
        taskStoreExecuteUnits.RemoveFirst();

        StartCoroutine(InStoreProcessTaskRoutine());

        return true;
    }

    IEnumerator InStoreProcessTaskRoutine()
    {
        isTableTaskWork = true;

        // 태이블로 가려는 뒷 손님들 땡기기
        LinkedList<AIUnit> taskStoreExecuteUnitsCopy = new LinkedList<AIUnit>(taskStoreExecuteUnits);
        if (0 < taskStoreExecuteUnitsCopy.Count)
        {
            Unit    targetUnit = taskStoreExecuteUnitsCopy.First.Value;
            Vector3 targetPos  = taskExecutePoss[1].position;

            foreach (AIUnit elem in taskStoreExecuteUnitsCopy)
            {
                Animator anim = elem.GetComponent<Animator>();
                anim.SetBool("IsProccessMove", true);
            }

            while (true)
            {
                foreach (AIUnit elem in taskStoreExecuteUnitsCopy)
                {
                    elem.transform.position = Vector3.MoveTowards(elem.transform.position, targetPos, 3.5f * Time.deltaTime);
                }

                if (Vector3.Distance(targetUnit.transform.position, targetPos) < 0.1f)
                {
                    foreach (AIUnit elem in taskStoreExecuteUnitsCopy)
                    {
                        Animator anim = elem.GetComponent<Animator>();
                        anim.SetBool("IsProccessMove", false);
                    }

                    break;
                }

                yield return null;
            }
        }

        isTableTaskWork = false;
    }
}
