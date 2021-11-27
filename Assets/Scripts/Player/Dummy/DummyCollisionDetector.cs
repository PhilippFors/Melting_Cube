using System.Collections.Generic;
using UnityEngine;

public class DummyCollisionDetector : MonoBehaviour
{
    public List<Vector3> collisionPoints = new List<Vector3>();

    private void OnCollisionEnter(Collision other)
    {
        collisionPoints.Add(transform.position);
    }
}