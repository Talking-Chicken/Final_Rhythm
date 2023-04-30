using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDrum : MonoBehaviour
{
    public AK.Wwise.Event DrumEvent;
    void Start()
    {
        DrumEvent.Post(gameObject);
    }

    void Update()
    {
        
    }
}
