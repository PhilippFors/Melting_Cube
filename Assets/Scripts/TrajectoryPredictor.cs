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
    private Scene simScene;
    private PhysicsScene physScene;
    private Vector3 oldvel;
    private Vector3 oldPos;
    private void Start()
    {
        CreatePhysicsScene();
    }

    private void CreatePhysicsScene()
    {
        simScene = SceneManager.CreateScene("PhysSim", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        physScene = simScene.GetPhysicsScene();

        foreach (Transform obj in obstacleParent) {
            var ghostObj = Instantiate(obj.gameObject, obj.transform.position, obj.transform.rotation);
            ghostObj.GetComponent<Renderer>().enabled = false;
            SceneManager.MoveGameObjectToScene(ghostObj, simScene);
        }
    }

    public void SimulateTrajectory(GameObject player, Vector3 startPos, Vector3 vel)
    {
        if (vel != oldvel) {
            var ghostObj = Instantiate(dummyPlayer, startPos, player.transform.rotation);
            ghostObj.transform.localScale = player.transform.localScale;
            ghostObj.GetComponent<Renderer>().enabled = false;
            SceneManager.MoveGameObjectToScene(ghostObj, simScene);
            ghostObj.GetComponent<Rigidbody>().AddForce(vel, ForceMode.Impulse);
            oldPos = ghostObj.transform.position;
            lineRenderer.positionCount = iterations;

            for (int i = 0; i < iterations; i++) {
                physScene.Simulate(timeStep);
                var diff = Vector3.Distance(oldPos, ghostObj.transform.position) * 0.1f / 10;
                var newScale = ghostObj.transform.localScale - new Vector3(diff, diff, diff);
                ghostObj.transform.localScale = newScale;
                lineRenderer.SetPosition(i, ghostObj.transform.position);
                oldPos = ghostObj.transform.position;
            }
            
            Debug.Log($"EndScale: {ghostObj.transform.localScale}");
            Destroy(ghostObj);
        }

        oldvel = vel;
    }
}
