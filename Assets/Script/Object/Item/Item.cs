using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected new Rigidbody     rigidbody;
    protected Collider          crashCollider;
    [SerializeField]
    protected float             stackOffsetY;
    [SerializeField]
    private int                 price = 3;

    public float StackOffsetY { get { return stackOffsetY; } }
    public int   Price        { get { return price; } }

    protected void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        crashCollider = GetComponent<Collider>();
    }

    // =====================================

    public virtual void Clear()
    {
        if (crashCollider)
            crashCollider.enabled = true;

        if (rigidbody)
        { 
            rigidbody.isKinematic = false;
            rigidbody.useGravity  = true;
        }
    }

    public virtual void Stack(ItemReceiver _Owner, Transform _StackPos, Vector3 _Offset, bool _IsDelete = false)
    { 
        rigidbody.isKinematic = true;
        rigidbody.useGravity  = false;
        rigidbody.AddForce(Vector3.zero);
        crashCollider.enabled = false;

        StartCoroutine(ChaseRoutine(_Owner, _StackPos, _Offset, _IsDelete));
    }

    public virtual void StackSoundPlay()
    {
        SoundManager.Instance.PlaySFX("GetObject");
    }

    public virtual void ReleaseSoundPlay()
    {
        SoundManager.Instance.PlaySFX("PutObject");
    }

    protected IEnumerator ChaseRoutine(ItemReceiver _Owner, Transform _StackPos, Vector3 _Offset, bool _IsDelete)
    {
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
                    ObjectPool.Instance.ReturnObject(name, gameObject);
                else
                    StartCoroutine(StackRoutine(_StackPos, _Offset.y));

                yield break;
            }

            rigidbody.MovePosition(currentPos);
            rigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, _StackPos.rotation * Quaternion.Euler(0f, 90f, 0f), 0.3f));

            t += Time.deltaTime * 3f;  // °î¼± ÁøÇà·ü Á¶Àý

            yield return null;
        }
    }

    protected IEnumerator StackRoutine(Transform _StackPos, float _OffsetY)
    {
        rigidbody.AddForce(Vector3.zero);

        yield return new WaitForSeconds(0.1f);      

        transform.parent   = _StackPos;
        transform.rotation = _StackPos.rotation * Quaternion.Euler(0f, 90f, 0f);
        transform.position = _StackPos.transform.position + new Vector3(0f, _OffsetY, 0f);
    }
}
