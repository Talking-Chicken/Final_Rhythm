using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PlayerManager : MonoBehaviour
{
    [ReadOnly, SerializeField, BoxGroup("Reference")]private AudioManager audioManager;
    public enum HitOrMiss { Perfect, Good, Okay, Miss, Late }
    [SerializeField, BoxGroup("Recieving")] int perfectWindow = 100;
    [SerializeField, BoxGroup("Recieving")] int goodWindow = 250;
    [SerializeField, BoxGroup("Recieving")] int okayWindow = 500;
    float movementTime = 1.0f; //这定义根本没用
    Vector3 movementSpeed = Vector3.zero;
    bool hasSetMovementSpeed = false;
    int targetTime;
    private SpriteRenderer noteSprite;
    [SerializeField, BoxGroup("Note")] private NoteCircle notePrefab;

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
        NoteCircle note = Instantiate(notePrefab, transform.position, Quaternion.identity);
        note.InitializeCircle(1, audioManager);
        
    }

    void CheckInput() {
        if (Input.GetKeyDown(KeyCode.A)) {
            HitOrMiss hm = HitOrMiss.Miss;

            int currentTime = audioManager.GetMusicTimeInMS();
            int offBy = targetTime - currentTime;

            if (offBy >= 0) {
                if (offBy <= perfectWindow) {
                    hm = HitOrMiss.Perfect;
                    GameManager.Instance.Score += 5;
                } else if (offBy <= goodWindow) {
                    hm = HitOrMiss.Good;
                    GameManager.Instance.Score += 3;
                } else {
                    hm = HitOrMiss.Okay;
                    GameManager.Instance.Score += 1;
                }
                Debug.Log("not miss");
            }
            else {
                if (offBy > -perfectWindow) {
                    hm = HitOrMiss.Late;
                    Debug.Log("missed");
                }
            }
            // Debug.Log($"Target Time: {targetTime} || Input time: {currentTime} ||  OffBy: {offBy} || Hit or Miss: {hm}", gameObject);

            FadeOut(hm);
        }
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
