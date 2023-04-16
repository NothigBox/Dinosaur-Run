using UnityEngine;

public abstract class GameStateObserver : MonoBehaviour
{
    public virtual void UpdateState(GameState newState) 
    {
        switch (newState)
        {
            case GameState.Home:
                Home();
                break;

            case GameState.Waiting:
                Waiting();
                break;

            case GameState.Playing:
                Playing();
                break;

            case GameState.Restarting:
                Restarting();
                break;
        }
    }

    protected abstract void Home();
    protected abstract void Waiting();
    protected abstract void Playing();
    protected abstract void Restarting();
}