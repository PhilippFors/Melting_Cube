using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Player.PlayerInput
{
    public class MeltingController : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private bool meltOnce;
        [SerializeField] private float meltOverDistanceAmount;
        [SerializeField] private float meltOnceAmount;
        private Vector3 oldPosition;

        private void Start()
        {
            oldPosition = transform.position;
            if (playerController) {
                playerController.onRelase += MeltOnce;
            }

        }

        private void OnDisable()
        {
            if (playerController) {
                playerController.onRelase -= MeltOnce;
            }
        }

        private void Update()
        {
            MeltOverDistance();
            if (transform.localScale.x < 0.1f) {
                Debug.Log("GameOver lol");
                ResetMelt();
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
                var newScale = transform.localScale - new Vector3(diff, diff, diff);
                if (!(transform.localScale.x < 0.1f)) {
                    transform.localScale = newScale;
                }
            }

            oldPosition = newPos;
        }

        [Button]
        private void ResetMelt()
        {
            transform.localScale = Vector3.one;
        }
    }
}