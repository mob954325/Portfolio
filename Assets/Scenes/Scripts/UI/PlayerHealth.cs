using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private GameObject[] _playerHealthObject;
    Player _player;
    int _playerMaxHp;
    int _playerCurrentHp;


    void Awake()
    {
        _player = FindAnyObjectByType<Player>();  
    }

    void OnEnable()
    {
        // init value
        _playerCurrentHp = _player._Hp;
        _playerMaxHp = _player._maxHp;

        _playerHealthObject = new GameObject[_playerMaxHp];
        for(int i = 0; i < _playerMaxHp; i++)
        {
            _playerHealthObject[i] = transform.GetChild(i).transform.GetChild(0).gameObject;
        }
    }

    public void ChangeHealth()
    {
        // check
        _playerCurrentHp = _player._Hp;
        _playerMaxHp = _player._maxHp;

        int _changeHealthNum = _playerMaxHp - _playerCurrentHp;

        for (int i = _playerMaxHp - 1; i >= 3 - _playerCurrentHp; i--) // 2 1 0
        {
            _playerHealthObject[i].SetActive(true);
        }
        for(int i = 0; i < _changeHealthNum; i++)
        {
            // max - cur = ���� ��Ȱ��ȭ�� ü�� ��
            _playerHealthObject[i].SetActive(false);
        }

    }
}