using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEnterBossRoom : MonoBehaviour
{
    Collider coll;

    GameObject childWall;
    GameObject BlockWall;

    public GameObject Boss;
    public GameObject[] nomals;

    int cnt = 0; // check nomals disable

    void Start()
    {
        coll = GetComponent<Collider>();
        childWall = transform.GetChild(0).gameObject;
        BlockWall = transform.GetChild(1).gameObject;

        if(Boss == null)
        {
            Debug.LogError("���� ������Ʈ�� �������� �ʽ��ϴ�");
        }
    }

    void LateUpdate()
    {
        foreach(GameObject obj in nomals)
        {
            if (obj.activeSelf == false)
                cnt++;
        }

        if (cnt == nomals.Length)
            BlockWall.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {   
            // �÷��̾ �����ϸ� �� Ȱ��ȭ
            coll.enabled = false;
            childWall.SetActive(true);
            Boss.SetActive(true);
            GameManager.Instance.BattleBegin();
        }
    }
}
