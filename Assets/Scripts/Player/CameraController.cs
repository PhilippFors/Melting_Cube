using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private bool followPlayer = true;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;
    [SerializeField] private float edgePadding = 100f;
    [SerializeField] private float speed = 5f;
    private Camera cam;
    private int ScreenWidth => Screen.width;
    private int ScreenHeight => Screen.height;
    
    private void Start()
    {
        cam = GetComponent<Camera>();
        StartCoroutine(FindPlayer());
        transform.position = new Vector3(xOffset, transform.position.y, transform.position.z);
    }

    void Update()
    {
        if (!player) {
            return;
        }
        
        if (followPlayer) {
            transform.position = new Vector3(player.position.x + xOffset, player.position.y + yOffset, player.position.z);
            return;
        }
        
        var newPos = Vector3.zero;
        var screenPoint = cam.WorldToScreenPoint(player.position);

        if (screenPoint.x <= edgePadding) {
            newPos += -Vector3.forward * Time.deltaTime * speed;
        }

        if (screenPoint.x >= ScreenWidth - edgePadding) {
            newPos += Vector3.forward * Time.deltaTime * speed;
        }

        if (screenPoint.y <= edgePadding) {
            newPos += Vector3.down * Time.deltaTime * speed;
        }

        if (screenPoint.y >= ScreenHeight - edgePadding) {
            newPos += Vector3.up * Time.deltaTime * speed;
        }
        
        newPos = new Vector3(newPos.x, newPos.y, newPos.z);

        transform.position += newPos;
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