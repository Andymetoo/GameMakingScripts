using UnityEngine;

public class Trashcan : MonoBehaviour
{
    private ToolActions toolActions;

    void Start()
    {
        toolActions = FindObjectOfType<ToolActions>();
        if (toolActions == null)
        {
            Debug.LogError("ToolActions script not found in the scene.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TrashItem trashItem = other.GetComponent<TrashItem>();
        if (trashItem != null)
        {
            if (trashItem.IsHeld)
            {
                toolActions.ForceDropItem(); // Force drop the item before disposing
            }
            trashItem.Dispose();
        }
    }
}

