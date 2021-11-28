using UnityEngine.SceneManagement;
using UsefulCode.SOArchitecture.Utility;
using UsefulCode.Utilities;

public class SceneLoader : SingletonBehaviour<SceneLoader>
{
    public SceneInfo[] scenes;
    public SceneInfo activeScene;
    private int activeLevel;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        activeScene = scenes[0];
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
    }

    public void LoadNextLevel()
    {
        activeLevel++;
        if (activeLevel < scenes.Length) {
            LoadScene(scenes[activeLevel]);
        }
    }
}
