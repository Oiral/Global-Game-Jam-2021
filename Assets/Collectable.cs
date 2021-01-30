using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            CollectableManager.instance.CollectedItem(this);
        }
    }

    private void Start()
    {
        CollectableManager.instance.AddCollectableToWorld(this);
    }
}
