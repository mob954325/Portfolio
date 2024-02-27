using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// UI���� ��ũ��Ʈ
/// </summary>
public class GameUIManager : Singleton<GameUIManager>
{
    [Header("Gauge UI")]
    PlayerBar playerBar;
    EnemyBar enemyBar;

    [Header("Panels")]
    ResultPanel resultPanel;
    [HideInInspector]public bool isPlayerInteraction = false;


    protected override void OnInitialize()
    {
        playerBar = FindAnyObjectByType<PlayerBar>();
        enemyBar = FindAnyObjectByType<EnemyBar>();

        resultPanel = FindAnyObjectByType<ResultPanel>();

        Cursor.lockState = CursorLockMode.None; // Ŀ�� ��������
        Cursor.visible = true; // Ŀ�� �����ֱ�
    }

    /// <summary>
    /// ��� �г��� �����ִ� �Լ�
    /// </summary>
    public void ShowResult(bool isPlayer = false)
    {
        Cursor.lockState = CursorLockMode.None; // Ŀ�� ��������
        Cursor.visible = true; // Ŀ�� �����ֱ�
        resultPanel.SetResultText(isPlayer); // ��������
        resultPanel.Show(); // �����ֱ�
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void EndGame()
    {
        SceneManager.LoadScene(0);
    }
}
