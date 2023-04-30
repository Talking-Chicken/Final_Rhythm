using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLine : MonoBehaviour
{
    public bool isCheckingNote = false;
    void OnTriggerEnter2D(Collider2D collider) {
        if (isCheckingNote) {
            if (collider.GetComponent<NoteCircle>() != null) {
                collider.GetComponent<NoteCircle>().IsCheckingInput = true;
            }
        } else {
            if (collider.GetComponent<BeatLine>() != null) {
                collider.GetComponent<BeatLine>().IsCheckingInput = true;
            }
        }
    }

    void OntriggerExit2D(Collider2D collider) {
        if (isCheckingNote) {
            if (collider.GetComponent<NoteCircle>() != null) {
                collider.GetComponent<NoteCircle>().IsCheckingInput = false;
            }
        } else {
            if (collider.GetComponent<BeatLine>() != null) {
                collider.GetComponent<BeatLine>().IsCheckingInput = false;
            }
        }
    }
}
