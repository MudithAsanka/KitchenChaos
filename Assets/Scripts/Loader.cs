using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MainMenuScene,
        GameScene,
        LoadingScene,
        LobbyScene,
        CharacterSelectScene
    }

    public static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    // In order to load through NetworkManager
    public static void LoadNetwork(Scene targetScene)
    {
        // If we donot load through NetworkManager, objects of new scene will not automatically spawn
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString()); // To String-LoadScene doesnot have aurgument of custom enum
    }
}
