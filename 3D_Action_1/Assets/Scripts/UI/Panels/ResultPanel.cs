using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultPanel : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI gradeText;

    char c;

    void Start()
    {

        timeText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        gradeText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        gameObject.SetActive(false);
    }

    public void SetResultText(bool isPlayer = false)
    {
        float resultTime = GameManager.Instance.timer;
        timeText.text = $"{(int)resultTime / 60}M {(int)resultTime % 60}S"; // �� : ��
        if(!isPlayer)
            gradeText.text = $"{SetGrade((int)resultTime)}";
        else if(isPlayer)
            gradeText.text = $"F";

    }

    char SetGrade(int timeSec)
    {
        // ���� ���� ����
        if (timeSec < 30)
        { 
            c = 'A';
        }
        else if (timeSec < 45)
        {
            c = 'B';
        }
        else if (timeSec < 60)
        {
            c = 'C';
        }
        else
        {
            c = 'D';
        }

        return c;
    }

    /// <summary>
    /// �г� Ȱ��ȭ
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }
}
