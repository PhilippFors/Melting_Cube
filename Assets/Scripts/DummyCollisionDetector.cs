using UnityEngine;

public class DummyCollisionDetector : MonoBehaviour
{
    public bool hasCollided;

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.gameObject.layer == 6) {
            hasCollided = true;
        }
    }
}
