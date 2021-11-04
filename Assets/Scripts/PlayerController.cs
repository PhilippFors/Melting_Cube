using Entities.Player.PlayerInput;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Vector2 sensitivity = new Vector2(2,4);
    [SerializeField] private Vector2 clamPoint = new Vector2(-1 ,1);
    [SerializeField] private Vector2 absoluteDelta;
    [SerializeField] private float force = 10;
    
    private bool canThrow;
    private bool hitPlayer;
    private Rigidbody rb;
    private Camera cam;
    private Vector2 mousePos => PlayerInputController.Instance.MousePosition.ReadValue();
    private Vector2 currentDelta => PlayerInputController.Instance.MouseDelta.ReadValue();
    private bool lmbPressed => PlayerInputController.Instance.LeftMouseButton.IsPressed;
    private bool rmbPressed => PlayerInputController.Instance.RightMouseButton.IsPressed;

    private void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (rmbPressed) {
            canThrow = false;
            hitPlayer = false;
            absoluteDelta = Vector2.zero;
            return;
        }
        
        if (lmbPressed) {
            var ray = cam.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity)) {
                var player = hit.transform.GetComponent<PlayerController>();
                if (player) {
                    canThrow = true;
                    hitPlayer = true;
                }
            }
        }
        else {
            canThrow = false;
            if (hitPlayer) {
                Release();
                hitPlayer = false;
            }
        }

        if (canThrow) {
            absoluteDelta += (currentDelta / Screen.width) * sensitivity;
            absoluteDelta = new Vector2(
                Mathf.Clamp(absoluteDelta.x, clamPoint.x,clamPoint.y), 
                Mathf.Clamp(absoluteDelta.y, clamPoint.x, clamPoint.y)
                );
        }
        else {
            absoluteDelta = Vector2.zero;
        }
    }

    private void Release()
    {
        rb.AddForce(new Vector3(0, absoluteDelta.y  * -1, absoluteDelta.x * -1) * force, ForceMode.Impulse);
    }
}
