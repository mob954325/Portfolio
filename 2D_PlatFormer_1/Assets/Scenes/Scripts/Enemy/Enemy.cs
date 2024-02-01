using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    public Action _onDie;

    [Header("#Enemy Stats")]
    public int _maxHp = 1;
    [SerializeField]private int _hp = 1;
    [SerializeField]private int _score = 10;
    public float _speed;
    public int _Hp
    {
        get => _hp;
        set 
        { 
            _hp = value; 
            if(_hp <= 0)
            {
                _hp = 0;
                OnDie();
            }
        }
    }

    [Header("#Effect")]
    public GameObject _deadEffect;
    protected ObjectSound _sound;

    protected virtual void Start()
    {
        Player player = FindAnyObjectByType<Player>();
        _sound = gameObject.GetComponent<ObjectSound>();
        _onDie += () => player.AddScore(_score);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("PlayerBullet"))
        {
            _Hp--;
        }
    }

    protected virtual void OnDie()
    {
        Instantiate(_deadEffect, transform.position, Quaternion.identity);
        _onDie?.Invoke();
        Destroy(gameObject);
    }
}
