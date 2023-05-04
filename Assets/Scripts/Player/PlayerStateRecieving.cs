using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateRecieving : PlayerStateBase
{
    public override void EnterState(PlayerManager player) {}
    public override void UpdateState(PlayerManager player) {
        if (Input.GetKeyDown(KeyCode.P)) {
            player.respondNote(player.ReciveingNotes.Dequeue());
        }
    }
    public override void LeaveState(PlayerManager player) {
        player.CurrentPlayingIndex = 0;
    }
}
