using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Player.PlayerInput
{
    public class MeltingController : MonoBehaviour
    {
        public float CurrentSize => currentSize;
        public float MeltOverDistanceAmount => meltOverDistanceAmount;

        public bool isDummy;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private bool stopMeltOnCollision;
        [SerializeField] private bool meltOnce;
        [SerializeField] private float meltOverDistanceAmount;
        [SerializeField] private float meltOnceAmount;
        private Vector3 oldPosition;
        [SerializeField] private float currentSize = 1;
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

                if (transform.localScale.x < 0.1f) {
                    Debug.Log("GameOver lol");
                    ResetMelt();
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
                if (diff > 0.001) {
                    currentSize -= diff;

                    var newScale = (maxSize * currentSize) * startScale;
                    if (!(transform.localScale.x < 0.1f)) {
                        transform.localScale = newScale;
                    }
                }
            }

            oldPosition = newPos;
        }

        public void AddSize(float value)
        {
            currentSize += value;
            var newScale = (maxSize * currentSize) * startScale;
            transform.localScale = newScale;
        }
        
        public void ForceMelt()
        {
            MeltOverDistance();
        }

        [Button]
        private void ResetMelt()
        {
            transform.localScale = Vector3.one;
            currentSize = 1;
        }
    }
}