using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interaction : ItemReceiver
{
    protected Collider                      workerTriggerCollider;
    protected Dictionary<string, Transform> effectPoss;
    [SerializeField]
    protected Transform[]                   taskExecutePoss;
    protected bool[]                        taskOccupiedPoss;
    protected LinkedList<AIUnit>            taskExecuteUnits  = new LinkedList<AIUnit>(); 
    //protected LinkedList<AIUnit>            taskWaitUnits     = new LinkedList<AIUnit>(); // 이동에 실패한 유닛들
    [SerializeField]
    protected OccupiedField                 occupiedField;

    public int taskCnt = 0;
    public int waitCnt = 0;

    protected void Awake()
    {
        workerTriggerCollider = GetComponent<Collider>();

        Transform effectPosChild = transform.Find("EffectPos");

        if (null != effectPosChild && 0 < effectPosChild.childCount)
        {
            effectPoss = new Dictionary<string, Transform>();

            for (int i = 0; i < effectPosChild.childCount; ++i)
            {
                Transform effect = effectPosChild.GetChild(i);
                effect.gameObject.SetActive(false);
                effectPoss.Add(effect.name, effect);
            }
        }

        taskOccupiedPoss = new bool[taskExecutePoss.Length];
    }

    /*protected void Update()
    {
        taskCnt = taskExecuteUnits.Count;
        waitCnt = taskWaitUnits.Count;
    }*/

    /*protected void OnEnable()
    {

    }

    protected void Start()
    {

    }*/

    // =============================================

    protected Transform Check_IsVaildExecuteTaskPos()
    {
        if (null == taskExecutePoss)
            return null;

        for (int i = 0; i < taskExecutePoss.Length; ++i)
        {
            if (!taskOccupiedPoss[i])
            {
                taskOccupiedPoss[i] = true;
                return taskExecutePoss[i];
            }       
        }

        return null;
    }

    protected void ReleasTask(Transform _Destination)
    {
        for (int i = 0; i < taskExecutePoss.Length; ++i)
        {
            if (taskExecutePoss[i] == _Destination)
            { 
                taskOccupiedPoss[i] = false;
                return;            
            }
        }

        Debug.Log("Can found Occupied Pos");
    }

    public virtual bool Regist_MoveTaskRequest(string _InteractionTag, AIUnit _Requestor)
    {
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

    public virtual void Regist_ProcessTaskRequest(AIUnit _Requestor)
    {

    }

    public virtual void Report_MoveTaskRequest(AIUnit _Requestor)
    {
        //taskExecuteUnits.Remove(_Requestor);
    }

    public virtual void Unlock()
    { 
    
    }
}
