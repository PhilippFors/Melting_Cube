using UnityEngine;

namespace DefaultNamespace
{
    public class SetParticleSystemPos : MonoBehaviour
    {
        public ParticleSystem wallSlideParticles;
        private void OnCollisionEnter(Collision other)
        {
            var contact = other.GetContact(0);
            var dir = contact.point - transform.position;
            var cross = Vector3.Cross(dir, contact.normal);
            var newCross = new Vector3(cross.x, wallSlideParticles.transform.rotation.y, wallSlideParticles.transform.rotation.z);
            var particleRot = Quaternion.LookRotation(newCross, Vector3.up);
            var lccalContactPoint = transform.InverseTransformPoint(contact.point);
            var tempContactPoint = new Vector3(wallSlideParticles.transform.localPosition.x, wallSlideParticles.transform.localPosition.y, lccalContactPoint.z);
            wallSlideParticles.transform.localPosition = tempContactPoint;
        }
    }
}