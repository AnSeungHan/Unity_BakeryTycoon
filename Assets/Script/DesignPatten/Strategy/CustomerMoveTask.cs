using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerMoveTask : UnitStrategy<Customer>
{
    protected NavMeshAgent navMeshAgent;

    public CustomerMoveTask(Customer _Owner)
        : base(_Owner)
    {
        navMeshAgent = _Owner.GetComponent<NavMeshAgent>();
    }

    public override bool CheckTaskCompletion()
    {
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            isCompleteTask = true;

            navMeshAgent.isStopped = true;
            animator.SetBool("IsHasTask", false);
            animator.SetBool("IsMove"   , false);

            ownerUnit.CompleteTask();
            ownerUnit.StartCoroutine(TurnRoutine());

            return true;
        }

        return false;
    }

    protected IEnumerator TurnRoutine()
    {
        //yield return new WaitForSeconds(0.1f);

        while (true)
        {
            if (null == ownerUnit ||
                null == ownerUnit.Destination)
            {
                //yield break;

                yield return new WaitForSeconds(0.1f);

                continue;
            }

            Quaternion newRotation = Quaternion.Slerp
            (
                ownerUnit.transform.rotation,
                ownerUnit.Destination.rotation,
                0.3f
            );

            if (Mathf.Approximately(newRotation.x, ownerUnit.Destination.rotation.x) &&
                Mathf.Approximately(newRotation.y, ownerUnit.Destination.rotation.y) &&
                Mathf.Approximately(newRotation.z, ownerUnit.Destination.rotation.z) &&
                Mathf.Approximately(newRotation.w, ownerUnit.Destination.rotation.w))
                yield break;

            rigidbody.MoveRotation(newRotation);

            yield return null;
        }
    }
}

// =======================================================

public class CustomerMoveStand : CustomerMoveTask
{
    // 가판대로 이동 전략

    public CustomerMoveStand(Customer _Owner)
        : base(_Owner)
    { }

    public override bool BindTask()
    {
        if (GameManager.Instance.Request_MoveToStandTask(ownerUnit))
        {
            Transform destinationes = ownerUnit.Destination;

            if (null == destinationes)
            {
                Debug.Log("Err : CustomerMoveStand => Can not found Destination");
                return false;
            }

            if (!navMeshAgent.SetDestination(destinationes.transform.position))
            {
                Debug.Log("Err : CustomerMoveStand => Can not found navMeshPath");
                return false;
            }

            navMeshAgent.isStopped = false;
            animator.SetBool("IsHasTask", true);
            animator.SetBool("IsMove"   , true);

            return true;
        }

        return false;
    }
}

/*public class CustomerMoveOven : CustomerMoveTask
{
    // 가판대로 오븐 이동 전략(AI Work 전용)

    public CustomerMoveOven(Customer _Owner)
        : base(_Owner)
    { }

    public override bool BindTask()
    {
        if (GameManager.Instance.Request_MoveToOvenTask(ownerUnit))
        {
            Transform destinationes = ownerUnit.Destination;

            if (null == destinationes)
            {
                Debug.Log("Err : CustomerMoveOven => Can not found Destination");
                return false;
            }

            if (!navMeshAgent.SetDestination(destinationes.transform.position + ownerUnit.OffsetDestination))
            {
                Debug.Log("Err : CustomerMoveOven => Can not found navMeshPath");
                return false;
            }

            navMeshAgent.isStopped = false;
            animator.SetBool("IsHasTask", true);
            animator.SetBool("IsMove"   , true);

            return true;
        }

        return false;
    }
}*/

public class CustomerMoveCounter : CustomerMoveTask
{
    // 계산대 이동 전략

    public CustomerMoveCounter(Customer _Owner)
        : base(_Owner)
    { }

