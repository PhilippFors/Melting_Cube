using UnityEditor;
using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject previewPlayerPrefab;
    [SerializeField] private Transform levelObjects;
    void Start()
    {
        Instantiate(playerPrefab, transform.position, transform.rotation);
        var obj = Instantiate(previewPlayerPrefab, transform.position, transform.rotation);
        obj.SetActive(false);
        TrajectoryPredictor.Instance.previewPlayer = obj;
        TrajectoryPredictor.Instance.obstacleParent = levelObjects;
    }

}
