using UnityEngine;
using System;

public class TrashItem : MonoBehaviour
{
    //[SerializeField] private int pointsValue = 10;
    private Task taskComponent; // Reference to the Task component
    public Room room; // Assign this in the inspector
    public event Action OnDisposal;
    private float pickupCooldown = 1.0f; // Cooldown duration in seconds
    private float currentCooldown;

    void Start()
    {
        // Randomize rotation
        float randomYRotation = UnityEngine.Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, randomYRotation, 0f);

        // Randomize scale
        Vector3 currentScale = transform.localScale;
        transform.localScale = new Vector3(
            currentScale.x * UnityEngine.Random.Range(1f, 1.2f), // Randomize X scale
            currentScale.y * UnityEngine.Random.Range(1f, 1.2f), // Randomize Y scale
            currentScale.z * UnityEngine.Random.Range(1f, 1.2f)  // Randomize Z scale
        );

        taskComponent = GetComponent<Task>(); // Get the Task component
    }

    void Update()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }
    }

    public void Drop()
    {
        // Reset the cooldown when the trash is dropped
        currentCooldown = pickupCooldown;
    }

    public bool IsPickupAllowed()
    {
        return currentCooldown <= 0;
    }

    public void Dispose()
    {
        
        OnDisposal?.Invoke();

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
