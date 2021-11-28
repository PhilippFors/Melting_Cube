using Entities.Player.PlayerInput;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool HasCollided => hasCollided;
    public event System.Action onRelase;
    
    [SerializeField] private bool useMouseDistance;
    [SerializeField] private Vector2 sensitivity = new Vector2(2, 4);
    [SerializeField] private Vector2 clamPoint = new Vector2(-1, 1);
    [SerializeField] private Vector2 absoluteDelta;
    [SerializeField] private float force = 8;
    [SerializeField] private float maxDistance = 4;
    [SerializeField] private float distance;
    [SerializeField] private bool hasCollided;
    
    private bool canThrow;
    private bool hitPlayer;
    private Rigidbody rb;
    private Camera cam;
    private Vector2 mousePos => PlayerInputController.Instance.MousePosition.ReadValue();
    private Vector2 currentDelta => PlayerInputController.Instance.MouseDelta.ReadValue();
    private bool lmbPressed => PlayerInputController.Instance.LeftMouseButton.IsPressed;
    private bool rmbPressed => PlayerInputController.Instance.RightMouseButton.IsPressed;
    private Plane plane;
    private Vector3 direction;
    private bool isGrounded;
    
    private void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (GroundCheck()) {
            if (useMouseDistance) {
                DistanceBased();
            }
            else {
                DeltaBased();
            }
        }
        
        
    }

    private bool GroundCheck()
    {
        return Physics.CheckSphere(transform.position, transform.localScale.x + 0.05f, LayerMask.GetMask("Ground"));
    }

    private void DistanceBased() // based and asianwifepilled
    {
        var ray = cam.ScreenPointToRay(mousePos);

        if (rmbPressed) {
            canThrow = false;
            hitPlayer = false;
            NewTrajectoryPredictor.Instance.Disable();
            return;
        }
        
        if (lmbPressed) {
            
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
                Release(true);
                hitPlayer = false;
            }
        }

        if (canThrow) {
            plane = new Plane(Vector3.right, Vector3.zero);
            plane.Raycast(ray, out var d);
            var point = ray.GetPoint(d);
            direction = (transform.position - point).normalized;
            distance = Vector3.Distance(point, transform.position);
            distance = Mathf.Clamp(distance, 0, maxDistance);
            
            NewTrajectoryPredictor.Instance.Enable();
            //TODO: Find a better way to do trajectories. Use phys sym for only checking collisions
            // if (!TrajectoryPredictor.Instance.isRunning) {
            //     TrajectoryPredictor.Instance.SimulateTrajectory(gameObject, transform.position, direction * (distance * force)).Forget();
            // }
            NewTrajectoryPredictor.Instance.Simulate(this.gameObject, rb.velocity, direction * (distance * force));
        }
    }

    private void DeltaBased() // based? based on what
    {
        var ray = cam.ScreenPointToRay(mousePos);

        if (rmbPressed) {
            canThrow = false;
            hitPlayer = false;
            absoluteDelta = Vector2.zero;
            return;
        }

        if (lmbPressed) {
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
                Release(false);
                hitPlayer = false;
            }
        }

        if (canThrow) {
            absoluteDelta += (currentDelta / Screen.width) * sensitivity;
            absoluteDelta = new Vector2(
                Mathf.Clamp(absoluteDelta.x, clamPoint.x, clamPoint.y),
                Mathf.Clamp(absoluteDelta.y, clamPoint.x, clamPoint.y)
            );
        }
        else {
            absoluteDelta = Vector2.zero;
        }
    }

    private void Release(bool usemoused)
    {
        if (usemoused) {
            rb.AddForce(direction * (distance * force), ForceMode.Impulse);
        }
        else {
            rb.AddForce(new Vector3(0, absoluteDelta.y * -1, absoluteDelta.x * -1) * force, ForceMode.Impulse);
        }

        onRelase?.Invoke();
        hasCollided = false;
        TrajectoryPredictor.Instance.Disable();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<MeltStopper>()) {
            hasCollided = true;
        }
    }
}