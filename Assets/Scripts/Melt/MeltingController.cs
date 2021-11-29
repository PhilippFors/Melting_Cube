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

        public float meltOverDistanceAmount = 0.2f;
        public bool isDummy;

        [SerializeField] private PlayerController playerController;
        [SerializeField] private bool stopMeltOnCollision = true;
        [SerializeField] private bool meltOnce;
        [SerializeField] private float meltOnceAmount;
        [SerializeField] private FloatVariable currentSize;

        private Vector3 oldPosition;
        public float maxSize = 1;
        public Vector3 startScale;

        private void Start()
        {
            Init();

            if (playerController) {
                playerController.onRelase += MeltOnce;
            }
        }

        public void Init()
        {
            currentSize.Value = maxSize;
            startScale = transform.localScale;
            oldPosition = transform.position;
        }

        private void OnDisable()
        {
            if (playerController) {
                playerController.onRelase -= MeltOnce;
            }
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

        private void MeltOnce()
        {
            if (!meltOnce) {
                return;
            }

            var newScale = transform.localScale - new Vector3(meltOnceAmount, meltOnceAmount, meltOnceAmount);

            transform.localScale = newScale;
        }

        private void MeltOverDistance()
        {
            var newPos = transform.position;


            if (!meltOnce) {
                var diff = Vector3.Distance(oldPosition, transform.position) * meltOverDistanceAmount / 10;
                if (diff > 0.0001) {
                    currentSize.Value -= diff;

                    var newScale = (maxSize * currentSize.Value) * startScale;
                    if (newScale.x > 0.15f) {
                        transform.localScale = newScale;
                    }
                }
            }

            oldPosition = newPos;
        }

        public void AddSize(float value)
        {
            currentSize.Value += value;
            var newScale = (maxSize * currentSize.Value) * startScale;
            transform.localScale = newScale;
        }

        public void ForceMelt(Vector3 currentPos, Vector3 oldPos)
        {
            oldPosition = oldPos;
            transform.position = currentPos;
            MeltOverDistance();
        }

        private void OnDeath()
        {
            SceneLoader.Instance.ReloadScene();
        }
    }
}