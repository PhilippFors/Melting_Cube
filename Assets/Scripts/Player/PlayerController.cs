using System.Collections;
using Entities.Player.PlayerInput;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool OnWall => onWall;
    public bool HasCollided => hasCollided;
    public event System.Action onRelase;

    [SerializeField] private GameObject visual;
    [SerializeField] private bool useMouseDistance;
    [SerializeField] private Vector2 sensitivity = new Vector2(2, 4);
    [SerializeField] private Vector2 clamPoint = new Vector2(-1, 1);
    [SerializeField] private Vector2 absoluteDelta;
    [SerializeField] private float force = 8;
    [SerializeField] private float maxDistance = 4;
    [SerializeField] private float distance;
    [SerializeField] private bool hasCollided;
    [SerializeField] private float wallSlideTime;
    [SerializeField] private float wallSlideDelay = 0.5f;
    [SerializeField] private float wallSlideSpeed = 5f;
    [SerializeField] private ParticleSystem wallSlideParticles;

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
    private bool canWallSlide;
    private bool onWall;
    private ContactPoint contact;
    private Vector3 rayCastContact;
    private Coroutine wallSlide;

    private void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (GroundCheck() || onWall) {
            if (useMouseDistance) {
                DistanceBased();
            }
            else {
                DeltaBased();
            }
        }
        else {
            canThrow = false;
            hitPlayer = false;
            NewTrajectoryPredictor.Instance.DisableTrajectory();
        }

        if (onWall) {
            var dir = contact.point - transform.position;
            var raycastDir = rayCastContact - transform.position;
            var cross = Vector3.Cross(dir, contact.normal);
            Debug.Log(cross);
            var crossY = cross.y;
            var crossZ = cross.z;

            if (cross.y > 0) {
                crossY = cross.y * -1;
            }

            if (cross.z > 0 && cross.y < 0) {
                crossZ = cross.z;
            }
            else if (cross.z > 0 && cross.y > 0) {
                crossZ = -cross.z;
            }
            else if (cross.z < 0 && cross.y > 0) {
                crossZ = -cross.z;
            }

            var newCross = new Vector3(0, crossY, crossZ);

            Physics.Raycast(transform.position, raycastDir, out var hit, visual.transform.localScale.x + 0.2f, LayerMask.GetMask("Ground"));
            rayCastContact += newCross * (Time.deltaTime * wallSlideSpeed);
            
            if (hit.transform && hit.transform.CompareTag("Wall")) {
                transform.position += newCross * (Time.deltaTime * wallSlideSpeed);
            }
            else {
                StopWallSlide();
            }

            if (Physics.Raycast(transform.position, newCross, visual.transform.localScale.x + 0.1f, LayerMask.GetMask("Ground"))) {
                StopWallSlide();
            }
        }
    }

    private bool GroundCheck()
    {
        return Physics.CheckBox(transform.position, visual.transform.localScale / 2 + new Vector3(0.05f, 0.05f, 0.05f),
            transform.rotation, LayerMask.GetMask("Ground"));
    }

    private void DistanceBased() // based and asianwifepilled
    {
        var ray = cam.ScreenPointToRay(mousePos);

        if (rmbPressed) {
            canThrow = false;
            hitPlayer = false;
            NewTrajectoryPredictor.Instance.DisableTrajectory();
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

            NewTrajectoryPredictor.Instance.Simulate(gameObject, rb.velocity, direction * (distance * force));
            NewTrajectoryPredictor.Instance.EnableTrajectory();
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
                var player = hit.transform.GetComponentInParent<PlayerController>();
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
        StopWallSlide();

        if (usemoused) {
            rb.AddForce(direction * (distance * force), ForceMode.Impulse);
        }
        else {
            rb.AddForce(new Vector3(0, absoluteDelta.y * -1, absoluteDelta.x * -1) * force, ForceMode.Impulse);
        }

        onRelase?.Invoke();
        hasCollided = false;
        NewTrajectoryPredictor.Instance.DisableTrajectory();
    }

    private void StartWallSlide(ContactPoint contact)
    {
        canWallSlide = false;
        onWall = true;
        wallSlideParticles.Play();

        var look = Quaternion.LookRotation(-contact.normal, Vector3.up);
        transform.rotation = look;
        SetWallSlideRigidbody();
        wallSlide = StartCoroutine(WallSlide());
    }

    private void StopWallSlide()
    {
        if (wallSlide != null) {
            StopCoroutine(wallSlide);
        }

        StartCoroutine(WallSlideDelay());
        onWall = false;
        wallSlideParticles.Stop();
        ResetRigidbody();
    }

    private void SetWallSlideRigidbody()
    {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
    }

    private void ResetRigidbody()
    {
        rb.isKinematic = false;
    }

    private IEnumerator WallSlide()
    {
        yield return new WaitForSeconds(wallSlideTime);
        onWall = false;
        ResetRigidbody();
        wallSlideParticles.Stop();
        StartCoroutine(WallSlideDelay());
    }

    private IEnumerator WallSlideDelay()
    {
        yield return new WaitForSeconds(wallSlideDelay);
        canWallSlide = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<MeltStopper>()) {
            hasCollided = true;
        }

        if (other.gameObject.CompareTag("Wall") && canWallSlide) {
            var c = other.GetContact(0);
            contact = c;
            rayCastContact = contact.point;
            StartWallSlide(c);
        }

        if (other.gameObject.CompareTag("Ground")) {
            StopWallSlide();
        }
    }
}