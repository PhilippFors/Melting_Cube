using Entities.Player.PlayerInput;
using UnityEngine.SceneManagement;
using UsefulCode.SOArchitecture.Utility;
using UsefulCode.Utilities;

public class SceneLoader : SingletonBehaviour<SceneLoader>
{
    public SceneInfo[] scenes;
    private SceneInfo activeScene;
    private int activeLevel;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        var a = SceneManager.GetActiveScene();
        foreach (var s in scenes) {
            if (a.buildIndex == s.SceneIndex) {
                activeScene = s;
            }
        }
    }

    public void ReloadScene()
    {
        LoadScene(activeScene);
    }

    public void LoadScene(SceneInfo scene, bool asyncLoad = true)
    {
        LoadScene(scene.SceneIndex, asyncLoad);
        activeScene = scene;
    }
    
    public void LoadScene(int index, bool asyncLoad = true)
    {
        if (asyncLoad) {
            SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        }
        else {
            SceneManager.LoadScene(index, LoadSceneMode.Single);
        }
        
        PlayerInputController.Instance.EnableControls();
    }

    public void LoadNextLevel()
    {
        activeLevel++;
        if (activeLevel < scenes.Length) {
            LoadScene(scenes[activeLevel]);
        }
        else {
            activeLevel = 0;
            LoadScene(scenes[activeLevel]);
        }
    }
}
