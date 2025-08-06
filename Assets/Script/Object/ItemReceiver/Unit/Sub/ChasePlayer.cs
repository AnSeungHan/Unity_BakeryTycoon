using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayer : MonoBehaviour
{
    [SerializeField]
    private Vector3 offsetPos;

    private void Awake()
    {
        GameManager.Instance.MainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        Transform target = GameManager.Instance.Player.transform;
        transform.position = new Vector3(target.position.x + offsetPos.x, transform.position.y, target.position.z + offsetPos.z);
    }
}
