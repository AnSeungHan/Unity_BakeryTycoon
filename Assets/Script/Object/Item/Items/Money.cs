using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : Item
{
    [SerializeField]
    private float stackOffsetX = 1f;
    [SerializeField]
    private float stackOffsetZ = 1f;

    public Vector3 OffsetVector3 { get { return new Vector3(stackOffsetX, stackOffsetY, stackOffsetZ); } }

    public override void Stack(ItemReceiver _Owner, Transform _StackPos, Vector3 _Offset, bool _IsDelete = false)
    {
        StartCoroutine(MoneyChaseRoutine(_Owner, _StackPos, _Offset, _IsDelete));
    }

    public override void StackSoundPlay()
    {
        SoundManager.Instance.PlaySFX("CustomerPayMoney");
    }

    public override void ReleaseSoundPlay()
    {
        SoundManager.Instance.PlaySFX("GetMoney");
    }

    private IEnumerator MoneyChaseRoutine(ItemReceiver _Owner, Transform _StackPos, Vector3 _Offset, bool _IsDelete)
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
                {
                    //Destroy(gameObject);
                    ObjectPool.Instance.ReturnObject(name, gameObject);
                }
                else
                    StartCoroutine(StackRoutine(_StackPos, _Offset.y));

                yield break;
            }

            transform.position = currentPos;
            transform.rotation = Quaternion.Slerp(transform.rotation, _StackPos.rotation * Quaternion.Euler(0f, 90f, 0f), 0.3f);

            t += Time.deltaTime * 3f;  // °î¼± ÁøÇà·ü Á¶Àý

            yield return null;
        }
    }
}
