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
        private float lastSize;
        private float sizeDiff;
        
        private void Start()
        {
            Init();
        }

        public void Init()
        {
            CurrentSize = 1;
            lastSize = CurrentSize;
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

                if (currentSize.Value <= 0f) {
                    Debug.Log("GameOver lol");
                    OnDeath();
                }
            }
        }

        private float Remap(float from1, float to1, float from2, float to2, float value) =>
            from2 + (value - from1) * (to2 - from2) / (to1 - from1);
        

        private void SetMass()
        {
            if (rb) {
                rb.mass = Remap(minSize, maxSize, minMass, maxMass, CurrentSize);
                Debug.Log(rb.mass);
            }
        }
        private void MeltOverDistance()
        {
            var newPos = transform.position;

            var diff = Vector3.Distance(oldPosition, transform.position) * meltOverDistanceAmount / 10;
            if (diff > 0.0001) {
                sizeDiff += diff;
                // currentSize.Value -= diff;

                // var newScale = (maxSize * currentSize.Value) * startScale;
                // if (newScale.x > 0.1f) {
                //     SetScale();
                // }
            }

            oldPosition = newPos;
        }

        public void AddSize(float value)
        {
            currentSize.Value += value;
            SetScale();
        }

        private void SetScale()
        {
            currentSize.Value -= sizeDiff;
            sizeDiff = 0;
            var newScale = (startSize * currentSize.Value) * startScale;
            if (newScale.x > 0.1f) {
                if (!isDummy) {
                    visual.transform.localScale = newScale;
                    hitter.transform.localScale = newScale * 1.5f;

                }
                else {
                    transform.localScale = newScale;
                }
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.transform.CompareTag("Ground") || other.transform.CompareTag("Wall")) {
                if (sizeDiff > 0.05f) {
                    SetMass();
                    SetScale();
                    lastSize = CurrentSize;
                }
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

        private void OnDeath()
        {
            SceneLoader.Instance.ReloadScene();
        }
    }
}