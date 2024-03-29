using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IKitchenObjectParent
{

    public static event EventHandler OnAnyPlayerSpawned;    // static because we want this event belong to the player class itself not any specific player  
    public static event EventHandler OnAnyPickedSomething;

    public static void ResetStaticData()
    {
        // Clear the listeners
        OnAnyPlayerSpawned = null;
    }

    public static Player LocalInstance { get; private set; }     // This is a property; only set in this class, can acesess from any class

    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private LayerMask collisionsLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private List<Vector3> spawnPositionList;
    [SerializeField] private PlayerVisual playerVisual;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;

    private KitchenObject kitchenObject;

    private void Awake()
    {
        //Instance = this;
    }

    private void Start()
    {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;

        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
    }

    // When gameobject spawn on network
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // If this is local player
            LocalInstance = this;
        }

        // transform.position = spawnPositionList[(int)OwnerClientId]; // Spawn Players with different location offsets
        transform.position = spawnPositionList[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        // Handle players disconnect - this is only require for server
        // In here IsServer is refers to state of this build not this specific player
        // Also this will still attached to server-side eventhough players are not actually the host
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        // If this disconnected player has kitchenobject need to destroy
        if(clientId == OwnerClientId && HasKitchenObject())
        {
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        // If game is not in the GamePlaying State this will return without going furthur
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        // If game is not in the GamePlaying State this will return without going furthur
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        // If this is not localplayer, It will not execute and just return, If it is localplayer continue
        if (!IsOwner)
        {
            return;
        }
        HandleMovement();
        HandleInteractions();
    }

    // Returns player is moving or not
    public bool IsWalking()
    {
        return isWalking;
    }

    // Handle the player interactions
    private void HandleInteractions()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        // When stop moving moveDir become Vector3.zero and Raycast nowhere,
        // So lastInteractDir need to save otherwise no change the direction
        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if(Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)){
                // Has Clear counter component | TryGetComponent handles null check
                if(baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                // If Raycast hit something and it donot have ClearCounter
                SetSelectedCounter(null);
            }
        }
        else
        {
            // If Raycast hit the null
             SetSelectedCounter(null);
        } 
    }

    // Handle the player movements
    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        //float playerHeight = 1f; // Deprecated - For CapsulCast this was used
        bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDir, Quaternion.identity, moveDistance, collisionsLayerMask);

        // Attempting moving diaganally scenario
        if (!canMove)
        {
            // Cannot move towards the moveDir

            // Attempt only X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized; // normalized because diaganal speed need to normalized
            // moveDir.x != 0 -> Able to rotate when near the counter
            // For diaganal movement on gamepads need to consider here. for keyboards moveDir.x != 0 is fine
            canMove = (moveDir.x < -0.5f || moveDir.x > 0.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirX, Quaternion.identity, moveDistance, collisionsLayerMask);

            if (canMove)
            {
                // Can move only on the X
                moveDir = moveDirX;
            }
            else
            {
                // Cannot move only on the X

                // Attempt only Z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized; // normalized because diaganal speed need to normalized
                // moveDir.z != 0 -> Able to rotate when near the counter
                // For diaganal movement on gamepads need to consider here. for keyboards moveDir.z != 0 is fine
                canMove = (moveDir.z < -0.5f || moveDir.z > 0.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirZ, Quaternion.identity, moveDistance, collisionsLayerMask);

                if (canMove)
                {
                    // Can move only on the Z
                    moveDir = moveDirZ;
                }
                else
                {
                    // Cannot move any direction
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, rotateSpeed * Time.deltaTime);

    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        // When selectedCounter is changed execute this event
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if(kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);   // If kitchenObject is pickedup play sound
            OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
