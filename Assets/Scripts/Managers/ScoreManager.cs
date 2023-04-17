using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : GameStateObserver
{
    public event Action<int> OnScoreAdded;

    private int currentScore;

    public int CurrentScore => currentScore;

    public void AddScore(int score)
    {
        currentScore += Mathf.Abs(score);

        OnScoreAdded?.Invoke(currentScore);
    }

    protected override void Home()
    {
    }

    protected override void Waiting()
    {
        //Get data
    }

    protected override void Playing()
    {
        //Show Highscore
    }

    protected override void Restarting()
    {
        //Set data
    }
}
