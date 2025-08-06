using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    [SerializeField]
    private Joystick      joystick;
    [SerializeField]
    GameObject            maxStackUI;
    /*[SerializeField]
    private MaxStackInfo  maxStackInfo;*/
    [SerializeField]
    private float         moveSpeed = 3f;
    private bool          stopMove  = false;
    private int           money     = 100;

    Coroutine testRoutineA;
    Coroutine testRoutineB;

    public int Money { get { return money; } }

    protected new void Awake()
    {
        base.Awake();

        GameManager.Instance.Player = this;

        /*if (maxStackInfo)
        {
            maxStackInfo.transform.parent = null;
            maxStackInfo.OwnerUnit = this;
            maxStackInfo.gameObject.SetActive(false);
        }*/

        maxStackUI.SetActive(false);
    }

    protected void Start()
    {
        GameManager.Instance.UpdateUI_HUD("Money");
    }

    protected void Update()
    {
       // base.Update();

        if (Input.GetKeyDown(KeyCode.A))
        {
            testRoutineA = StartCoroutine(TestRoutine("1"));
            testRoutineB = StartCoroutine(TestRoutine("2"));
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            StopCoroutine(testRoutineA);
            StopCoroutine(testRoutineB);
        }
    }

    IEnumerator TestRoutine(string Input)
    {
        Debug.Log(Input + " Start");

        yield return new WaitForSeconds(3f);

        Debug.Log(Input + " End");
    }

    private void FixedUpdate()
    {
        if (stopMove)
            return;

        Vector3 input = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);

        if (Vector3.zero == input)
        {
            animator.SetBool("IsMove", false);
            return;
        }

        Vector3 moveVec = input * moveSpeed * Time.deltaTime;
        rigidbody.MovePosition(rigidbody.position + moveVec);

        if (Vector3.zero == moveVec)
        {
            animator.SetBool("IsMove", false);
            return;
        }
        else
        {
            animator.SetBool("IsMove", true);
            rigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveVec), 0.3f));
        }
    }

    public override void StopMove(bool _IsStopMove)
    {
        if (animator.GetBool("IsMove"))
            animator.SetBool("IsMove", false);

        stopMove = _IsStopMove;
    }

    public override bool StackItem(Item _Item)
    {
        bool result = base.StackItem(_Item);

        if (result)
        { 
            _Item.StackSoundPlay();
        
            if (CurItemStackCount >= MaxItemStackCount)
                maxStackUI.SetActive(true);
        }

        return result;
    }

    public override Item ReleaseItem()
    {
        Item result = base.ReleaseItem();

        if (null != result)
        {
            result.ReleaseSoundPlay();
            maxStackUI.SetActive(false);
        }

        return result;
    }

    public void Add_Money(int _AddMoneyCount)
    {
        money += _AddMoneyCount;

        GameManager.Instance.UpdateUI_HUD("Money");
    }
}
