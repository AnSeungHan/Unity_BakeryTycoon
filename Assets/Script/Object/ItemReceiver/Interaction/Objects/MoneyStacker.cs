using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyStacker : MonoBehaviour
{
    private Collider            playerTriggerCollider;
    [SerializeField]
    private Money               stackItemPrefab;
    [SerializeField]
    private Transform           stackPos;
    [SerializeField]
    private int                 rowCount = 5;
    [SerializeField]
    private int                 colCount = 5;
    private LinkedList<Item>    stackItems = new LinkedList<Item>();
    private Collider            triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        triggerCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ("Player" != other.gameObject.tag || LayerMask.NameToLayer("Worker") != other.gameObject.layer)
            return;

        StartCoroutine(MoneyReleaseRoutin(other.gameObject.GetComponent<Player>()));

        if (0 == stackItems.Count)
            triggerCollider.enabled = false;
    }

    public void CreateItem(int _Count)
    {
        if (0 == _Count)
            return;

        for (int i = 0; i < _Count; ++i)
        {
            Vector3 offset = stackItemPrefab.OffsetVector3;
            int maxStack = rowCount * colCount;

            int row = stackItems.Count % rowCount;
            int col = Mathf.Max((stackItems.Count % maxStack) / colCount, 0);
            int hight = (stackItems.Count / maxStack);

            Vector3 pos
                = stackPos.position
                + new Vector3(row * offset.x, hight * offset.y, col * offset.z)
                - new Vector3(rowCount * offset.x * 0.5f, 0f, colCount * offset.z * 0.5f);

            Money newMoney = new Builder(stackItemPrefab.gameObject)
                .Set_Position(pos)
                .Set_Rotation(stackPos.rotation)
                //.Set_Parent(stackPos)
                .Bulild()
                .GetComponent<Money>();

            stackItems.AddLast(newMoney);
        }

        stackItemPrefab.StackSoundPlay();

        triggerCollider.enabled = true;
    }

    IEnumerator MoneyReleaseRoutin(Player _Player)
    {
        if (null == _Player)
            yield break;

        LinkedList<Item> copyItems = new LinkedList<Item>(stackItems);
        int MoneyCount = copyItems.Count;

        stackItemPrefab.ReleaseSoundPlay();

        LinkedListNode<Item> node = copyItems.Last;
        while (node != null)
        {
            node.Value.Stack(_Player, _Player.transform, new Vector3(0f, 2f, 0f), true);
            stackItems.Remove(node.Value);
            node = node.Previous;

            _Player.Add_Money(1);
            yield return null;
        }
    }
}
