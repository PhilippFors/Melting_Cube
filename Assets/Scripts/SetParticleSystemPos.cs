using UnityEngine;

namespace DefaultNamespace
{
    public class SetParticleSystemPos : MonoBehaviour
    {
        public ParticleSystem wallSlideParticles;

        private void OnCollisionEnter(Collision other)
        {
            var contact = other.GetContact(0);
            var z = 0f;
            if (contact.point.z > transform.position.z) {
                z = transform.position.z + transform.localScale.z;
            }
            else {
                z = transform.position.z - transform.localScale.z;
            }

            var tempContactPoint = new Vector3(transform.position.x, transform.position.y, z);
            wallSlideParticles.transform.localPosition = tempContactPoint;
        }
    }
}