using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitFloor : Interaction
{
    /*[SerializeField]
    private int maxWaitCount = 3;
    private int curWaitCount = 0;*/

    [SerializeField]
    private bool isOccupied = false;

    public bool IsOccupied { get { return isOccupied; } }

    public void EnterWaitFloor()
    {
        isOccupied = true;
    }

    public void ExitWaitFloor()
    {
        isOccupied = false;
    }
}
