using UnityEngine;
using UsefulCode.Utilities;

public class NewTrajectoryPredictor : SingletonBehaviour<NewTrajectoryPredictor>
{
    [SerializeField] private float timeStep = 0.0333f;
    [SerializeField] private int iterations = 10;
    [SerializeField] private LineRenderer lineRenderer;

    // Called in Player Controller
    public void Simulate(GameObject player, Vector3 initVel, Vector3 force)
    {
        var pos = new Vector3[iterations];
        lineRenderer.positionCount = iterations;
        var prevPoint = player.transform.position;
        for (int i = 0; i < iterations; i++) {
            var point = GetPoint(prevPoint, initVel + force, i * timeStep);
            pos[i] = point;
        }

        lineRenderer.SetPositions(pos);
    }

    public void Enable()
    {
        lineRenderer.enabled = true;
    }

    public void Disable()
    {
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
    }

    private Vector3 GetPoint(Vector3 start, Vector3 force, float t) => (start + force * t) + Physics.gravity * (0.5f * Mathf.Pow(t, 2));
}