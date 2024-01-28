using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public CinemachineVirtualCamera playerCamera;

    ProgressManager _pManager;

    public ProgressManager pManager
    {
        get
        {
            if(_pManager == null)
            {
                _pManager = GameObject.Find("ProgressManager").GetComponent<ProgressManager>();
            }

            return _pManager;
        }
    }

    Player _player;
    public Player player
    {
        get
        {
            if(_player == null)
            {
                _player = FindAnyObjectByType<Player>();
            }
            return _player;
        }
    }

    void Awake()
    {
        instance = this;
        Time.timeScale = 1f;
    }
}
