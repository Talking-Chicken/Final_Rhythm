using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateRecieving : PlayerStateBase
{
    public override void EnterState(PlayerManager player) {
        player.PlayingNotes.Clear();
    }

    public override void UpdateState(PlayerManager player) {
        if (Input.GetKeyDown(player.PlayKey)) {
            if (!player.HasRecievedEnd()) {
                player.respondNote(player.ReciveingNotes.Peek());
            }
        }

        player.ShowNextNoteCount();
    }
    public override void LeaveState(PlayerManager player) {
        player.CurrentPlayingIndex = 0;
    }
}
