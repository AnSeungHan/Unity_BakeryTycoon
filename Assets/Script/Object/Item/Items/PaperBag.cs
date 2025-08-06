using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperBag : Item
{
    private Animator    animator;

    protected new void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();
    }

    protected void Start()
    {
        animator.SetBool("IsWorkDone", false);
    }

    public override void Stack(ItemReceiver _Owner, Transform _StackPos, Vector3 _Offset, bool _IsDelete = false)
    {
        StartCoroutine(BagChaseRoutine(_Owner, _StackPos, _Offset, _IsDelete));
    }

    protected IEnumerator BagChaseRoutine(ItemReceiver _Owner, Transform _StackPos, Vector3 _Offset, bool _IsDelete)
    {
        animator.SetBool("IsWorkDone", true);

        yield return new WaitForSeconds(0.3f);

        Vector3 startPos     = transform.position;
        Vector3 controlPoint = (startPos + _StackPos.position + _Offset) / 2f + new Vector3(0f, 5f, 0f); 
        Vector3 endPos       = _StackPos.position + _Offset;

        float t = 0f;  // °î¼± ÁøÇà·ü

        while (true)
        {
            Vector3 currentPos = Vector3.Lerp
            (
                Vector3.Lerp(startPos    , controlPoint, t),
                Vector3.Lerp(controlPoint, endPos      , t),
                t
            );

            if (Vector3.Distance(currentPos, endPos) < 0.1f)
            {
                if (_IsDelete)
                {
                    DestroyImmediate(gameObject);
                    //ObjectPool.Instance.ReturnObject(name, gameObject);

                    yield break;
                }
                else
                { 
                    StartCoroutine(BagStackRoutine(_StackPos, _Offset.y));
                }

                yield break;
            }

            transform.position = currentPos;
            transform.rotation = Quaternion.Slerp(transform.rotation, _StackPos.rotation, 0.3f);

            t += Time.deltaTime * 3f;  // °î¼± ÁøÇà·ü Á¶Àý

            yield return null;
        }    
    }

    protected IEnumerator BagStackRoutine(Transform _StackPos, float _OffsetY)
    {
        yield return new WaitForSeconds(0.1f);      

        transform.parent   = _StackPos;
        transform.rotation = _StackPos.rotation;
        transform.position = _StackPos.transform.position + new Vector3(0f, _OffsetY, 0f);

    }
}
