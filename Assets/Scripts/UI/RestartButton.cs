using UnityEngine;

public class RestartButton : MonoBehaviour
{
    public void Restart()
    {
        SceneLoader.Instance.ReloadScene();
    }
}
