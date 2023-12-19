using UnityEngine;

public class Trashcan : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        TrashItem trashItem = other.GetComponent<TrashItem>();
        if (trashItem != null)
        {
            trashItem.Dispose();
        }
    }
}
