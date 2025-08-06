using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
    : Singleton<GameManager>
{
    [SerializeField]
    private int maxCustomerCount = 5;

    private HashSet<int> FeildCustomer = new HashSet<int>();

    private Player player;
    private Camera mainCamera;

    public Player Player { get { return player; } set { player = value; } }
    public Camera MainCamera { get { return mainCamera; } set { mainCamera = value; } }
    public int TotalCustomerCount { get { return FeildCustomer.Count; } }

    private Dictionary<string, Queue<AIUnit>> taskWaitUnits = new();

    private List<MerchandiseStand> stands = new List<MerchandiseStand>();
    private List<BreadOven> ovens = new List<BreadOven>();
    private List<CheckoutCounter> counters = new List<CheckoutCounter>();
    private List<EatInTable> tables = new List<EatInTable>();
    private List<Entrance> entrance = new List<Entrance>();

    private Dictionary<string, List<UIBase>> HUDs = new Dictionary<string, List<UIBase>>();

    public bool IsCanCustomerMake()
    {
        Debug.Log($"{FeildCustomer.Count} / {maxCustomerCount}");

        return (maxCustomerCount > FeildCustomer.Count);
    }

    public void Report_CustomerCount(int iID, bool bIsRemove)
    {
        if (bIsRemove)
        {
            if (!FeildCustomer.Contains(iID))
            {
                Debug.Log($"Err [{name}] : Fail Remove => <{iID}>");

                return;
            }

            FeildCustomer.Remove(iID);
            entrance[0].CreateCustomer();

            Debug.Log($"Success [{name}] : Remove => <{iID}>");
        }
        else
        {
            if (FeildCustomer.Contains(iID))
            {
                Debug.Log($"Err [{name}] : Fail Add => <{iID}>");
                return;
            }

            FeildCustomer.Add(iID);
            Debug.Log($"Success [{name}] : Add => <{FeildCustomer.Count}> <{iID}>");
        }
    }

    /*private IEnumerator CreateUnitWaitRoutine()
    {
        yield return new WaitForSeconds(5f);

        if (!IsCanCustomerMake())
            yield break;

        int iCnt = maxCustomerCount - FeildCustomer.Count;
        if (0 >= iCnt)
            yield break;

        entrance[0].CreateCustomer(1);
    }*/

    #region [Regist : Interaction]

    public void Regist_Interaction(MerchandiseStand _Stand)
    {
        stands.Add(_Stand);
    }

    public void Regist_Interaction(BreadOven _Oven)
    {
        ovens.Add(_Oven);
    }

    public void Regist_Interaction(CheckoutCounter _Counter)
    {
        counters.Add(_Counter);
    }

    public void Regist_Interaction(EatInTable _Table)
    {
        tables.Add(_Table);
    }

    public void Regist_Interaction(Entrance _Entrance)
    {
        entrance.Add(_Entrance);
    }

    public bool Request_MoveToStandTask(AIUnit _Requestor)
    {
        return MakeMoveTask<MerchandiseStand>
        (
            new List<MerchandiseStand>(stands),
            _Requestor
        );
    }

    public bool Request_MoveToOvenTask(AIUnit _Requestor)
    {
        return MakeMoveTask<BreadOven>
        (
            new List<BreadOven>(ovens),
            _Requestor
        );
    }

    public bool Request_MoveToCounterTask(AIUnit _Requestor)
    {
        return MakeMoveTask<CheckoutCounter>
        (
            new List<CheckoutCounter>(counters),
            _Requestor
        );
    }

    public bool Request_MoveToTableTask(AIUnit _Requestor)
    {
        return MakeMoveTask<EatInTable>
        (
            new List<EatInTable>(tables),
            _Requestor
        );
    }

    public bool Request_MoveToEntranceTask(AIUnit _Requestor)
    {
        if (0 == entrance.Count)
            return false;

        int idx = Random.Range(0, entrance.Count - 1);
        entrance[idx].Regist_MoveTaskRequest(typeof(Entrance).ToString(), _Requestor);

        return true;
    }

    #endregion

    #region [Task : Interaction]

    private bool MakeMoveTask<T>(List<T> _List, AIUnit _Requestor) where T : Interaction
    {
        _List.Sort((T a, T b) =>
        {
            float distanceA = Vector3.Distance(_Requestor.transform.position, a.transform.position);
            float distanceB = Vector3.Distance(_Requestor.transform.position, b.transform.position);

            return distanceA.CompareTo(distanceB);
        });

        foreach (T elem in _List)
        {
            if (elem.Regist_MoveTaskRequest(typeof(T).Name, _Requestor))
                return true;
        }

        return false;
    }

    public bool TableCheck()
    {
        foreach (EatInTable elem in tables)
        {
            if (elem.CheckAvailable())
                return true;
        }

        return false;
    }

    public void TableCanUseReport(EatInTable _Table)
    {
        foreach (CheckoutCounter elem in counters)
        {
            if (elem.TableCanUseReport())
                return;
        }
    }

    public void Add_WaitUnit(string _InteractionTag, AIUnit unit)
    {
        if (taskWaitUnits.TryGetValue
            (
                _InteractionTag,
                out var waits
            ))
        {
            waits.Enqueue(unit);
        }
        else
        {
            waits = new();
            waits.Enqueue(unit);
            taskWaitUnits.Add(_InteractionTag, waits);
        }
    }

    public bool Pull_WaitUnit(string Tag, int iCnt, out LinkedList<AIUnit> WaitList)
    {
        if (0 >= taskWaitUnits.Count ||
            !taskWaitUnits.TryGetValue(Tag, out var list))
        {
            WaitList = null;

            return false;
        }

        WaitList = new LinkedList<AIUnit>();
        for (int i = 0; i < iCnt; ++i)
        {
            if (0 >= list.Count)
                break;

            var unit = list.Dequeue();
            unit.BindTask();
            WaitList.AddLast(unit);
        }

        return true;
    }

    #endregion

    #region [Regist : HUD]

    public void Regist_HUD(string _UITag, UIBase _NewUI)
    {
        if (!HUDs.ContainsKey(_UITag))
            HUDs.Add(_UITag, new List<UIBase>());

        HUDs[_UITag].Add(_NewUI);
    }

    #endregion

    public void UpdateUI_HUD(string _UITag)
    {
        if (!HUDs.ContainsKey(_UITag))
            return;

        foreach (UIBase elem in HUDs[_UITag])
            elem.UIUpdate();
    }
}
