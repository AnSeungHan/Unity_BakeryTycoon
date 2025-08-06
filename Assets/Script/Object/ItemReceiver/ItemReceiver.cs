using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemReceiver : MonoBehaviour
{
    public virtual void StopMove(bool _IsStopMove)
    { 
        
    }

    public virtual bool StackItem(Item _Item)
    {
        return false;
    }

    public virtual Item ReleaseItem()
    {
        return null;
    }
}
