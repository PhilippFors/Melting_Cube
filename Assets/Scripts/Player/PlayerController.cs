using System.Collections;
using Entities.Player.PlayerInput;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public ParticleSystem meltParticles;
    public bool HasCollided => hasCollided;
    public bool OnWall => onWall;

    [SerializeField] private GameObject visual;
    [SerializeField] private float force = 8;
    [SerializeField] private float maxDistance = 4;
    [SerializeField] private float trajectoryLerp = 6f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private TargetRingController targetRingController;
    
    [SerializeField, Header("Wallslide")] private float wallSlideTime;
    [SerializeField] private float wallSlideDelay = 0.5f;
    [SerializeField] private float wallSlideSpeed = 5f;
    [SerializeField] private float wallSlidePushaway = 5f;
    [SerializeField] private ParticleSystem wallSlideParticles;
    
    private Rigidbody rb;
    private Camera cam;
    private MeltingController meltingController;
    private float distance;
    private bool canThrow;
    private bool hitPlayer;
    private bool hasCollided = true;
    private Plane plane;
    private Vector3 throwDirection;

    // input
    private Vector2 mousePos => PlayerInputController.Instance.MousePosition.ReadValue();
    private bool lmbPressed => PlayerInputController.Instance.LeftMouseButton.IsPressed;
    private bool rmbPressed => PlayerInputController.Instance.RightMouseButton.IsPressed;

    // Wallslide
    private bool canWallSlide;
    private bool onWall;
    private ContactPoint contact;
    private Vector3 wallDir;
    private Vector3 cross;
    private Coroutine wallSlide;
    private bool grounded;

    private float tempDistance;
    private Vector3 tempDirection;

    private void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        meltingController = GetComponent<MeltingController>();
    }

    private void Start()
    {
        targetRingController.ExpandCircle(false, maxDistance, meltingController.HitterScale);
        StopWallSlide(true);
    }

    private void Update()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, 2f, groundMask)) {
            grounded = false;
        }

        if (grounded || onWall) {
            DistanceBased();
        }
        else {
            canThrow = false;
            hitPlayer = false;
            NewTrajectoryPredictor.Instance.DisableTrajectory();
            targetRingController.ExpandCircle(false, maxDistance, meltingController.HitterScale);
            targetRingController.SetTransparency(true);
        }

        if (onWall) {
            WallSlideMovement();
        }

        meltParticles.transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    private void FixedUpdate()
    {
        if (hasCollided) {
            meltParticles.Stop();
        }
        else if (rb.velocity.magnitude > 0.5f) {
            meltParticles.Play();
        }
        else {
            meltParticles.Stop();
        }
    }

    private void WallSlideMovement()
    {
        var crossY = cross.y;
        var crossZ = cross.z;

        if (!GameManager.Instance.reverseGravity) {
            if (cross.y > 0) {
                crossY = -cross.y;
            }

            if ((cross.z > 0 && cross.y > 0) || (cross.z < 0 && cross.y > 0)) {
                crossZ = -cross.z;
            }
        }
        else {
            if (cross.y < 0) {
                crossY = -cross.y;
            }

            if ((cross.z < 0 && cross.y < 0) || (cross.z > 0 && cross.y < 0)) {
                crossZ = -cross.z;
            }
        }

        var newCross = new Vector3(0, crossY, crossZ);

        if (Physics.Raycast(transform.position, wallDir, out var hit, visual.transform.localScale.x + 0.2f, groundMask,
            QueryTriggerInteraction.Ignore) && hit.transform.CompareTag("Wall")) {
            transform.position += newCross * (Time.deltaTime *
                                              (wallSlideSpeed + Mathf.Clamp(1 - meltingController.CurrentSize, 0, 1) *
                                                  2));
        }
        else {
            StopWallSlide();
        }

        if (Physics.Raycast(transform.position, newCross, visual.transform.localScale.x + 0.1f, groundMask,
            QueryTriggerInteraction.Ignore)) {
            StopWallSlide();
        }
    }

    private void DistanceBased()
    {
        var ray = cam.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out var h, Mathf.Infinity, LayerMask.GetMask("Hitter"))) {
            targetRingController.SetTransparency(false);
        }

        if (rmbPressed) {
            canThrow = false;
            hitPlayer = false;
            NewTrajectoryPredictor.Instance.DisableTrajectory();
            targetRingController.ExpandCircle(false, maxDistance, meltingController.HitterScale);
            targetRingController.SetTransparency(true);
            return;
        }

        if (lmbPressed) {
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, LayerMask.GetMask("Hitter"))) {
                var player = hit.transform.GetComponentInParent<PlayerController>();
                if (player) {
                    canThrow = true;
                    hitPlayer = true;
                }
            }
        }
        else {
            targetRingController.SetTransparency(true);
            canThrow = false;
            targetRingController.ExpandCircle(false, maxDistance, meltingController.HitterScale);
            if (hitPlayer) {
                Release();
                hitPlayer = false;
            }
        }

        if (canThrow) {
            targetRingController.ExpandCircle(true, maxDistance, meltingController.HitterScale);
            targetRingController.SetTransparency(false);
            plane = new Plane(Vector3.right, transform.position);
            plane.Raycast(ray, out var d);
            var point = ray.GetPoint(d);
            throwDirection = (transform.position - point).normalized;
            distance = Vector3.Distance(point, transform.position);
            distance = Mathf.Clamp(distance, 0, maxDistance);

            tempDirection = Vector3.Lerp(tempDirection, throwDirection, trajectoryLerp * Time.deltaTime);
            tempDistance = Mathf.Lerp(tempDistance, distance, trajectoryLerp * Time.deltaTime);
            NewTrajectoryPredictor.Instance.Simulate(gameObject, rb.velocity, tempDirection * (tempDistance * force));
            NewTrajectoryPredictor.Instance.EnableTrajectory();
        }
    }

    private void Release()
    {
        if (onWall) {
            StopWallSlide(true);
        }

        rb.AddForce(throwDirection * (distance * force), ForceMode.Impulse);

        hasCollided = false;
        grounded = false;
        NewTrajectoryPredictor.Instance.DisableTrajectory();
        meltParticles.Play();
    }

    private void StartWallSlide()
    {
        wallDir = contact.point - transform.position;
        cross = Vector3.Cross(wallDir, contact.normal);
        canWallSlide = false;
        onWall = true;
        wallSlideParticles.Play();

        var look = Quaternion.LookRotation(-contact.normal, Vector3.up);
        transform.rotation = look;
        SetWallSlideRigidbody();
        wallSlide = StartCoroutine(WallSlide());
    }

    public void StopWallSlide(bool manualRelease = false)
    {
        if (wallSlide != null) {
            StopCoroutine(wallSlide);
        }

        onWall = false;
        wallSlideParticles.Stop();
        ResetRigidbody();
        if (!manualRelease) {
            rb.AddForce(-wallDir * wallSlidePushaway, ForceMode.Impulse);
        }

        StartCoroutine(WallSlideDelay());
    }

    private void SetWallSlideRigidbody()
    {
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
    }

    private void ResetRigidbody()
    {
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
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
        var c = other.contacts[0];
        var dot = Vector3.Dot(c.normal, Vector3.up);
        if (dot > 0.6f) {
            grounded = true;
        }

        if (other.gameObject.GetComponent<MeltStopper>()) {
            hasCollided = true;
            other.gameObject.GetComponentInChildren<ParticleSystem>().Play();
        }

        if (other.gameObject.CompareTag("Wall") && canWallSlide) {
            contact = c;
            StartWallSlide();

            var tempContactPoint = new Vector3(visual.transform.localPosition.x, visual.transform.localPosition.y, transform.InverseTransformPoint(contact.point).z);
            wallSlideParticles.transform.localPosition = tempContactPoint;
        }

        if (other.gameObject.CompareTag("Ground") && onWall) {
            StopWallSlide();
        }
    }
}