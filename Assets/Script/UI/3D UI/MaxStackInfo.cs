using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxStackInfo : MonoBehaviour
{
    [SerializeField]
    private Vector3         offset       = Vector3.up;
    private float           stackOffsetY = 0f;
    private Unit            ownerUnit;   

    public Unit    OwnerUnit    { set { ownerUnit   = value; } }
    public Vector3 Offset       { set { offset      = value; } }
    public float   StackOffsetY 
    {
        get { return stackOffsetY; } 
        set 
        { 
            stackOffsetY = (5f < value) ? (5f) : (value);
            if (0 > stackOffsetY)
                stackOffsetY = 0;
        } }

    private void Update()
    {
        if (null == ownerUnit)
            return;

        transform.position
            = ownerUnit.transform.position
            + offset
            + new Vector3(0f, stackOffsetY, 0f);

        transform.rotation
            = Quaternion.LookRotation(GameManager.Instance.MainCamera.transform.position - transform.position)
            * Quaternion.Euler(0f, 180f, 0f);
    }
}
