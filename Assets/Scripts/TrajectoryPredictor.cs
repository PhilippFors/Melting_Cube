using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
                ghostObj.GetComponent<Renderer>().enabled = false;
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
            ghostObj.GetComponent<Rigidbody>().AddForce(vel, ForceMode.Impulse);
            oldPos = ghostObj.transform.position;
            lineRenderer.positionCount = iterations;
            
            positions.Clear();
            currentBatch = 0;
            for (int i = 0; i < iterations; i++) {
                physScene.Simulate(timeStep);
                var diff = Vector3.Distance(oldPos, ghostObj.transform.position) * 0.1f / 10;
                var newScale = ghostObj.transform.localScale - new Vector3(diff, diff, diff);
                ghostObj.transform.localScale = newScale;
                positions.Add(ghostObj.transform.position);
                oldPos = ghostObj.transform.position;
                currentBatch++;
                if (currentBatch > batchSize) {
                    await UniTask.Yield();
                    currentBatch = 0;
                }
            }

            for (int i = 0; i < positions.Count; i++) {
                lineRenderer.SetPosition(i, positions[i]);
            }

            Debug.Log($"EndScale: {ghostObj.transform.localScale}");
            Destroy(ghostObj);
            isRunning = false;
        }

        oldvel = vel;
    }
}