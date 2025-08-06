using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : Interaction
{
    [SerializeField]
    private AIUnit[]  customerPrefabs;
    [SerializeField]
    private bool      isNoSpawn = false;
    [SerializeField]
    private int       makeCount = 0;

    protected new void Awake()
    {
        base.Awake();

        GameManager.Instance.Regist_Interaction(this);
    }

    protected void Start()
    {
        if (isNoSpawn)
            return;

        if (0 == makeCount)
            StartCoroutine(CreateUnitRoutine());
        else
            StartCoroutine(CreateUnitRoutine(makeCount));
    }

    public override bool Regist_MoveTaskRequest(string _InteractionTag, AIUnit _Requestor)
    {
        if (null == taskExecutePoss || 0 >= taskExecutePoss.Length)
            return false;

        int idx = Random.Range(0, taskExecutePoss.Length - 1);
        Transform vaildTaskPos = taskExecutePoss[idx];

        if (null == vaildTaskPos)
            return false;

        _Requestor.BindTaskTarget(vaildTaskPos, this);

        return true;
    }

    public void CreateCustomer()
    {
        StartCoroutine(CreateUnitRoutine_One());
    }

    IEnumerator CreateUnitRoutine()
    {
        yield return new WaitForSeconds(5f);

        while (true)
        { 
            if (!GameManager.Instance.IsCanCustomerMake())
            {
                yield break;
            }

            int   randomPrefabIdx  = Random.Range(0, customerPrefabs.Length - 1);
            int   randomExecuteIdx = Random.Range(0, taskExecutePoss.Length - 1);
            float randomDelaytime  = Random.Range(2f, 5f);

            AIUnit newUnit = new Builder(customerPrefabs[randomPrefabIdx].gameObject)
                .Set_Parent(null)
                .Set_Position(taskExecutePoss[randomExecuteIdx].position)
                .Bulild()
                .GetComponent<AIUnit>();

            newUnit.MakeRandom();

            yield return new WaitForSeconds(randomDelaytime);     
        }
    }

    IEnumerator CreateUnitRoutine(int _Count)
    {
        for (int i = 0; i < _Count; ++i)
        { 
            int   randomPrefabIdx  = Random.Range(0, customerPrefabs.Length - 1);
            int   randomExecuteIdx = Random.Range(0, taskExecutePoss.Length - 1);

            AIUnit newUnit = new Builder(customerPrefabs[randomPrefabIdx].gameObject)
               .Set_Parent(null)
               .Set_Position(taskExecutePoss[randomExecuteIdx].position)
               .Bulild()
               .GetComponent<AIUnit>();

            newUnit.MakeRandom();

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator CreateUnitRoutine_One()
    {
        if (!GameManager.Instance.IsCanCustomerMake())
        {
            yield break;
        }

        int     randomPrefabIdx  = Random.Range(0, customerPrefabs.Length - 1);
        int     randomExecuteIdx = Random.Range(0, taskExecutePoss.Length - 1);
        float   randomDelaytime  = Random.Range(1f, 2f);

        AIUnit newUnit = new Builder(customerPrefabs[randomPrefabIdx].gameObject)
            .Set_Parent(null)
            .Set_Position(taskExecutePoss[randomExecuteIdx].position)
            .Bulild()
            .GetComponent<AIUnit>();
        newUnit.MakeRandom();
    }
}
