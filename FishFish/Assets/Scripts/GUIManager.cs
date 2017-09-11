using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {
    public Text[] m_scores = new Text[3];

    public TextNumberAnimated m_roundScore = null;

    public InterRoundUI InterRoundUI = null;
}
