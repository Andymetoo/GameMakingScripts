using UnityEngine;

public class TrashItem : MonoBehaviour
{
    //[SerializeField] private int pointsValue = 10;
    private Task taskComponent; // Reference to the Task component
    public Room room; // Assign this in the inspector

    void Start()
    {
        // Randomize rotation
        float randomYRotation = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, randomYRotation, 0f);

        // Randomize scale
        Vector3 currentScale = transform.localScale;
        transform.localScale = new Vector3(
            currentScale.x * Random.Range(1f, 1.2f), // Randomize X scale
            currentScale.y * Random.Range(1f, 1.2f), // Randomize Y scale
            currentScale.z * Random.Range(1f, 1.2f)  // Randomize Z scale
        );

        taskComponent = GetComponent<Task>(); // Get the Task component
    }

    public void Dispose()
    {
        if (taskComponent != null)
        {
            taskComponent.isActive = false; // Update task status
        }

        if (room != null)
        {
            room.UpdateCompletionUI(); // Update room completion
        }

        Destroy(gameObject);
    }

    /*private void PlayDisposalEffects()
    {
        // Implement any visual or audio effects for disposing of the trash
    }*/
}
