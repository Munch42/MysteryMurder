using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField]
    TMP_Text text;
    Player player;

    public void Setup(Player _player)
    {
        player = _player;
        text.text = player.NickName;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // otherPlayer is the player that left the room. So we want to check if it was the player this list item is assigned to.
        if (player == otherPlayer)
        {
            // If our player left the room, we want to destroy this gameobject in the list.
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
