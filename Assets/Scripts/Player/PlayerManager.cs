using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PlayerManager : MonoBehaviour
{
    #region FSM
    public enum PlayerStateType {Playing, Recieving}
    [SerializeField, BoxGroup("FSM")] private PlayerStateType enteringState;
    private PlayerStateBase currentState;
    public PlayerStateBase previousState;
    public PlayerStatePlaying statePlaying = new PlayerStatePlaying();
    public PlayerStateRecieving stateRecieving = new PlayerStateRecieving();

    public void ChangeState(PlayerStateBase newState)
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

    void Start()
    {
        
    }

    
    void Update()
    {
        currentState.UpdateState(this);
    }
}
