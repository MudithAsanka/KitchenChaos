using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            // If Player holding something destroy it
            player.GetKitchenObject().DestroySelf();
        }
    }
}
