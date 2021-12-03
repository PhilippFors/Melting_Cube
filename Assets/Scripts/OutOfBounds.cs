using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        SceneLoader.Instance.ReloadScene();
    }
}
