using DG.Tweening;
using Entities.Player.PlayerInput;
using UnityEngine;
using UsefulCode.Utilities;

public class GameManager : SingletonBehaviour<GameManager>
{
    public bool reverseGravity;
    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayerInputController.Instance.Quit.Performed += ctx => CloseGame();
    }
    
   private void CloseGame()
    {
        Application.Quit();
    }

    public void ToggleGravity()
    {
        Physics.gravity = Physics.gravity * -1;
        reverseGravity = !reverseGravity;
        var mainCam = Camera.main;
        if (reverseGravity) {
            mainCam.transform.DORotate(new Vector3(0, -90,180), 0.5f);
        }
        else {
            mainCam.transform.DORotate(new Vector3(0, -90, 0), 0.5f);
        }
    }
}
