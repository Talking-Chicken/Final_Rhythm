using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    [ReadOnly, SerializeField, BoxGroup("Reference")] private AudioManager audioManager;
    [SerializeField, BoxGroup("Reference")] private PlayerManager otherPlayer;
    
    [SerializeField, BoxGroup("Both")] KeyCode playKey;
    [SerializeField, BoxGroup("Playing")] Transform startPosition;
    [SerializeField, BoxGroup("Playing")] TextMeshProUGUI playingCountText;
    [SerializeField, BoxGroup("Recieving")] Transform targetPosition;

    public enum HitOrMiss { Perfect, Good, Okay, Miss, Late }
    [SerializeField, BoxGroup("Recieving")] int perfectWindow = 100;
    [SerializeField, BoxGroup("Recieving")] int goodWindow = 250;
    [SerializeField, BoxGroup("Recieving")] int okayWindow = 500;
    float movementTime = 1.0f; //这定义根本没用
    Vector3 movementSpeed = Vector3.zero;
    private SpriteRenderer noteSprite;
    [SerializeField, BoxGroup("Note")] private NoteCircle notePrefab;
    private Queue<NoteCircle> playingNotes = new Queue<NoteCircle>(), recievingNotes = new Queue<NoteCircle>();

    [ReadOnly, SerializeField, BoxGroup("Inside Section")] private int currentPlayingIndex;

    //getters & setters
    public KeyCode PlayKey {get=>playKey;}
    public Queue<NoteCircle> PlayingNotes {get=>playingNotes; set=>playingNotes = value;}
    public Queue<NoteCircle> ReciveingNotes {get=>recievingNotes; set=>recievingNotes=value;}
    public int CurrentPlayingIndex {get=>currentPlayingIndex; set=>currentPlayingIndex=value;}

    #region FSM
    public enum PlayerStateType {Playing, Recieving}
    [SerializeField, BoxGroup("FSM")] private PlayerStateType enteringState;
    [ReadOnly, SerializeField, BoxGroup("FSM")] private PlayerStateType currentStateType;
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

    public void ToggleState() {
        if (currentState == statePlaying)
            ChangeState(stateRecieving);
        else
            ChangeState(statePlaying);
    }
    #endregion

    void Start()
    {
        //set reference
        audioManager = FindObjectOfType<AudioManager>();

        if (enteringState == PlayerStateType.Playing)
            ChangeState(statePlaying);
        else
            ChangeState(stateRecieving);
    }

    
    void Update()
    {
        currentState.UpdateState(this);
        if (currentState == statePlaying)
            currentStateType = PlayerStateType.Playing;
        else
            currentStateType = PlayerStateType.Recieving;
    }

    public void generateNote() {
        NoteCircle note = Instantiate(notePrefab, startPosition.position, Quaternion.identity);
        note.InitializeCircle(1, audioManager, audioManager.CurrentSection.PlayingMusicEvents[currentPlayingIndex],
                              audioManager.CurrentSection.RecievingMusicEvents[currentPlayingIndex],
                              startPosition, targetPosition, playerPlaying: this, playerReciving: otherPlayer);
        PlayingNotes.Enqueue(note);
        otherPlayer.recievingNotes.Enqueue(note);
    }

    /*respond the note of other player's playing notes*/
    public void respondNote(NoteCircle note) {
        if (note == null)
            return;
        //move based on how good player is
        switch (note.CheckPerformance(perfectWindow, goodWindow, okayWindow)) {
            case HitOrMiss.Perfect:
                break;
            case HitOrMiss.Good:
                break;
            default:
                Debug.Log("no input");
                break;
        }
    }

    /*check if there's no more note need to play/recieve in this section
      and if time allows player to play*/
    public bool HasReachedEnd() {
        if (audioManager.CurrentSection.PlayingMusicEvents.Count > 0 
            && CurrentPlayingIndex < audioManager.CurrentSection.PlayingMusicEvents.Count
            && audioManager.BarCount < audioManager.CurrentSection.TotalBarDuration/2)
            return false;
        return true;
    }

    public bool HasRecievedEnd() {
        if (ReciveingNotes.Count <= 0)
            return true;
        return false;
    }

    public void ShowPlayingNoteCount() {
        playingCountText.text = audioManager.CurrentSection.PlayingMusicEvents.Count - playingNotes.Count + "";
    }

    public void ShowNextNoteCount() {
        if (audioManager.NextSection != null)
            playingCountText.text = audioManager.NextSection.PlayingMusicEvents.Count + "";
        else
            playingCountText.text = "0";
    }
}
