using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public int Score;
    public bool IsLeftTurn = true;
    [SerializeField] private TriggerLine leftTriggerLine, rightTriggerLine;

    #region FSM
    private GameStateBase currentState;
    public GameStateBase previousState;
    public GameStateNormal stateNormal = new GameStateNormal();

    public void ChangeState(GameStateBase newState)
    {
        if (currentState != newState) {
            if (currentState != null)
            {
                currentState.LeaveState(this);
            }

            currentState = newState;

            if (currentState != null)
            {
                currentState.EnterState(this);
            }
        }
    }

    public void ChangeToPreviousState() {
        if (currentState != previousState) {
            ChangeState(previousState);
        }
    }
    #endregion

    void Update() {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
        if (leftTriggerLine != null) {
            if (IsLeftTurn) {
                leftTriggerLine.isCheckingNote = false;
                rightTriggerLine.isCheckingNote = true;
            } else {
                leftTriggerLine.isCheckingNote = true;
                rightTriggerLine.isCheckingNote = false;
            }
        }
    }
}
