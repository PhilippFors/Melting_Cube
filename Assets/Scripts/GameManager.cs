using Entities.Player.PlayerInput;
using UnityEngine;
using UsefulCode.Utilities;

public class GameManager : SingletonBehaviour<GameManager>
{
    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayerInputController.Instance.Quit.Performed += ctx => CloseGame();
    }
    
    void CloseGame()
    {
        Application.Quit();
    }
}
