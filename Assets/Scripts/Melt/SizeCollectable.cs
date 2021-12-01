using Entities.Player.PlayerInput;
using UnityEngine;

public class SizeCollectable : MonoBehaviour
{
    public float addSize;

    private void OnTriggerEnter(Collider other)
    {
        var meltythingy = other.GetComponentInParent<MeltingController>();
        if (meltythingy && !meltythingy.isDummy) {
            meltythingy.AddSize(addSize);
            Destroy(gameObject);
        }
    }
}