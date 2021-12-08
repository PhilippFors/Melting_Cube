using UnityEngine;

public class StartMenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject controlsMenu;

    public void ShowControls()
    {
        mainMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    public void ShowMainMenu()
    {
        controlsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void StartGame()
    {
        SceneLoader.Instance.StartGame();
    }

    public void Quit()
    {
        GameManager.Instance.CloseGame();
    }
}
