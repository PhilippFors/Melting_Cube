using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Entities.Player.PlayerInput;
using UnityEngine;
using UnityEngine.SceneManagement;
using UsefulCode.Utilities;

public class TrajectoryPredictor : SingletonBehaviour<TrajectoryPredictor>
{
    [SerializeField] private Transform obstacleParent;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int iterations = 50;
    [SerializeField] private GameObject dummyPlayer;
    [SerializeField] private float timeStep = 0.033333f;
    [SerializeField] private int batchSize = 3;

    private Scene simScene;
    private PhysicsScene physScene;
    private Vector3 oldvel;
    private Vector3 oldPos;
    private int currentBatch;
    public bool isRunning;
    private List<Vector3> positions = new List<Vector3>();

    private void Start()
    {
        CreatePhysicsScene();
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
            }
        }
    }

    public async UniTaskVoid SimulateTrajectory(GameObject player, Vector3 startPos, Vector3 vel)
    {
        if (vel != oldvel) {
            isRunning = true;
            var ghostObj = Instantiate(dummyPlayer, startPos, player.transform.rotation);
            ghostObj.transform.localScale = player.transform.localScale;
            ghostObj.GetComponent<Renderer>().enabled = false;
            SceneManager.MoveGameObjectToScene(ghostObj, simScene);

            var meltingController = player.GetComponent<MeltingController>();
            var dummyMeltingController = ghostObj.GetComponent<MeltingController>();
            var dummyCollisionDetector = ghostObj.GetComponent<DummyCollisionDetector>();
            var ignoreCollision = true;
            dummyMeltingController.isDummy = true;
            dummyMeltingController.Init();
            
            ghostObj.GetComponent<Rigidbody>().AddForce(vel, ForceMode.Impulse);
            oldPos = ghostObj.transform.position;
            
            currentBatch = 0;
            int positionCount = 0;
            for (int i = 0; i < iterations; i++) {
                positionCount++;
                physScene.Simulate(timeStep);
                var hasCollided = dummyCollisionDetector.hasCollided;
                
                dummyMeltingController.ForceMelt();
                
                positions.Add(ghostObj.transform.position);
                
                if (hasCollided && !ignoreCollision) {
                    positionCount--;
                    positions[positionCount] = ghostObj.transform.position;
                    break;
                }

                oldPos = ghostObj.transform.position;
                currentBatch++;
                if (currentBatch > batchSize) {
                    await UniTask.Yield();
                    currentBatch = 0;
                }

                if (i == batchSize) {
                    dummyCollisionDetector.hasCollided = false;
                    ignoreCollision = false;
                }
            }

            lineRenderer.positionCount = positionCount;
            
            for (int i = 0; i < positionCount; i++) {
                lineRenderer.SetPosition(i, positions[i]);
            }

            Debug.Log($"EndScale: {dummyMeltingController.CurrentSize}");
            positions.Clear();
            Destroy(ghostObj);
            isRunning = false;
        }

        oldvel = vel;
    }
}