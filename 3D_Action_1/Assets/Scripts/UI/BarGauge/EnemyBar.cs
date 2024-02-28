using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// EnemyBarObject�� �� ��ũ��Ʈ
/// </summary>
public class EnemyBar : BaseGauge
{
    // Enemy
    EnemyBase enemy;

    public GameObject currentEnemyHPObj;
    public GameObject currentEnemyToughnessObj;

    public Image currentEnemyHP;
    public Image currentEnemyToughness;

    public Transform hpChild;
    public Transform toughChild;


    protected override void Init()
    {
    }

    protected override void UpdateUI()
    {
        if (GameManager.Instance.GetIsBattle())
        {
            enemy = FindAnyObjectByType<EnemyBase>(); // enemy ��ũ��Ʈ�� �ִ� ������Ʈ ã��

            hpChild = transform.GetChild(0);
            toughChild = transform.GetChild(1);

            currentEnemyHPObj = hpChild.gameObject;
            currentEnemyToughnessObj = toughChild.gameObject;

            currentEnemyHP = hpChild.GetChild(1).GetComponent<Image>();
            currentEnemyToughness = toughChild.GetChild(1).GetComponent<Image>();


            if (enemy != null)
            {
                currentEnemyHPObj.SetActive(true);
                currentEnemyToughnessObj.SetActive(true);

                if (enemy.HP < 1) // ���� ����߰ų� ������
                {
                    gameObject.SetActive(false); // �����
                }

            }
            currentEnemyHP.fillAmount = enemy.HP / (float)enemy.maxHp;
            currentEnemyToughness.fillAmount = enemy.Toughness / (float)enemy.maxToughness;
        }
    }
}
