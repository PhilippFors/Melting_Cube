using System.Collections.Generic;
using Entities.Player.PlayerInput;
using UnityEngine;
using UsefulCode.Utilities;

/// <summary>
/// Calculates a list of points base on velocity and gravity.
/// </summary>
public class NewTrajectoryPredictor : SingletonBehaviour<NewTrajectoryPredictor>
{
    public GameObject previewPlayer;
    public GameObject dummyPrefab;
    [SerializeField] private float timeStep = 0.0333f;
    [SerializeField] private int iterations = 10;
    [SerializeField] private LineRenderer lineRenderer;

    private GameObject previewVisual;
    private GameObject dummy;
    private bool isInstantiated;

    private Vector3[] oldpos;
    // Called in Player Controller
    public void Simulate(GameObject player, Vector3 initVel, Vector3 force)
    {
        var tempPos = new List<Vector3>();
        var tempScales = new List<Vector3>();
        var tempSizes = new List<float>();
        var meltingController = player.GetComponent<MeltingController>();

        if (!isInstantiated) {
            previewVisual = previewPlayer.GetComponentInChildren<MeshRenderer>().gameObject;
            dummy = Instantiate(dummyPrefab, player.transform.position, player.transform.rotation);
            isInstantiated = true;
        }

        var rb = player.GetComponent<Rigidbody>();
        var dummyController = dummy.GetComponent<MeltingController>();
        
        dummyController.CurrentSize = meltingController.CurrentSize;
        dummyController.startScale = meltingController.startScale;
        dummyController.startSize = meltingController.startSize;
        dummyController.meltOverDistanceAmount = meltingController.meltOverDistanceAmount;
        dummy.transform.localScale = player.GetComponentInChildren<MeshRenderer>().transform.localScale;

        var sizeCollectables = new List<SizeCollectable>();
        var startPoint = player.transform.position;
        var previousPoint = startPoint;

        dummyController.ForceMelt(startPoint, startPoint);
        
        tempSizes.Add(dummyController.CurrentSize);
        for (int i = 0; i < iterations; i++) {
            var point = GetPoint(startPoint, initVel + force, rb.mass, (i + 2) * timeStep);
            dummyController.ForceMelt(point, previousPoint);
            previousPoint = point;
            
            var cols = Physics.OverlapBox(dummyController.transform.position, dummyController.transform.localScale / 2,
                Quaternion.identity, LayerMask.GetMask("Default"));

            if (cols.Length > 0) {
                for (int j = 0; j < cols.Length; j++) {
                    var sizeCol = cols[j].GetComponent<SizeCollectable>();
                    if (sizeCol && !sizeCollectables.Contains(sizeCol)) {
                        dummyController.AddSize(sizeCol.addSize);
                        sizeCollectables.Add(sizeCol);
                    }
                }
            }
            
            tempScales.Add(dummyController.transform.localScale);
            tempSizes.Add(dummyController.CurrentSize);
            tempPos.Add(point);
        }

        
        var pos = new List<Vector3>();
        float size = 0;
        for (int i = 0; i < tempPos.Count; i++) {
            var scale = tempScales[i] / 2 - new Vector3(0.05f, 0.05f, 0.05f);
            var tempScale = scale.x < 0 ? Vector3.zero : scale;
            if (!Physics.CheckBox(tempPos[i], tempScale, Quaternion.identity, LayerMask.GetMask("Ground"))) {
                pos.Add(tempPos[i]);
                size = tempSizes[i];
            }
            else {
                break;
            }
        }

        dummyController.CurrentSize = size;
        var newScale = (dummyController.startSize * size) * dummyController.startScale;
        previewVisual.transform.localScale = newScale;
        lineRenderer.positionCount = pos.Count;

        if (pos.Count > 1) {
            previewPlayer.transform.position = pos[pos.Count - 1];
        }
        
        lineRenderer.SetPositions(pos.ToArray());
    }

    public void EnableTrajectory()
    {
        lineRenderer.enabled = true;
        previewPlayer.SetActive(true);
    }

    public void DisableTrajectory()
    {
        lineRenderer.enabled = false;
        previewPlayer.SetActive(false);
        lineRenderer.positionCount = 0;
    }

    private Vector3 GetPoint(Vector3 start, Vector3 force, float mass, float t) =>
        (start + force / mass * t) + Physics.gravity * (t * t) / 2;
}