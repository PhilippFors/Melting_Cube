using Entities.Player.PlayerInput;
using UnityEngine;

public class SizeCollectable : MonoBehaviour
{
    [SerializeField] private float addSize;
    private void OnTriggerEnter(Collider other)
    {
        var meltythingy = other.GetComponent<MeltingController>();
        if (meltythingy && meltythingy.isDummy) {
            meltythingy.AddSize(addSize);
            gameObject.SetActive(false);
        } else if (meltythingy && !meltythingy.isDummy) {
            meltythingy.AddSize(addSize);
            Destroy(gameObject);
        }
    }
}
