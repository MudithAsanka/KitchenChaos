using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnInteractAction;

    PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions =  new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Interact.performed += Interact_performed;
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        /* --- Note ---
        Check if there any listners(subscribers) , otherwise it throwback null reference exception
        if(OnInteractAction != null)
        {
            OnInteractAction(this, EventArgs.Empty);
        }
         OR can use null conditional operator
        */
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        // inputVector need to be normalized because when pressing two keys it's vector has large magnitude, that need to normalize 
        inputVector = inputVector.normalized;

        return inputVector;
    }
}
