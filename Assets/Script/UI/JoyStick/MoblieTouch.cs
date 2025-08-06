using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoblieTouch : MonoBehaviour
{
    [SerializeField]
    private RectTransform   laverFram;
    [SerializeField]
    private RectTransform   laver;
    [SerializeField]
    private RectTransform   clickLaver;

    [SerializeField, Range(10, 300)]
    private float   laverRange;
    private Vector2 inputDirection = Vector2.zero;

    public Vector2 InputDirection { get { return inputDirection; } }

    void Update()
    {
        SingleTouch();
    }

    private void SingleTouch()
    {
        if (0 >= Input.touchCount)
            return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
            {

            }
            break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
            {
                    //clickLaver.anchoredPosition = touch.position;

                    
                    clickLaver.anchoredPosition = Input.mousePosition;

                    Debug.Log(Input.mousePosition);

            }
            break;

            case TouchPhase.Ended:
            {
                    clickLaver.anchoredPosition = Vector2.zero;
            }
            break;
        }
    }
}
