using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatePlaying : PlayerStateBase
{
    public override void EnterState(PlayerManager player) {}
    public override void UpdateState(PlayerManager player) {
        if (Input.GetKeyDown(KeyCode.Q)) {
            if (!player.HasReachedEnd()) {
                player.generateNote();
                player.CurrentPlayingIndex++;
            }
        }
    }
    public override void LeaveState(PlayerManager player) {
        player.CurrentPlayingIndex = 0;
    }
}
