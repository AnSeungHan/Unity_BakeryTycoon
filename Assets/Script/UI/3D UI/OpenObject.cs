using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OpenObject : ItemReceiver
{
    [SerializeField]
    private Item         payItemPrefab;
    [SerializeField]
    private TextMeshPro  costCountText;
    private Interaction  targetInteraction;
    [SerializeField]
    private int          unlockCost;
    private bool         isCollion = false;

    void Awake()
    {
        targetInteraction = transform.parent.root.GetComponent<Interaction>();
    }

    private void Start()
    {
        costCountText.text = unlockCost.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ("Player" != other.gameObject.tag)
            return;

        isCollion = true;

        StartCoroutine(PlayerPayRoutine(other.gameObject.GetComponent<Player>()));
    }

    private void OnTriggerExit(Collider other)
    {
        if ("Player" != other.gameObject.tag)
            return;

        isCollion = false;

        StopCoroutine(PlayerPayRoutine(other.gameObject.GetComponent<Player>()));
    }

    IEnumerator PlayerPayRoutine(Player _Player)
    {
        if (null == _Player)
            yield break; 

        for (int i = 0; i < _Player.Money; ++i)
        {
            Item payItem = new Builder(payItemPrefab.gameObject)
                .Set_Parent(null)
                .Set_Position(_Player.transform.position)
                .Set_EulerRotation(0f, 0f, 0f)
                .Bulild()
                .GetComponent<Item>();

            payItem.Stack(this, transform, Vector3.zero, true);

            --unlockCost;
            costCountText.text = unlockCost.ToString();

            _Player.Add_Money(-1);

            if (0 >= unlockCost || !isCollion)
                break;

            SoundManager.Instance.PlaySFX("PutObject");

            yield return new WaitForSeconds(0.1f);
        }

        if (0 >= unlockCost)
        {
            yield return new WaitForSeconds(0.5f);

            targetInteraction.Unlock();
        }
    }
}
