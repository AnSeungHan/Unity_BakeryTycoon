using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_MoveTask : UnitStateMachinBase<Customer>
{
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineEnter(animator, stateMachinePathHash);

        ownerUnit.StartCoroutine(CheckMoveTaskCompletion());
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineExit(animator, stateMachinePathHash);

        ownerUnit.StopCoroutine(CheckMoveTaskCompletion());
    }

    /*public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        ownerUnit.CheckTaskCompletion();
    }*/

    IEnumerator CheckMoveTaskCompletion()
    {
        while (true)
        {
            if (ownerUnit.CheckTaskCompletion())
                break;

            yield return null;
        }
    }
}
