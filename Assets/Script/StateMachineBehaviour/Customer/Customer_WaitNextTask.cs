using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_WaitNextTask : UnitStateMachinBase<Customer>
{
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineEnter(animator, stateMachinePathHash);

        ownerUnit.BindTask();
    }
}
