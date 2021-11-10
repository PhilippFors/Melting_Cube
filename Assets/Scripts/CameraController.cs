using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;
    void Update()
    {
        var newPos = new Vector3(player.position.x + xOffset, player.position.y + yOffset, player.position.z);
        transform.position = newPos;
    }

    private void OnValidate()
    {
        transform.position = new Vector3(player.position.x + xOffset, player.position.y + yOffset, player.position.z);
    }
}
