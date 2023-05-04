using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;
using NaughtyAttributes;

public class NoteCircle : MonoBehaviour
{
    AudioManager audioManager;
    [SerializeField] private AK.Wwise.Event drumEvent, endDrumEvent;
    [ReadOnly, SerializeField, BoxGroup("Music Event")] private AK.Wwise.Event playingEvent, recievingEvent;
    [SerializeField] Color[] successColors = new Color[5];
    float movementTime = 1.0f; //这定义根本没用
    Vector3 movementSpeed = Vector3.zero;
    bool hasSetMovementSpeed = false;
    int targetTime;
    [ReadOnly, SerializeField, BoxGroup("Movement")] private Vector2 targetPosition;
    [ReadOnly, SerializeField, BoxGroup("Movement")] private Vector2 startPosition = Vector2.zero;
    private bool isCheckingInput = false;
    private SpriteRenderer noteSprite;
    private bool isKeyPressed = false;

    //getters & setters
    public bool IsCheckingInput {get=>isCheckingInput;set=>isCheckingInput=value;}
    public AK.Wwise.Event PlayingEvent {get=>playingEvent; set=>playingEvent=value;}
    public AK.Wwise.Event RecievingEvent {get=>recievingEvent; set=>recievingEvent=value;}

    void Start()
    {
        noteSprite = GetComponent<SpriteRenderer>();
        PlayingEvent.Post(gameObject);
    }
    
    void Update()
    {
        // if (IsCheckingInput) {
        //     CheckInput();
        // }
    }

    public void InitializeCircle(float multiplier, AkSegmentInfo segInfo, AudioManager ls) {
        audioManager = ls;
        int currentTime = audioManager.GetMusicTimeInMS();
        targetTime = Mathf.FloorToInt(currentTime + (ls.BeatDuration * 1000 * multiplier));
        movementTime = (targetTime - currentTime) * 0.001f;
        // Debug.Log($"Current time: {currentTime} || Target time: {targetTime}", gameObject);
        StartCoroutine(MoveNoteRoutine());
    }

    public void InitializeCircle (float multiplier, AudioManager audioManager, AK.Wwise.Event playingMusicEvent, AK.Wwise.Event recevingMusicEvent, 
                                  Transform startPosition, Transform targetPosition) {
        this.audioManager = audioManager;
        int currentTime = this.audioManager.GetMusicTimeInMS();
        targetTime = Mathf.FloorToInt(currentTime + (audioManager.BarDuration * 1000 * multiplier));
        movementTime = (targetTime - currentTime) * 0.001f;

        //initialize positions
        this.startPosition = startPosition.position;
        this.targetPosition = targetPosition.position;

        //initialize music events
        PlayingEvent = playingMusicEvent;
        recievingEvent = recevingMusicEvent;

        StartCoroutine(MoveNoteRoutine());
    }

    IEnumerator MoveNoteRoutine() {
        float t = 0.0f;
        while (t < movementTime) {
            Vector3 deltaPosition = Vector3.zero;
            deltaPosition = Vector3.Lerp(startPosition, targetPosition, t / movementTime) - transform.position;
            if (!hasSetMovementSpeed && t > 0.0f) {
                Vector3 fixedDeltaPosition = Vector3.Lerp(startPosition, targetPosition, Time.fixedDeltaTime / movementTime) - transform.position;
                hasSetMovementSpeed = true;
                movementSpeed = fixedDeltaPosition;
            }
            transform.position += deltaPosition;
            t += Time.deltaTime;
            yield return null;
        }
        if (IsCheckingInput) {
            IsCheckingInput = false;
            FadeOut(PlayerManager.HitOrMiss.Miss);
        }
    }

    public PlayerManager.HitOrMiss CheckPerformance(float perfectWindow, float goodWindow) {
        PlayerManager.HitOrMiss hm = PlayerManager.HitOrMiss.Miss;
        endDrumEvent.Post(gameObject);

        int currentTime = audioManager.GetMusicTimeInMS();
        int offBy = targetTime - currentTime;

        if (offBy >= 0) {
            if (offBy <= perfectWindow)
                hm = PlayerManager.HitOrMiss.Perfect;
            else if (offBy <= goodWindow)
                hm = PlayerManager.HitOrMiss.Good;
            else
                hm = PlayerManager.HitOrMiss.Okay;
            
            RecievingEvent.Post(gameObject);
        }
        else {
            if (offBy > -perfectWindow)
                hm = PlayerManager.HitOrMiss.Late;
        }
        FadeOut(hm);
        return hm;
    }

    // void CheckInput() {
    //     if (Input.GetKeyDown(KeyCode.L) && !isKeyPressed) {
    //         HitOrMiss hm = HitOrMiss.Miss;
    //         isKeyPressed = true;
    //         endDrumEvent.Post(gameObject);

    //         int currentTime = audioManager.GetMusicTimeInMS();
    //         int offBy = targetTime - currentTime;

    //         if (offBy >= 0) {
    //             if (offBy <= perfectWindow)
    //                 hm = HitOrMiss.Perfect;
    //             else if (offBy <= goodWindow)
    //                 hm = HitOrMiss.Good;
    //             else
    //                 hm = HitOrMiss.Okay;
    //         }
    //         else {
    //             if (offBy > -perfectWindow)
    //                 hm = HitOrMiss.Late;
    //         }
    //         // Debug.Log($"Target Time: {targetTime} || Input time: {currentTime} ||  OffBy: {offBy} || Hit or Miss: {hm}", gameObject);

    //         IsCheckingInput = false;
    //         FadeOut(hm);
    //     }
    // }

    #region Animations
    public void FadeOut(PlayerManager.HitOrMiss hm) {
        StartCoroutine(FadeOutRoutine(hm));
    }
    IEnumerator FadeOutRoutine(PlayerManager.HitOrMiss hm) {
        noteSprite.color = successColors[(int)hm];
        float t = 0.0f;
        while (t < 0.5f) {
            t += Time.deltaTime;
            transform.position += movementSpeed;
            noteSprite.color = Color.Lerp(successColors[(int)hm], Color.clear, t);
            yield return null;
        }
        Destroy(gameObject);
    }
    #endregion
}
