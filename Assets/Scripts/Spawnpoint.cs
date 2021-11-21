using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    void Start()
    {
        Instantiate(playerPrefab, transform.position, transform.rotation);
    }

}
