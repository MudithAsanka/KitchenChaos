using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    // Overrided the BaseCounter Interact function
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // There is no kitchenObject here...
            if (player.HasKitchenObject())
            {
                // Player is carrying something...
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                // Player is not carrying anything...
            }
        }
        else
        {
            // There is a kitchenObject here...
            if (player.HasKitchenObject())
            {
                // Player is carrying something...
            }
            else
            {
                // Player is not carrying anything...
                GetKitchenObject().SetKitchenObjectParent(player); 
            }
        }
    }
}
