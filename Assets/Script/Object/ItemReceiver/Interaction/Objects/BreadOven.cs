using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadOven : Interaction
{
    [SerializeField]
    protected Transform[]   outputPoss;
    [SerializeField]
    protected GameObject[]  breadPrefabs;
    private Transform       basketTransform;
    private List<Bread>     stackBreads        = new List<Bread>();
    [SerializeField, Range(0.3f, 10f)]
    private float           makeBreadTime      = 2f;
    [SerializeField, Range(1f, 10f)]
    private float           breadOutSpeed      = 1f;
    [SerializeField]
    private int             maxMakeBreadCount  = 10;
    private bool            isMakeMax          = false;
    private bool            isWork             = false;
    private bool            isTrigger          = false;

    protected new void Awake()
    {
        base.Awake();

        basketTransform = transform.Find("Basket");
        stackBreads.Capacity = maxMakeBreadCount;

        GameManager.Instance.Regist_Interaction(this);

        foreach (KeyValuePair<string, Transform> elem in effectPoss)
            elem.Value.gameObject.SetActive(true);
    }

    protected void Start()
    {
        Action();
    }

    private void OnDisble()
    {
        StopCoroutine(MakeBreadRoutine());
        //StopCoroutine(MakeEffectRoutine());
    }

    private void OnTriggerEnter(Collider other)
    {
        if ("Player" != other.gameObject.tag)
            return;

        // Player에게 빵 주기
        occupiedField.Action(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if ("Player" != other.gameObject.tag)
            return;

        // 빵 주기 캔슬
        occupiedField.Action(false);
    }


    private void OnTriggerStay(Collider other)
    {
        if (LayerMask.NameToLayer("Worker") != other.gameObject.layer)
            return;

        Unit unit = other.gameObject.GetComponent<Unit>();
        if (null == unit)
            return;

        switch (other.gameObject.tag)
        {
            case "Player":
            {
                if (!isTrigger)
                    StartCoroutine(StackItemRoutin(unit));
            }
            break;
        }
    }

    IEnumerator StackItemRoutin(Unit _TargetUnit)
    {
        isTrigger = true;
        //_TargetUnit.StopMove(true);

        for (int i = 0; i < stackBreads.Count; ++i)
        {
            // 내가 보유한 빵(stackBreads)을 대상(_TargetUnit)에게 주기

            if (_TargetUnit.StackItem(stackBreads[0]))
                stackBreads.RemoveAt(0);
            else
                break;       

            yield return new WaitForSeconds(0.1f);
        }

        if (!isWork && stackBreads.Count < maxMakeBreadCount)
            Action();

        isTrigger = false;
        //_TargetUnit.StopMove(false);
    }

    // =====================================

    private void Action()
    {
        StartCoroutine(MakeBreadRoutine());
        //StartCoroutine(MakeEffectRoutine());
    }

    IEnumerator MakeBreadRoutine()
    {
        isWork = true;

        while (true)
        {
            Bread bread = new Builder(breadPrefabs[0])
                .Set_Parent(null)
                .Set_Position(outputPoss[0].position)
                .Bulild()
                .GetComponent<Bread>();

            bread.Clear();
            bread.MoveToBasket(basketTransform.Find("EndPos").position, breadOutSpeed);
            stackBreads.Add(bread);

            if (stackBreads.Count >= maxMakeBreadCount)
            { 
                isMakeMax = true;
                break;
            }

            yield return new WaitForSeconds(makeBreadTime);
        }

        isWork = false;
    }

    /*IEnumerator MakeEffectRoutine()
    {
        while (true)
        {
            if (isMakeMax)
                yield break;

            effectPoss["VFX_Smoke"].gameObject.SetActive(true);
            effectPoss["VFX_Smoke"].GetComponent<ParticleSystem>().Play();

            yield return new WaitForSeconds(2f);

            effectPoss["VFX_Smoke"].gameObject.SetActive(false);

            yield return new WaitForSeconds(1f);
        }
    }*/
}
