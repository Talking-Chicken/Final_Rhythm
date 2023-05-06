using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "Section", menuName = "Blues/Section", order = 1)]
public class Section : ScriptableObject
{
    [Tooltip("total bar duration for both player to finish this section")] public int TotalBarDuration;
    public List<AK.Wwise.Event> PlayingMusicEvents;
    public List<AK.Wwise.Event> RecievingMusicEvents;
    public bool ZoomRightIn, ZoomLeftOut, ZoomRightOut, ZoomLeftIn, LinesIn, LinesOut;
}