    public override bool BindTask()
    {
        // 네비 메쉬 경로가 있다면 Task 실행 준비

        if (GameManager.Instance.Request_MoveToCounterTask(ownerUnit))
        {
            Transform destinationes = ownerUnit.Destination;

            if (null == destinationes)
            {
                Debug.Log("Err : CustomerMoveOven => Can not found Destination");
                return false;
            }

            if (!navMeshAgent.SetDestination(destinationes.transform.position + ownerUnit.OffsetDestination))
            {
                Debug.Log("Err : CustomerMoveOven => Can not found navMeshPath");
                return false;
            }

            navMeshAgent.isStopped = false;
            animator.SetBool("IsHasTask", true);
            animator.SetBool("IsMove"   , true);

            return true;
        }

        return false;
    }

    public override bool CheckTaskCompletion()
    {
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            isCompleteTask = true;

            navMeshAgent.isStopped = true;
            animator.SetBool("IsHasTask", false);
            animator.SetBool("IsMove"   , false);

            ownerUnit.CompleteTask();
            ownerUnit.StartCoroutine(TurnRoutine()); // 도착지에서 선회(TaskExcutePos에 맞게 회전)

            return true;
        }

        return false;
    }
}

public class CustomerMoveTable : CustomerMoveTask
{
    // Table로 이동 잔략

    public CustomerMoveTable(Customer _Owner)
        : base(_Owner)
    { }

    public override bool BindTask()
    {
        ownerUnit.BindTaskInfo(TASK_TYPE.TABLE);

        if (GameManager.Instance.Request_MoveToTableTask(ownerUnit))
        {
            Transform destinationes = ownerUnit.Destination;

            if (null == destinationes)
            {
                Debug.Log("Err : CustomerMoveTable => Can not found Destination");
                return false;
            }

            if (!navMeshAgent.SetDestination(destinationes.transform.position + ownerUnit.OffsetDestination))
            {
                Debug.Log("Err : CustomerMoveTable => Can not found navMeshPath");
                return false;
            }

            navMeshAgent.isStopped = false;
            animator.SetBool("IsHasTask", true);
            animator.SetBool("IsMove", true);

            return true;
        }

        return false;
    }

     public override bool CheckTaskCompletion()
    {
        Vector3 pos = ownerUnit.Destination.position - ownerUnit.transform.position;
        if (pos.magnitude <= 0.3f)
        {
            isCompleteTask = true;

            navMeshAgent.isStopped = true;
            animator.SetBool("IsHasTask", false);
            animator.SetBool("IsMove"   , false);

            ownerUnit.CompleteTask();
            ownerUnit.StartCoroutine(TurnRoutine());

            return true;
        }

        return false;
    }
}


public class CustomerMoveEntrance : CustomerMoveTask
{
    public CustomerMoveEntrance(Customer _Owner)
        : base(_Owner)
    { }

    public override bool BindTask()
    {
        // TODO : Table에서 나와도 행복하겠네? 수정 필요할 듯
        ownerUnit.BindTaskInfo(TASK_TYPE.HAPPY);

        if (GameManager.Instance.Request_MoveToEntranceTask(ownerUnit))
        {
            Transform destinationes = ownerUnit.Destination;

            if (null == destinationes)
            {
                Debug.Log("Err : CustomerMoveOven => Can not found Destination");
                return false;
            }

            if (!navMeshAgent.SetDestination(destinationes.transform.position + ownerUnit.OffsetDestination))
            {
                Debug.Log("Err : CustomerMoveOven => Can not found navMeshPath");
                return false;
            }

            navMeshAgent.isStopped = false;
            animator.SetBool("IsHasTask", true);
            animator.SetBool("IsMove"   , true);

            return true;
        }

        return false;
    }

    public override bool CheckTaskCompletion()
    {
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            //Debug.Log("End Customer");

            isCompleteTask = true;

            navMeshAgent.isStopped = true;
            animator.SetBool("IsHasTask", false);
            animator.SetBool("IsMove"   , false);

            ownerUnit.CompleteTask();
            ownerUnit.Clear();

            return true;
        }

        return false;
    }
}