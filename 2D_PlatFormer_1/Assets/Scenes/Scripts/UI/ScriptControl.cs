using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScriptControl : MonoBehaviour
{
    public TextMeshProUGUI _textComponent;
    public string[] _lines;
    public float _textSpeed;

    private int index;

    void Start()
    {
        _textComponent.text = string.Empty;
        StartDialogue();
    }

    void Update()
    { 
        if (Input.GetMouseButtonDown(0))
        {
            if (_textComponent.text == _lines[index])
            {
                nextLine();
            }
            else
            {
                StopAllCoroutines();
                _textComponent.text =_lines[index];
            }
            GameManager.instance.player._isClick = false;
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach(char c in _lines[index].ToCharArray())
        {
            _textComponent.text += c;
            yield return new WaitForSeconds(_textSpeed);
        }
    }

    void nextLine()
    {
        if(index < _lines.Length - 1)
        {
            index++;
            _textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
            GameManager.instance.pManager.CloseDialog();
        }
    }

    public int GetIndex()
    {
        return index;
    }
}
