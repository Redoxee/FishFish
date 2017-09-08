using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProcess : MonoBehaviour {

    public const int nbControls = 3;

    public float m_minWait = 1.5f;
    public float m_maxWait = 6.0f;
    [SerializeField]
    private GUIManager m_gui = null;

    float m_timer = 0;
    float[] m_startStamps = new float[nbControls];
    int[] m_scores = new int[]{ 10,10,10};
    float[] m_animDuration = new float[] { 1.1f, 1.1f, 1.1f };
    int[] m_catchPoints = new int[] { 0, 0, 0 };
    
    private bool m_hasCountDownStarted = true;


    private void Start()
    {
        Init();
    }

    public void Init()
    {
        for (int i = 0; i < m_startStamps.Length; ++i)
        {
            m_startStamps[i] = Random.Range(m_minWait, m_maxWait);
            m_gui.m_scores[i].SetText(m_scores[i].ToString());
            m_catchPoints[i] = 0;
        }
    }

    private void Update()
    {
        if (m_hasCountDownStarted)
        {
            var prevTime = m_timer;
            m_timer += Time.deltaTime;
            for (int i = 0; i < m_startStamps.Length; ++i)
            {
                float stamp = m_startStamps[i];
                if (m_timer >= stamp)
                {
                    if (prevTime < stamp)
                    {
                        //Start Animation
                    }
                    if (m_catchPoints[i] == 0)
                    {
                        float progression = Mathf.Clamp01((m_timer - stamp) / m_animDuration[i]);
                        var points = Mathf.FloorToInt((1 - progression) * m_scores[i]);
                        m_gui.m_scores[i].SetText(points.ToString());
                    }
                }
            }
        }
    }

    private int GetCurrentCatchPoint(int index)
    {
        if (!m_hasCountDownStarted)
            return 0;
        var stamp = m_startStamps[index];
        if (m_timer < m_startStamps[index])
            return -1;
        var duration = m_animDuration[index];
        if (m_timer >= m_startStamps[index] + m_animDuration[index])
            return 0;
        if (m_catchPoints[index] != 0)
            return m_catchPoints[index];
        return Mathf.FloorToInt((1 - ((m_timer - stamp) / duration)) * m_scores[index]);
    }
    public bool IsTapped(int index)
    {
        return m_catchPoints[index] != 0;
    }

    public void OnGameButtonPressed(int index)
    {
        if(!IsTapped(index))
        {
            m_catchPoints[index] = GetCurrentCatchPoint(index);
        }
    }
}
