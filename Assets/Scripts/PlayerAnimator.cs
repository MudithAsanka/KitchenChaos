using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    private const string IS_WALKING = "IsWalking";
    [SerializeField] private Player player;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // If this is not localplayer, It will not execute and just return, If it is localplayer continue
        if (!IsOwner)
        {
            return;
        }

        animator.SetBool(IS_WALKING, player.IsWalking());
    }
}
