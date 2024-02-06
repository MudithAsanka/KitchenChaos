using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToJoinGame;

        Hide();
    }

    private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        Show();

        messageText.text = NetworkManager.Singleton.DisconnectReason;

        // If connection timeout, this event might be execute. So,
        if(messageText.text == "")
        {
            messageText.text = "Failed to connect";
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        // You need to only unsubscribe event if the objects has different lifetime
        // If both publisher and subscriber has same lifetime, do not need to manually unsubscribe
        // These UI excist on LobbyScene when change scene this will destroy, but KitchenGameMultiplayer live through all the scenes
        // Different Lifetime, So unsubscribe
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToJoinGame;
    }
}
