using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskInfo : MonoBehaviour
{
    [SerializeField]
    private GameObject[]    taskInfoObject = new GameObject[3];
    [SerializeField]
    private SpriteRenderer  normalSpriteRenderer;
    [SerializeField]
    private TextMeshPro     txtBreadCount;
    [SerializeField]
    private Sprite[]        taskImage = new Sprite[2];
    private TASK_TYPE       curtype   = TASK_TYPE.DISABLE;

    [SerializeField]
    private Vector3         offset = Vector3.up;
    private Customer        ownerCustomer;   

    public Customer  OwnerCustomer { set { ownerCustomer = value; } }
    public Vector3   Offset        { set { offset = value; } }


    private void Update()
    {
        if (null == ownerCustomer)
            return;

        transform.position
            = ownerCustomer.transform.position
            + offset;

        /*if (TASK_TYPE.BREAD == curtype)
        {
            int cnt = ownerCustomer.MaxItemStackCount - ownerCustomer.CurItemStackCount;
            txtBreadCount.text = ownerCustomer.MaxItemStackCount.ToString();

            if (0 == cnt)
                txtBreadCount.enabled = false;
        }*/
    }

    public void Set_TaskType(TASK_TYPE _TaskType)
    {
        if (!ownerCustomer)
            return;

        curtype = _TaskType;

        switch (_TaskType)
        {
            case TASK_TYPE.ENABLE:
            {
                enabled = false;
            }
            break;

            case TASK_TYPE.DISABLE:
            {
                enabled = false;
            }
            break;

            case TASK_TYPE.BREAD:
            {
                    enabled = true;
                    taskInfoObject[0].SetActive(false);
                    taskInfoObject[1].SetActive(true);
                    taskInfoObject[2].SetActive(false);

                    txtBreadCount.text = ownerCustomer.MaxItemStackCount.ToString();
            }
            break;

            case TASK_TYPE.TAKE_OUT:
            {
                    enabled = true;
                    taskInfoObject[0].SetActive(true);
                    taskInfoObject[1].SetActive(false);
                    taskInfoObject[2].SetActive(false);
                    normalSpriteRenderer.sprite = taskImage[0];
            }
            break;

            case TASK_TYPE.TABLE:
            {
                    enabled = true;
                    taskInfoObject[0].SetActive(true);
                    taskInfoObject[1].SetActive(false);
                    taskInfoObject[2].SetActive(false);
                    normalSpriteRenderer.sprite = taskImage[1];
            }
            break;

            case TASK_TYPE.HAPPY:
            {
                    enabled = true;
                    taskInfoObject[0].SetActive(false);
                    taskInfoObject[1].SetActive(false);
                    taskInfoObject[2].SetActive(true);
            }
            break;
        }
    }

    public void Set_BreadCount(int _MaxCnt, int _CurCnt)
    {
        if (TASK_TYPE.BREAD != curtype)
            return;

        int remainCnt = _MaxCnt - _CurCnt;
        txtBreadCount.text = remainCnt.ToString();
    }
}

public enum TASK_TYPE
{
    ENABLE,
    DISABLE,
    BREAD,
    TAKE_OUT,
    TABLE,
    HAPPY
}