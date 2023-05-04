using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PlayerManager : MonoBehaviour
{
    [ReadOnly, SerializeField, BoxGroup("Reference")] private AudioManager audioManager;
    [SerializeField, BoxGroup("Reference")] private PlayerManager otherPlayer;
    
    [SerializeField, BoxGroup("Playing")] Transform startPosition;
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
    public Queue<NoteCircle> PlayingNotes {get=>playingNotes; set=>playingNotes = value;}
    public Queue<NoteCircle> ReciveingNotes {get=>recievingNotes; set=>recievingNotes=value;}
    public int CurrentPlayingIndex {get=>currentPlayingIndex; set=>currentPlayingIndex=value;}

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
    }

    public void generateNote() {
        NoteCircle note = Instantiate(notePrefab, startPosition.position, Quaternion.identity);
        note.InitializeCircle(1, audioManager, audioManager.CurrentSection.PlayingMusicEvents[currentPlayingIndex],
                              audioManager.CurrentSection.RecievingMusicEvents[currentPlayingIndex],
                              startPosition, targetPosition);
        PlayingNotes.Enqueue(note);
        otherPlayer.recievingNotes.Enqueue(note);
        Debug.Log(name + " playing notes now have " + PlayingNotes.Count);
    }

    /*respond the note of other player's playing notes*/
    public void respondNote(NoteCircle note) {
        if (note == null)
            return;
        //move based on how good player is
        switch (note.CheckPerformance(perfectWindow, goodWindow)) {
            case HitOrMiss.Perfect:
                break;
            case HitOrMiss.Good:
                break;
            default:
                Debug.Log("no input");
                break;
        }
    }

    /*check if there's no more note need to play/recieve in this section*/
    public bool HasReachedEnd() {
        if (CurrentPlayingIndex < audioManager.CurrentSection.PlayingMusicEvents.Count)
            return false;
        return true;
    }

    #region Animations
    public void FadeOut(HitOrMiss hm) {
        StartCoroutine(FadeOutRoutine(hm));
    }
    IEnumerator FadeOutRoutine(HitOrMiss hm) {
        noteSprite.color = Color.blue;
        float t = 0.0f;
        while (t < 0.5f) {
            t += Time.deltaTime;
            transform.position += movementSpeed;
            noteSprite.color = Color.Lerp(Color.blue, Color.clear, t);
            yield return null;
        }
        Destroy(gameObject);
    }
    #endregion
}
