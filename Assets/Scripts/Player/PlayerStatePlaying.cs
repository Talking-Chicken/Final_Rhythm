using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatePlaying : PlayerStateBase
{
    public override void EnterState(PlayerManager player) {
    }
    public override void UpdateState(PlayerManager player) {
        if (Input.GetKeyDown(player.PlayKey)) {
            if (!player.HasReachedEnd()) {
                player.generateNote();
                player.CurrentPlayingIndex++;
            }
        }

        player.ShowPlayingNoteCount();
    }
    public override void LeaveState(PlayerManager player) {
        player.CurrentPlayingIndex = 0;
    }
}
