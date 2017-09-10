using System;
using System.Collections.Generic;
using UnityEngine;

public class Wakka : MonoBehaviour {

    public ParticleSystem m_popParticles = null;
    public ParticleSystem m_catchedParticles = null;
    public SpriteRenderer BodyImage = null;
    public SpriteRenderer Bubble = null;

    public Sprite m_waitingSprite = null;
    public Sprite m_FleeingSprite = null;
    public Sprite m_catchedSprite = null;

    public enum State { Waiting, Fleeing, Over, Catched, Hidden }
    FSMState m_currentState;

    [SerializeField]
    private Vector3 m_basePosition = Vector3.zero;
    [SerializeField]
    private Vector3 m_endFleePosition = Vector3.zero;
    [SerializeField]
    private Vector3 m_hoverPosition = Vector3.zero;
    private float m_fleeingDuration = 1;

    private void Awake()
    {
        InitStates();
    }

    private void Update()
    {
        if (m_currentState.Update != null)
            m_currentState.Update();
    }

    public void StartFleeing(float duration)
    {
        m_fleeingDuration = duration;
        SetState(State.Fleeing);
    }

    #region FSM
    private float m_timer;

    struct FSMState
    {
        public Action Start;
        public Action Update;
        public Action Stop;
    }

    private Dictionary<State, FSMState> m_stateDict = new Dictionary<State, FSMState>();

    private void InitStates()
    {
        m_stateDict[State.Waiting] = new FSMState() { Start = Waiting_Start};
        m_stateDict[State.Fleeing] = new FSMState() { Start = Fleeing_Start, Update = Fleeing_Update };
        m_stateDict[State.Over] = new FSMState() ;
        m_stateDict[State.Catched] = new FSMState() { Start = Catched_Start, Update = Catched_Update };
        m_stateDict[State.Hidden] = new FSMState() { Start  = Hidden_Start};

        m_currentState = m_stateDict[State.Waiting];
    }

    public void SetState(State state)
    {
        if (m_currentState.Stop != null)
            m_currentState.Stop();
        m_currentState = m_stateDict[state];
        if (m_currentState.Start != null)
            m_currentState.Start();
    }

    private void Waiting_Start()
    {
        transform.localPosition = m_basePosition;
        Bubble.gameObject.SetActive(false);
        m_popParticles.Stop();
        m_catchedParticles.Stop();
        BodyImage.sprite = m_waitingSprite;
        BodyImage.enabled = true;
    }

    private Vector3 m_fleeingVector;
    private void Fleeing_Start()
    {
        m_popParticles.Play();
        m_fleeingVector = m_endFleePosition - m_basePosition;
        BodyImage.sprite = m_FleeingSprite;
        m_timer = 0;
    }

    private void Fleeing_Update()
    {
        m_timer += Time.deltaTime;
        float progression = Mathf.Clamp01(m_timer / m_fleeingDuration);
        transform.localPosition = m_basePosition + m_fleeingVector * progression;
        if (m_timer > m_fleeingDuration)
        {
            SetState(State.Over);
        }
    }

    private Vector3 m_hoverVelocity;
    private float m_hoverPhase = 0;
    private void Catched_Start()
    {
        Bubble.gameObject.SetActive(true);
        BodyImage.sprite = m_catchedSprite;
        m_catchedParticles.Play();
        m_hoverVelocity = new Vector3(0, 0, 0);
        m_hoverPhase = transform.position.x;
    }

    private void Catched_Update()
    {
        Vector3 hoverTarget = m_hoverPosition;
        float t = Time.time;
        hoverTarget.x += Mathf.Sin((t + m_hoverPhase) * 2) * .1f;
        hoverTarget.y += Mathf.Sin(t + m_hoverPhase) * .1f;


        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, hoverTarget, ref m_hoverVelocity, .85f);
    }

    private void Hidden_Start()
    {
        BodyImage.enabled = false;
        m_popParticles.Play();
    }

    #endregion
}
