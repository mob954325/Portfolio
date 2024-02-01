using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    [Header("# Boss Info")]
    public GameObject _boss;
    public GameObject _bossMapBlock;
    [Tooltip("Check Trigger Object, The Game Object name is MUST BE EnterBossRoom")]
    public bool _isMeetBoss;
    public bool _isBossAlive;
    bool _isEnd;

    [Tooltip("check Obejectname : BossHealth")]
    public GameObject BossHpBar;

    [Header("# Panels")]
    public GameObject[] DialogPanel;
    public GameObject VictoryPanel;
    public GameObject DefeatPanel;
    public GameObject PausePanel;
    public GameObject GameOverPanel;

    //Dialog
    public bool _isActiveDialog;

    void Awake()
    {
        _boss = GameObject.Find("Boss").gameObject;
        BossHpBar = GameObject.Find("BossHealth").gameObject;

        _isBossAlive = false;
        _boss.SetActive(_isBossAlive);
    }

    void Start()
    {
        ShowDialog(0);
    }

    void Update()
    {
        CheckBossDead();
    }

    public void ShowBossHp()
    {
        ShowDialog(1);
        _bossMapBlock.SetActive(true);

        _isBossAlive = true;
        _boss.SetActive(_isBossAlive);
        BossHpBar.transform.GetChild(0).gameObject.SetActive(true);

        GameManager.instance.player._sound._audioSource[6].loop = true;
        GameManager.instance.player._sound._audioSource[6].Play();
    }

    public void ShowDialog(int i)
    {
        DialogPanel[i].SetActive(true);
        _isActiveDialog = true;
    }

    public void CloseDialog()
    {
        _isActiveDialog = false;
    }

    public void PauseActive(bool _isActive)
    {
        PausePanel.gameObject.SetActive(_isActive);
    }

    public bool isPauseActive()
    {
        return PausePanel.gameObject.activeSelf;
    }

    void CheckBossDead()
    {
        if (_isBossAlive)
        {
            ScriptControl _sc = DialogPanel[1].GetComponent<ScriptControl>();
            if (_sc.GetIndex() == 1)
            {
                GameManager.instance.playerCamera.Follow = _boss.gameObject.transform;
            }
            else
            {
                GameManager.instance.playerCamera.Follow = GameManager.instance.player.transform;
            }
        }
        else if(_isMeetBoss && !_isBossAlive && !_isEnd)
        {
            GameManager.instance.player.StopAllSound();
            ShowDialog(2);
            GameManager.instance.player._sound._audioSource[5].loop = true;
            GameManager.instance.player._sound._audioSource[5].Play();
            _isEnd = true;
        }
    }
}
