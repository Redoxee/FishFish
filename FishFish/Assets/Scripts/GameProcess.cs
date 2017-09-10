using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProcess : MonoBehaviour {


    [SerializeField]
    private GUIManager m_gui = null;

    private void Start()
    {
        Ini_Session();
        InitRound();
    }

    private void Update()
    {
        Update_Round();
    }

    public void OnGameButtonPressed(int index)
    {
        GameButtonPressed_Round(index);
    }

    #region Rounds

    public const int nbControls = 3;
    public float m_minWait = 1.5f;
    public float m_maxWait = 6.0f;
    private float m_roundTimer = 0;
    private float[] m_startStamps = new float[nbControls];
    private int[] m_roundMaxScores = new int[] { 100, 100, 100 };
    private float[] m_animDuration = new float[] { 1.1f, 1.1f, 1.1f };
    private int[] m_roundCurrentScores = new int[] { 0, 0, 0 };
    private bool[] m_isTapped = new bool[] { false, false, false };
    private int m_roundScore = 0;
    private bool m_hasCountDownStarted = true;
    
    private float m_roundDuration;

    public void InitRound()
    {
        m_roundTimer = 0;
        m_roundDuration = -1;
        for (int i = 0; i < m_startStamps.Length; ++i)
        {
            m_startStamps[i] = Random.Range(m_minWait, m_maxWait);
            m_gui.m_scores[i].text = (m_roundMaxScores[i].ToString());
            m_roundCurrentScores[i] = 0;
            m_isTapped[i] = false;
            m_roundDuration = Mathf.Max(m_roundDuration, m_startStamps[i] + m_animDuration[i]);
        }

        m_hasCountDownStarted = true;
        m_roundScore = 0;
        m_gui.m_roundScore.SetDisplay(0);
    }

    private void Update_Round()
    {
        if (m_hasCountDownStarted)
        {
            var prevTime = m_roundTimer;
            m_roundTimer += Time.deltaTime;
            for (int i = 0; i < m_startStamps.Length; ++i)
            {
                float stamp = m_startStamps[i];
                if (m_roundTimer >= stamp)
                {
                    if (prevTime < stamp)
                    {
                        //Start Animation
                    }
                    if (!m_isTapped[i])
                    {
                        float progression = Mathf.Clamp01((m_roundTimer - stamp) / m_animDuration[i]);
                        var points = Mathf.FloorToInt((1 - progression) * m_roundMaxScores[i]);
                        m_gui.m_scores[i].text = (points.ToString());
                    }
                }
            }

            if (IsRoundOver())
            {
                RoundEnd();
            }
        }
    }

    private int GetCurrentCatchPoint(int index)
    {
        if (!m_hasCountDownStarted)
            return 0;
        var stamp = m_startStamps[index];
        if (m_roundTimer < m_startStamps[index])
            return 0;
        var duration = m_animDuration[index];
        if (m_roundTimer >= m_startStamps[index] + m_animDuration[index])
            return 0;
        if (m_isTapped[index])
            return m_roundCurrentScores[index];
        return Mathf.FloorToInt((1 - ((m_roundTimer - stamp) / duration)) * m_roundMaxScores[index]);
    }

    private void GameButtonPressed_Round(int index)
    {
        if (!m_isTapped[index])
        {
            m_roundCurrentScores[index] = GetCurrentCatchPoint(index);
            m_isTapped[index] = true;
            m_gui.m_scores[index].text = (m_roundCurrentScores[index].ToString());
            m_roundScore += m_roundCurrentScores[index];
            m_gui.m_roundScore.SetNumber(m_roundScore);
        }
    }

    private bool IsRoundOver()
    {
        if (m_roundTimer >= m_roundDuration)
        {
            return true;
        }
        for (int i = 0; i < m_isTapped.Length; ++i)
        {
            if (!m_isTapped[i])
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    #region Session

    private int m_nbRoundPerSession = 3;
    private int[] m_sessionScores;
    private int m_currentRound;

    private void Ini_Session()
    {
        m_sessionScores = new int[m_nbRoundPerSession];
        for (int i = 0; i < m_nbRoundPerSession; ++i)
        {
            m_sessionScores[i] = 0;
        }
        m_currentRound = 0;
    }

    private void RoundEnd()
    {
        m_hasCountDownStarted = false;
        int score = 0;
        for (int i = 0; i < nbControls; ++i)
        {
            score += m_roundCurrentScores[i];
        }
        m_sessionScores[m_currentRound] = score;
        if (m_currentRound < m_nbRoundPerSession)
        {
            m_currentRound += 1;
        }
    }
    

    #endregion

    public void RestartButtonPressed()
    {
        Ini_Session();
        InitRound();
    }
}
