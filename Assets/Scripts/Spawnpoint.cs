using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject previewPlayerPrefab;
    void Start()
    {
        Instantiate(playerPrefab, transform.position, transform.rotation);
        var obj = Instantiate(previewPlayerPrefab, transform.position, transform.rotation);
        obj.SetActive(false);
        NewTrajectoryPredictor.Instance.previewPlayer = obj;
    }
}
