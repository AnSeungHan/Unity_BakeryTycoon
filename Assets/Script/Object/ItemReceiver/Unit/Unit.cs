using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : ItemReceiver
{
    protected new Rigidbody     rigidbody;
    [SerializeField]
    protected     Animator      animator;
    [SerializeField]
    protected     Transform     stackPos;
    [SerializeField]
    protected     int           maxItemStackCount = 5;
    protected     List<Item>    stackItems  = new List<Item>();

    public int          MaxItemStackCount { get { return maxItemStackCount; } }
    public int          CurItemStackCount { get { return stackItems.Count;  } }
    public Transform    StackPos          { get { return stackPos;  } }

    protected void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        stackItems.Capacity = maxItemStackCount;
    }

    /*protected void Update()
    {
        rigidbody.AddForce(Vector3.zero);
    }*/

    protected void OnDisable()
    {
        animator.SetBool("IsMove" , false);
        animator.SetBool("IsStack", false);
    }

    /*protected void OnEnable()
    {

    }

    protected void Start()
    {

    }*/

    // ====================================

    public virtual void Clear()
    {
        stackItems.Clear();

        foreach (Item elem in stackItems)
        {
            elem.transform.parent = null;
            ObjectPool.Instance.ReturnObject(elem.name, elem.gameObject);
        }
    }

    public override bool StackItem(Item _Item)
    {
        if (maxItemStackCount <= stackItems.Count)
            return false;

        _Item.Stack(this, stackPos, new Vector3(0f, stackItems.Count * _Item.StackOffsetY, 0f));
        stackItems.Add(_Item);

        if (0 < stackItems.Count)
            animator.SetBool("IsStack", true);

        return true;
    }

    public override Item ReleaseItem()
    {
        if (0 >= stackItems.Count)
            return null;

        int lastIdx = stackItems.Count - 1;
        Item removeItem = stackItems[lastIdx];
        stackItems.RemoveAt(lastIdx);

        if (null == removeItem)
            return null;

        if (0 == stackItems.Count)
            animator.SetBool("IsStack", false);

        return removeItem;
    }
}
