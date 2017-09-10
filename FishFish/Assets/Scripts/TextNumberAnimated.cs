using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextNumberAnimated : MonoBehaviour {
    private Text m_text;

    private float m_currentNumber = 0;
    private int m_displayedNumber = 0;
    private int m_targetNumber = 0;

    [SerializeField]
    private float m_speed = 15;

	void Awake () {
        m_text = GetComponent<Text>();
	}

    Action m_reachedAction = null;
    public Action ReachedAction { set { m_reachedAction = value; } }

    public void SetNumber(int target)
    {
        m_targetNumber = target;
        if (m_displayedNumber != target)
        {
            enabled = true;
        }
    }

    public void SetDisplay(int display)
    {
        m_targetNumber = display;
        m_currentNumber = display;
        m_displayedNumber = display;
        m_text.text = display.ToString();
    }

	void Update () {
        float delta = Time.deltaTime * m_speed * (m_currentNumber < m_targetNumber ? 1 : -1);
        if (delta < Mathf.Abs(m_currentNumber - m_targetNumber))
        {
            m_currentNumber += delta;
        }
        else
        {
            m_currentNumber = m_targetNumber;
        }

        int display = Mathf.FloorToInt(m_currentNumber);
        if (m_displayedNumber != display)
        {
            m_text.text = display.ToString();
            m_displayedNumber = display;
        }
        if (m_displayedNumber == m_targetNumber)
        {
            enabled = false;
            if (m_reachedAction != null)
                m_reachedAction();
        }
	}
}
