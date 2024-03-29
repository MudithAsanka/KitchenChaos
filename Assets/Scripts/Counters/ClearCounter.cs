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
                if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // Player is holding a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }

                }
                else
                {
                    // Player is not carrying a plate but something else
                    if(GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        // Checking the counter and counter is holding a plate
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                        }
                    }
                }
            }
            else
            {
                // Player is not carrying anything...
                GetKitchenObject().SetKitchenObjectParent(player); 
            }
        }
    }
}
