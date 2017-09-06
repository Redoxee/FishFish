using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProcess : MonoBehaviour {

    public const int nbControls = 3;

    float[] m_startStamps = new float[nbControls];

    float timer = 0;
    int[] m_scores = new int[nbControls];
    


    public void OnGameButtonPressed(int index)
    {

    }
}
