using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private bool        isNoSpawn = false;
    [SerializeField]
    private Unit[]      customerPrefabs;
    private Transform   createPos;
    private float       time;
    [SerializeField]
    private float       delaytime = 2f;

    private void Awake()
    {
        createPos = transform.Find("CreatePos");
    }

    private void Update()
    {
        if (isNoSpawn)
            return;

        time += Time.deltaTime;

        if (time < delaytime)
            return;

        time = 0f;

        delaytime   = Random.Range(2f, 10f);
        int randIdx = Random.Range(0, customerPrefabs.Length - 1);
        new Builder(customerPrefabs[randIdx].gameObject)
            .Set_Parent(null)
            .Set_Position(createPos.position)
            .Bulild();
    }
}
