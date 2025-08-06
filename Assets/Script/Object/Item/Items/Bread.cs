using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bread : Item
{
    private float       moveSpeed = 1f;
    private Vector3     targetPos;
    private bool        isCanMove = false;

    public void MoveToBasket(Vector3 _TargetPos, float _MoveSpeed) 
    {
        isCanMove = true;
        targetPos = _TargetPos;
        moveSpeed = _MoveSpeed;

        StartCoroutine(MoveRoutine());
    }

    public override void Stack(ItemReceiver _Owner, Transform _StackPos, Vector3 _Offset, bool _IsDelete)
    {
        StopCoroutine(MoveRoutine());
        isCanMove = false;

        base.Stack(_Owner, _StackPos, _Offset, _IsDelete);
    }

    IEnumerator MoveRoutine()
    {
        while (true)
        {
            if (!isCanMove)
                yield break;

            Vector3 dist = targetPos - transform.position;

            if (1f > dist.magnitude)
                yield break;

            Vector3 nextMove  = dist.normalized * moveSpeed * Time.deltaTime;

            rigidbody.MovePosition(rigidbody.position + nextMove);
            rigidbody.AddForce(Vector3.zero);

            yield return null;
        }
    }
}
