using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick
    : MonoBehaviour
    , IBeginDragHandler
    , IDragHandler
    , IEndDragHandler
{
    [SerializeField]
    private RectTransform   clickLaver;
    [SerializeField]
    private RectTransform   laver;
    [SerializeField]
    private RectTransform   laverFram;

    [SerializeField, Range(10, 300)]
    private float   laverRange;
    private Vector2 inputDirection = Vector2.zero;

    public Vector2 InputDirection { get { return inputDirection; } }

    void Awake()
    { 
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");

        laver.anchoredPosition      = Vector2.zero;
        clickLaver.anchoredPosition = Vector2.zero;
        inputDirection              = Vector2.zero;
    }

    private void ControlJoyStickLever(PointerEventData eventData)
    { 
        Vector2 inputPos = eventData.position - laverFram.anchoredPosition;
        Vector2 inputVec = (inputPos.magnitude < laverRange) ? (inputPos) : (inputPos.normalized * laverRange);

        laver.anchoredPosition      = eventData.position;
        clickLaver.anchoredPosition = eventData.position;
        inputDirection              = inputVec / laverRange;
    }
}
