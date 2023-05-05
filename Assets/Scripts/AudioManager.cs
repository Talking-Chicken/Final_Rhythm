using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using MoreMountains.Feedbacks;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AK.Wwise.Event musicEvent;
    uint playingID;

    [SerializeField]
    float beatDuration;

    [SerializeField]
    float barDuration;
    bool durationSet = false;

    [SerializeField] NoteCircle noteCirclePrefab;
    [SerializeField] BeatLine beatLinePrefab;
    [SerializeField] Transform startPosition;

    public AkMusicSyncCallbackInfo MusicInfo;

    private bool isMakingNote = false;
    private int barCount = 0;
    [SerializeField, BoxGroup("Players")] private PlayerManager player1, player2;
    [ReadOnly, SerializeField, Foldout("Sections")] private Section currentSection;
    [ReadOnly, SerializeField, Foldout("Sections")] private int currentSectionIndex = 0;
    [SerializeField, Foldout("Sections")] private List<Section> sections;
    [SerializeField, Foldout("Feedbacks")] private MMF_Player playerBumpPlayer, textBumpPlayer;

    //getters & setters
    public float BeatDuration { get { return beatDuration; } }
    public float BarDuration {get=>barDuration;}
    public bool IsMakingNote {get=>isMakingNote;set=>isMakingNote=value;}
    public int BarCount {get=>barCount;private set=>barCount=value;}
    public Section CurrentSection {get=>currentSection; private set=>currentSection=value;}
    public Section NextSection {
        get {
            if (currentSectionIndex < sections.Count-1)
                return sections[currentSectionIndex + 1];
            else
                return null;
        }
    }

    #region Setup
    private void Awake() {
        playingID = musicEvent.Post(gameObject, (uint)(AkCallbackType.AK_MusicSyncAll | AkCallbackType.AK_EnableGetMusicPlayPosition | AkCallbackType.AK_MIDIEvent), CallbackFunction);

        if (sections.Count > 0)
            CurrentSection = sections[0];
    }
    #endregion

    void Start()
    {
        
    }

    void CallbackFunction(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info) {
        AkMusicSyncCallbackInfo musicInfo;

        if (in_info is AkMusicSyncCallbackInfo) {
            musicInfo = (AkMusicSyncCallbackInfo)in_info;
            MusicInfo = musicInfo;
            switch (in_type) {

                case AkCallbackType.AK_MusicSyncUserCue:
                    // CreateBeatLine(musicInfo);
                    // if (IsMakingNote) {
                    //     CreateCircleNote(musicInfo);
                    //     IsMakingNote = false;
                    // }
                    if (musicInfo.userCueName.Equals("C")) {
                        GameManager.Instance.IsLeftTurn = !GameManager.Instance.IsLeftTurn;
                        Debug.Log("CCC");
                    }
                    break;

                case AkCallbackType.AK_MusicSyncBeat:
                    playerBumpPlayer.PlayFeedbacks();
                    textBumpPlayer.PlayFeedbacks();
                    break;
                case AkCallbackType.AK_MusicSyncBar:
                    BarCount++;
                    if (BarCount >= CurrentSection.TotalBarDuration) {
                        TogglePlayer();
                    }
                    break;
            }

            if (in_type is AkCallbackType.AK_MusicSyncBar) {
                if (!durationSet) {
                    beatDuration = musicInfo.segmentInfo_fBeatDuration;
                    barDuration = musicInfo.segmentInfo_fBarDuration;
                    durationSet = true;
                }
            }

        }
    }

    public int GetMusicTimeInMS() {
        AkSegmentInfo segmentInfo = new AkSegmentInfo();
        AkSoundEngine.GetPlayingSegmentInfo(playingID, segmentInfo, true);
        return segmentInfo.iCurrentPosition;
    }

    #region Letter Spawning

    public void CreateCircleNote(AkMusicSyncCallbackInfo info) {
        // Debug.Log($"Choosing letter {info}");

        NoteCircle pt = Instantiate(noteCirclePrefab, startPosition.position, Quaternion.identity);

        float multiplier = 4.0f;
        if (info.userCueName == "2") {
            multiplier = 2.0f;
        }
        //Debug.Log($"Multiplier: {multiplier}");

        AkSegmentInfo segmentInfo = new AkSegmentInfo();
        AkSoundEngine.GetPlayingSegmentInfo(playingID, segmentInfo, true);

        pt.InitializeCircle(multiplier, segmentInfo, this);

    }

    void CreateBeatLine(AkMusicSyncCallbackInfo info) {
        BeatLine pt;
        if (GameManager.Instance.IsLeftTurn) {
            pt = Instantiate(beatLinePrefab, new Vector2(-9,0), Quaternion.identity);
        } else {
            pt = Instantiate(beatLinePrefab, new Vector2(9,0), Quaternion.identity);
            Debug.Log(pt.transform.position);
        }

        float multiplier = 4.0f;
        if (info.userCueName == "2") {
            multiplier = 2.0f;
        }
        //Debug.Log($"Multiplier: {multiplier}");

        AkSegmentInfo segmentInfo = new AkSegmentInfo();
        AkSoundEngine.GetPlayingSegmentInfo(playingID, segmentInfo, true);

        pt.InitializeBeatline(multiplier, segmentInfo, this);
    }

    #endregion

    /*toggle between playing and recieving
      reset bar count to zero
      set current section to newest one*/
    public void TogglePlayer() {
        player1.ToggleState();
        player2.ToggleState();
        BarCount = 0;
        if (currentSectionIndex < sections.Count-1) {
            currentSectionIndex++;
            CurrentSection = sections[currentSectionIndex];
        }
    }
}
