using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions =  new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        // inputVector need to be normalized because when pressing two keys it's vector has large magnitude, that need to normalize 
        inputVector = inputVector.normalized;

        return inputVector;
    }
}
