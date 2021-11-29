using System.Collections.Generic;
using Entities.Player.PlayerInput;
using UnityEngine;
using UsefulCode.Utilities;

public class NewTrajectoryPredictor : SingletonBehaviour<NewTrajectoryPredictor>
{
    public GameObject previewPlayer;
    public GameObject dummyPrefab;
    [SerializeField] private float timeStep = 0.0333f;
    [SerializeField] private int iterations = 10;
    [SerializeField] private LineRenderer lineRenderer;

    private GameObject dummy;
    private bool isInstantiated;

    // Called in Player Controller
    public void Simulate(GameObject player, Vector3 initVel, Vector3 force)
    {
        var tempPos = new List<Vector3>();
        var meltingController = player.GetComponent<MeltingController>();
        
        if (!isInstantiated) {
            dummy = Instantiate(dummyPrefab, player.transform.position, player.transform.rotation);
            isInstantiated = true;
        }
        
        var dummyController = dummy.GetComponent<MeltingController>();
        dummyController.CurrentSize = meltingController.CurrentSize;
        dummyController.startScale = meltingController.startScale;
        dummyController.maxSize = meltingController.maxSize;
        dummyController.meltOverDistanceAmount = meltingController.meltOverDistanceAmount;
        dummy.transform.localScale = player.transform.localScale;

        var sizeCollectables = new List<SizeCollectable>();
        var startPoint = player.transform.position;
        var previousPoint = startPoint;

        dummyController.ForceMelt(startPoint, startPoint);
        for (int i = 0; i < iterations; i++) {
            var point = GetPoint(startPoint, initVel + force, (i + 2) * timeStep);
            dummyController.ForceMelt(point, previousPoint);
            previousPoint = point;
            
            var cols = Physics.OverlapBox(dummyController.transform.position, dummyController.transform.localScale / 2,
                Quaternion.identity, LayerMask.GetMask("Default"));

            if (cols.Length > 0) {
                for (int j = 0; j < cols.Length;j++) {
                    var sizeCol = cols[j].GetComponent<SizeCollectable>();
                    if (sizeCol && !sizeCollectables.Contains(sizeCol)) {
                        dummyController.AddSize(sizeCol.addSize);
                        sizeCollectables.Add(sizeCol);
                    }
                }
            }
        
            if (Physics.CheckBox(point, dummyController.transform.localScale / 2, Quaternion.identity,
                LayerMask.GetMask("Ground"))) {
                break;
            }

            tempPos.Add(point);
        }

        var pos = tempPos.ToArray();
        lineRenderer.positionCount = pos.Length;
        var newSize = dummyController.CurrentSize;
        var newScale = (dummyController.maxSize * newSize) * dummyController.startScale;
        previewPlayer.transform.localScale = newScale;
        
        if (pos.Length > 1) {
            previewPlayer.transform.position = pos[pos.Length - 1];
        }

        lineRenderer.SetPositions(pos);
    }

    public void Enable()
    {
        lineRenderer.enabled = true;
        previewPlayer.SetActive(true);
    }

    public void Disable()
    {
        lineRenderer.enabled = false;
        previewPlayer.SetActive(false);
        lineRenderer.positionCount = 0;
    }

    private Vector3 GetPoint(Vector3 start, Vector3 force, float t) =>
        (start + force * t) + Physics.gravity * (0.5f * Mathf.Pow(t, 2));
}