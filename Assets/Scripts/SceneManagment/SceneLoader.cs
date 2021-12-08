using Entities.Player.PlayerInput;
using UnityEngine.SceneManagement;
using UsefulCode.SOArchitecture.Utility;
using UsefulCode.Utilities;

public class SceneLoader : SingletonBehaviour<SceneLoader>
{
    public SceneInfo mainMenu;
    public SceneInfo[] scenes;
    private int activeScene;
    private int activeLevel;

    private void Start()
    {
        PlayerInputController.Instance.RestartLevel.Performed += ctx => ReloadScene();
        DontDestroyOnLoad(gameObject);
        var a = SceneManager.GetActiveScene();
        for (int i = 0; i < scenes.Length; i++) {
            if (a.buildIndex == scenes[i].SceneIndex) {
                activeScene = scenes[i].SceneIndex;
                activeLevel = i;
            }
        }
    }

    public void StartGame()
    {
        LoadScene(scenes[0]);
        activeLevel = 0;
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
        activeLevel++;
        if (activeLevel >= scenes.Length) {
            activeLevel = 0;
            ReturnToMenu();
        }
        else {
            LoadScene(scenes[activeLevel]);
        }
    }
}