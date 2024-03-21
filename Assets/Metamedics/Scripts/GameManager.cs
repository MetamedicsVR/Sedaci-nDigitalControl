using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourInstance<GameManager>
{
    public SceneName currentScene { get; protected set; }

    private Coroutine loadingScene;
    private MetaMedicsBuildSettings settings;

    protected override void OnInstance()
    {
        DontDestroyOnLoad(gameObject);
    }

    public MetaMedicsBuildSettings GetSettings()
    {
        if (settings == null)
        {
            settings = Resources.Load<MetaMedicsBuildSettings>("MetaMedicsBuildSettings");
        }
        return settings;
    }

    public void LoadScene(SceneName sceneName)
    {
        if (loadingScene == null)
        {
            loadingScene = StartCoroutine(LoadingScene(sceneName, LoadSceneMode.Single));
        }
    }

    public void AddScene(SceneName sceneName)
    {
        if (loadingScene == null)
        {
            loadingScene = StartCoroutine(LoadingScene(sceneName, LoadSceneMode.Additive));
        }
    }

    private IEnumerator LoadingScene(SceneName sceneName, LoadSceneMode loadSceneMode)
    {
        AsyncOperation asyncLoading = SceneManager.LoadSceneAsync(GetSceneString(sceneName), loadSceneMode);
        yield return asyncLoading;
        currentScene = sceneName;
        loadingScene = null;
    }

    public SceneName GetCurrentScene()
    {
        return currentScene;
    }

    public string GetSceneString(SceneName sceneName)
    {
        switch (sceneName)
        {
            case SceneName.Main:
                return "StreamingScene";
            case SceneName.SedationPhone:
                return "MobileApp";
            case SceneName.SedationVR:
                return "Scene_SedacionDigital_LINEAL";
        }
        return "";
    }

    public enum SceneName
    {
        Main,
        SedationPhone,
        SedationVR
    }

    public enum Device
    {
        OculusQuest2,
        PC,
        Mobile,
        Web
    }

    public enum Tracking
    {
        Any,
        Controllers,
        HandTracking,
        None
    }
}
