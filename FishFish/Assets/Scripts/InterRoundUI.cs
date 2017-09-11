using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InterRoundUI : MonoBehaviour {

    [SerializeField]
    private CanvasGroup m_canvasGroup = null;
    [SerializeField]
    private Vector3 m_animationDecal = Vector3.zero;
    [SerializeField]
    private AnimationCurve m_popCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField]
    private float m_poppDuration = .5f;
    [SerializeField]
    private TextNumberAnimated[] m_animatedTexts = null;
    [SerializeField]
    private TextNumberAnimated m_totalScore = null;

    private Action m_nextAction = null;
    private Action m_hiddenAction = null;

    private float m_animTimer = 0;

    private void Awake()
    {
        InitStates();
    }

    private void Update()
    {
        if (m_currentState.Update != null)
            m_currentState.Update();
    }

    public void Reset()
    {
        SetState(State.Hidden);
        foreach (var text in m_animatedTexts)
        {
            text.Text.text = "-"; // mmmmm goood naming here
        }
    }

    public void DisplayInterRound(int roundIndex, int roundScore, int prevTotalScore, int totalScore, Action nextAction)
    {
        m_animatedTexts[roundIndex].SetNumber(roundScore);
        m_totalScore.SetDisplay(prevTotalScore);
        m_totalScore.SetNumber(totalScore);
        m_nextAction = nextAction;
        SetState(State.Poppin);
    }

    public void Hide(Action onHiddenAction)
    {
        m_hiddenAction = onHiddenAction;
        SetState(State.Poppout);
    }

    public void NextButtonPressed()
    {
        m_nextAction();
    }

    enum State { Hidden, Poppin, Show, Poppout}
    struct FSMState
    {
        public Action Start;public  Action Update;public Action End;
    }
    Dictionary<State, FSMState> m_stateDict = new Dictionary<State, FSMState>();
    private void InitStates()
    {
        m_stateDict[State.Hidden] = new FSMState() { Start = Hidden_Start, End = Hidden_End };
        m_stateDict[State.Poppin] =  new FSMState() { Start = Poppin_Start, Update = Poppin_Update};
        m_stateDict[State.Show] =    new FSMState() { };
        m_stateDict[State.Poppout] = new FSMState() { Start = Poppout_Start, Update = Poppout_Update};

        m_currentState = m_stateDict[State.Hidden];
        if(m_currentState.Start != null)
            m_currentState.Start();
    }

    FSMState m_currentState;

    void SetState(State nextState)
    {
        if (m_currentState.End != null)
            m_currentState.End();
        m_currentState = m_stateDict[nextState];
        if (m_currentState.Start != null)
            m_currentState.Start();
    }

    #region Hidden
    private void Hidden_Start()
    {
        m_canvasGroup.alpha = 0;
        m_canvasGroup.blocksRaycasts = false;
        m_canvasGroup.interactable = false;
    }

    private void Hidden_End()
    {
        m_canvasGroup.blocksRaycasts = true;
        m_canvasGroup.interactable = true;
    }
    #endregion

    #region Poppin
    private void Poppin_Start()
    {
        m_animTimer = 0;
    }

    private void Poppin_Update()
    {
        m_animTimer += Time.deltaTime;
        var progression = Mathf.Clamp01(m_animTimer / m_poppDuration);
        progression = m_popCurve.Evaluate(progression);

        m_canvasGroup.alpha = progression;
        transform.localPosition = (1 - progression) * -m_animationDecal;

        if (m_animTimer >= m_poppDuration)
        {
            SetState(State.Show);
        }
    }
    #endregion

    #region Poppout

    private void Poppout_Start()
    {
        m_animTimer = 0;
        m_canvasGroup.blocksRaycasts = false;
        m_canvasGroup.interactable = false;
    }

    private void Poppout_Update()
    {
        m_animTimer += Time.deltaTime;
        var progression = Mathf.Clamp01(m_animTimer / m_poppDuration);
        progression = m_popCurve.Evaluate(progression);

        m_canvasGroup.alpha = 1 - progression;
        transform.localPosition = progression * m_animationDecal;

        if (m_animTimer >= m_poppDuration)
        {
            SetState(State.Hidden);
            if (m_hiddenAction != null)
            {
                m_hiddenAction();
            }
        }
    }
    #endregion
}
