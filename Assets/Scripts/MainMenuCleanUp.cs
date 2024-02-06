using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour
{
    private void Awake()
    {
        // CleanUp NetworkManager, otherwise everytime open MainMenu create NetworkManager
        if(NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        // CleanUp KitchenGameMultiplayer, otherwise everytime open MainMenu create KitchenGameMultiplayer
        if (KitchenGameMultiplayer.Instance != null)
        {
            Destroy(KitchenGameMultiplayer.Instance.gameObject);
        }
    }
}
