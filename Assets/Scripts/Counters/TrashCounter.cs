using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnAnyObjectTrashed;

    // 'TrashCounter.ResetStaticData()' hides inherited member 'BaseCounter.ResetStaticData()'.
    // Use the new keyword if hiding was intended.
    new public static void ResetStaticData()
    {
        // Clear the listeners
        OnAnyObjectTrashed = null;
    }

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            // If Player holding something destroy it
            player.GetKitchenObject().DestroySelf();

            OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);  // On Event of trashed
        }
    }
}
