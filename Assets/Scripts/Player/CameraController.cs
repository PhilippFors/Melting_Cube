
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;

    private void Start()
    {
        StartCoroutine(FindPlayer());
    }

    void Update()
    {
        if (!player) {
            return;
        }

        var newPos = new Vector3(player.position.x + xOffset, player.position.y + yOffset, player.position.z);
        transform.position = newPos;
    }

    // private void OnValidate()
    // {
    //     transform.position = new Vector3(player.position.x + xOffset, player.position.y + yOffset, player.position.z);
    // }

    private IEnumerator FindPlayer()
    {
        yield return new WaitUntil(GetPlayer);
    }

    private bool GetPlayer()
    {
        var p = GameObject.FindWithTag("Player");
        if (p) {
            player = p.transform;
            return true;
        }

        return false;
    }
}