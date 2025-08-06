using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIUnit : Unit
{
    [SerializeField]
    protected int iFeildID = 0;

    private static int iID = 0;

    public int    taskCnt = 0;

    protected NavMeshAgent          navMeshAgent;
    protected LinkedList<Strategy>  reservedTasks = new LinkedList<Strategy>();
    private   Transform             destination;
    private   Vector3               offsetDestination;
    private   Interaction           interactionProp;
    protected int                   actiontype = 0;

    public Transform   Destination       { get { return destination;       } }
    public Vector3     OffsetDestination { get { return offsetDestination; } }
    public Interaction InteractionProp   { get { return interactionProp;   } }
    public int         ActionType        { get { return actiontype;        } }

    protected new void Awake()
    {
        base.Awake();

        navMeshAgent = GetComponent<NavMeshAgent>();

        iFeildID = ++iID;
    }

    protected void Start()
    {
        animator.SetBool("IsReady", true);
    }

    protected new void OnDisable()
    {
        base.OnDisable();

        animator.SetBool("IsHasTask", false);
    }

    protected void Update()
    {
        //base.Update();

        taskCnt = reservedTasks.Count;
    }

    public virtual void MakeRandom()
    { 
    
    }

    public override void Clear()
    {
        base.Clear();

        animator.SetBool("IsReady", false);
        reservedTasks.Clear();
        destination       = null;
        offsetDestination = Vector3.zero;
        interactionProp   = null;
    }

    public void BindTaskTarget(Transform _Destination, Interaction _InteractionProp)
    {
        destination       = _Destination;
        interactionProp   = _InteractionProp;
        offsetDestination = Vector3.zero;
    }

    public void BindTaskTarget(Transform _Destination, Vector3 _Offset, Interaction _InteractionProp)
    {
        destination       = _Destination;
        interactionProp   = _InteractionProp;
        offsetDestination = _Offset;
    }

    public void BindTaskOffset(Vector3 _Offset)
    {
        offsetDestination = _Offset;
    }

    // ========================= Àü·«¿ë =========================

    public void BindPriorityStrategy(Strategy _NewStrategy)
    {
        reservedTasks.AddFirst(_NewStrategy);
    }

    public void BindStrategy(Strategy _NewStrategy)
    {
        reservedTasks.AddLast(_NewStrategy);
    }

    public bool IsMoveTask()
    {
        return animator.GetBool("IsMove");
       /* return (animator)
            ? (animator.GetBool("IsMove"))
            : (false);*/
    }

    public bool CheckStrategy<T>() where T : Strategy
    {
        if (0 == reservedTasks.Count)
            return false;

        return (null != (reservedTasks.First.Value as T));
    }


    public bool CheckTaskCompletion()
    {
        if (0 == reservedTasks.Count)
        {
            Debug.Log("Err : CheckTaskCompletion => no reserved task");
            return true;
        }

        return reservedTasks.First.Value.CheckTaskCompletion();
    }

    public bool BindTask()
    {
        if (0 == reservedTasks.Count)
        {
            //Debug.Log("check : BindNextTask => no reserved task");

            return false;
        }

        return reservedTasks.First.Value.BindTask();
    }

    public void CompleteTask()
    {
        if (0 == reservedTasks.Count)
            Debug.Log("check : CompleteTask => no reserved task");

        Debug.Log($"Check [CheckTaskCompletion]: {iFeildID} / {reservedTasks.Count}");

        /*if (!reservedTasks.First.Value.IsCompleteTask)
            Debug.Log("Err : CompleteTask => Try Wrong CompleteTask ");*/

        /*if (!interactionProp)
            Debug.Log("Err : CompleteTask => is null interactionProp");*/

        reservedTasks.RemoveFirst();
    }
}