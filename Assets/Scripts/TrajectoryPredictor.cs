using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Entities.Player.PlayerInput;
using UnityEngine;
using UnityEngine.SceneManagement;
using UsefulCode.Utilities;

public class TrajectoryPredictor : SingletonBehaviour<TrajectoryPredictor>
{
    public GameObject previewPlayer;
    public float predictiveSize = 1;
    public Transform obstacleParent;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int iterations = 15;
    [SerializeField] private GameObject dummyPlayer;
    [SerializeField] private float timeStep = 0.033333f;
    [SerializeField] private int batchSize = 3;

    private Scene simScene;
    private PhysicsScene physScene;
    private Vector3 oldvel;
    private int currentBatch;
    public bool isRunning;
    public List<Vector3> collisions = new List<Vector3>();
    public List<Vector3> oldPositions = new List<Vector3>();
    private List<GameObject> physSceneObjs = new List<GameObject>();
    private void Start()
    {
        lineRenderer.enabled = false;
        // CreatePhysicsScene();
    }

    private void Update()
    {
        // if (oldPositions.Count > 0 && lineRenderer.positionCount > 0) {
        //     for (int i = 0; i < iterations; i++) {
        //         lineRenderer.SetPosition(i, oldPositions[i]);
        //     }
        // }
        //
        // if (dummyPlayer.activeSelf) {
        //     if (collisions.Count > 1) {
        //         // foreach (var c in collisions) {
        //             if (Vector3.Distance(collisions[collisions.Count -1], transform.position) > 2f) {
        //                 previewPlayer.transform.position = new Vector3(transform.position.x, collisions[collisions.Count -1].y, collisions[collisions.Count -1].z);
        //                 // break;
        //             }
        //         // }
        //     }
        //     else if (oldPositions.Count > 0) {
        //         previewPlayer.transform.position = oldPositions[oldPositions.Count - 1];
        //     }
        // }
    }

    private void SetPreviewSize(MeltingController dummyMelter)
    {
        // var newScale = (dummyMelter.maxSize * predictiveSize) * dummyMelter.startScale;
        // if (previewPlayer.transform.localScale.x > 0.1f) {
        //     previewPlayer.transform.localScale = newScale;
        // }
    }

    public void Enable()
    {
        lineRenderer.enabled = true;
        previewPlayer.SetActive(true);
    }

    public void Disable()
    {
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
        previewPlayer.SetActive(false);
    }

    private void CreatePhysicsScene()
    {
        simScene = SceneManager.CreateScene("PhysSim", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        physScene = simScene.GetPhysicsScene();

        if (obstacleParent) {
            foreach (Transform obj in obstacleParent) {
                var ghostObj = Instantiate(obj.gameObject, obj.transform.position, obj.transform.rotation);
                ghostObj.GetComponentInChildren<Renderer>().enabled = false;
                SceneManager.MoveGameObjectToScene(ghostObj, simScene);
                physSceneObjs.Add(ghostObj);
            }
        }
    }

    public async UniTaskVoid SimulateTrajectory(GameObject player, Vector3 startPos, Vector3 vel)
    {
        if (vel != oldvel) {
            
            foreach (var s in physSceneObjs) {
                var sizecoll = s.GetComponentInChildren<SizeCollectable>();
                if (sizecoll) {
                    s.SetActive(true);
                }
            }
            
            isRunning = true;
            var ghostObj = Instantiate(dummyPlayer, startPos, player.transform.rotation);
            ghostObj.transform.localScale = player.transform.localScale;
            ghostObj.GetComponent<Renderer>().enabled = false;
            SceneManager.MoveGameObjectToScene(ghostObj, simScene);

            var realMeltingController = player.GetComponent<MeltingController>();
            var dummyMeltingController = ghostObj.GetComponent<MeltingController>();
            // dummyMeltingController.maxSize = realMeltingController.maxSize;
            dummyMeltingController.startScale = realMeltingController.startScale;
            dummyMeltingController.meltOverDistanceAmount = realMeltingController.meltOverDistanceAmount;
            
            var dummyCollisionDetector = ghostObj.GetComponent<DummyCollisionDetector>();

            dummyMeltingController.isDummy = true;
            dummyMeltingController.Init();

            ghostObj.GetComponent<Rigidbody>().AddForce(vel, ForceMode.Impulse);

            var positions = new List<Vector3>();

            currentBatch = 0;

            for (int i = 0; i < iterations; i++) {
                physScene.Simulate(timeStep);

                // dummyMeltingController.ForceMelt();

                positions.Add(ghostObj.transform.position);

                currentBatch++;
                if (currentBatch > batchSize) {
                    await UniTask.Yield();
                    currentBatch = 0;
                }
            }

            var cols = new List<Vector3>();
            if (dummyCollisionDetector.collisionPoints.Count == 0) {
                collisions.Clear();
            }
            else {
                foreach (var c in dummyCollisionDetector.collisionPoints) {
                    cols.Add(c);
                }
                collisions = cols;
            }

            lineRenderer.positionCount = iterations;


            oldPositions = positions;

            predictiveSize = dummyMeltingController.CurrentSize;
            SetPreviewSize(dummyMeltingController);

            Destroy(ghostObj);
            isRunning = false;
        }

        oldvel = vel;
    }
}