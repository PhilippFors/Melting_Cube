using System.Collections;
using UnityEngine;
using UsefulCode.SOArchitecture;

namespace Entities.Player.PlayerInput
{
    public class MeltingController : MonoBehaviour
    {
        public float CurrentSize
        {
            get => currentSize.Value;
            set => currentSize.Value = value;
        }

        public bool isDummy;
        public float meltOverDistanceAmount = 0.2f;
        [HideInInspector] public float startSize;
        [SerializeField] private BoxCollider col;
        [SerializeField] private bool setSizeOnImpact;
        [SerializeField] private float minMass = 0.5f;
        [SerializeField] private float maxMass = 1.5f;
        [SerializeField] private float minSize = 0.1f;
        [SerializeField] private float maxSize = 2f;
        [SerializeField] private GameObject visual;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private bool stopMeltOnCollision = true;
        [SerializeField] private FloatVariable currentSize;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private GameObject hitter;

        private Vector3 oldPosition;
        public Vector3 startScale;
        private float sizeDiff;
        private bool dead;
        private void Start()
        {
            Init();
        }

        public void Init()
        {
            CurrentSize = 1;
            startSize = CurrentSize;

            if (!isDummy) {
                startScale = visual.transform.localScale;
            }
            else {
                startScale = transform.localScale;
            }

            oldPosition = transform.position;

            SetMass();
        }

        private void Update()
        {
            if (!isDummy) {
                if (stopMeltOnCollision && !playerController.HasCollided) {
                    MeltOverDistance();
                }
                else if (!stopMeltOnCollision) {
                    MeltOverDistance();
                }
                else if (playerController.HasCollided) {
                    oldPosition = transform.position;
                }
            }
        }

        private float Remap(float from1, float to1, float from2, float to2, float value) =>
            from2 + (value - from1) * (to2 - from2) / (to1 - from1);


        private void SetMass()
        {
            if (rb) {
                rb.mass = Remap(minSize, maxSize, minMass, maxMass, CurrentSize);
                Debug.Log($"Mass: {rb.mass}");
            }
        }

        private void MeltOverDistance()
        {
            var newPos = transform.position;

            var diff = Vector3.Distance(oldPosition, transform.position) * meltOverDistanceAmount / 10;
            if (diff > 0.00001f) {
                if (!setSizeOnImpact) {
                    currentSize.Value -= diff;
                    SetScale();
                }
                else {
                    sizeDiff += diff;
                }
            }

            oldPosition = newPos;
        }

        public void AddSize(float value)
        {
            currentSize.Value += value;
            currentSize.Value = Mathf.Clamp(CurrentSize, 0, maxSize);
            SetScale();
        }

        private void SetScale()
        {
            if (setSizeOnImpact) {
                currentSize.Value -= sizeDiff;
            }

            var newScale = (startSize * currentSize.Value) * startScale;
            if (newScale.x > 0.1f) {
                if (!isDummy) {
                    visual.transform.localScale = newScale;
                    hitter.transform.localScale = newScale * 1.5f;
                    col.size = newScale;
                }
                else {
                    transform.localScale = newScale;
                }
            }
            
            if (currentSize.Value <= 0f && !dead) {
                dead = true;
                Debug.Log("GameOver lol");
                OnDeath();
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.transform.CompareTag("Ground") || other.transform.CompareTag("Wall")) {
                SetMass();
                if (setSizeOnImpact) {
                    SetScale();
                }
                transform.position += -other.contacts[0].normal * sizeDiff;
                sizeDiff = 0;
            }
        }

        public void ForceMelt(Vector3 currentPos, Vector3 oldPos)
        {
            oldPosition = oldPos;
            transform.position = currentPos;
            var diff = Vector3.Distance(oldPosition, transform.position) * meltOverDistanceAmount / 10;
            if (diff > 0.0001) {
                CurrentSize -= diff;
            }
        }

        public void OnDeath()
        {
            StartCoroutine(DeathAnim());
        }

        private IEnumerator DeathAnim()
        {
            playerController.StopWallSlide(true);
            rb.velocity = Vector3.zero;
            yield return new WaitForSeconds(0.5f);
            SceneLoader.Instance.ReloadScene();
        }
    }
}