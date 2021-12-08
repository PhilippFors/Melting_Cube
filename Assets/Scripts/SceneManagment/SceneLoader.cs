using Entities.Player.PlayerInput;
using UnityEngine.SceneManagement;
using UsefulCode.SOArchitecture.Utility;
using UsefulCode.Utilities;

public class SceneLoader : SingletonBehaviour<SceneLoader>
{
    public SceneInfo mainMenu;
    public SceneInfo[] scenes;
    private int activeScene;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        var a = SceneManager.GetActiveScene();
        foreach (var s in scenes) {
            if (a.buildIndex == s.SceneIndex) {
                activeScene = s.SceneIndex;
            }
        }
    }

    public void StartGame()
    {
        LoadScene(scenes[0]);
    }

    public void ReturnToMenu()
    {
        LoadScene(mainMenu);
    }
    
    public void ReloadScene()
    {
        LoadScene(activeScene);
    }

    public void LoadScene(SceneInfo scene, bool asyncLoad = true)
    {
        LoadScene(scene.SceneIndex, asyncLoad);
    }

    public void LoadScene(int index, bool asyncLoad = true)
    {
        activeScene = index;
        if (asyncLoad) {
            SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        }
        else {
            SceneManager.LoadScene(index, LoadSceneMode.Single);
        }

        if (GameManager.Instance.reverseGravity) {
            GameManager.Instance.ToggleGravity();
        }

        PlayerInputController.Instance.EnableControls();
    }

    public void LoadNextLevel()
    {
        activeScene++;
        if (activeScene >= scenes.Length) {
            activeScene = 0;
            ReturnToMenu();
        }
        else {
            LoadScene(scenes[activeScene]);
        }
    }
}