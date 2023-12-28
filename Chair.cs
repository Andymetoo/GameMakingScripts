using UnityEngine;

public class Chair : MonoBehaviour
{
    private Quaternion targetRotation; // Upright rotation
    private bool isBeingFixed = false;
    private float rotationSpeed = 2f; // Speed of rotation adjustment

    void Start()
    {
        // Initialize targetRotation to upright position
        targetRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    void Update()
    {
        if (isBeingFixed)
        {
            // Rotate towards the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // Check if the chair is close enough to the target rotation
            if (Quaternion.Angle(transform.rotation, targetRotation) < 1.0f)
            {
                transform.rotation = targetRotation; // Snap to exact rotation
                isBeingFixed = false; // Stop fixing process
                // Optionally, trigger completion update here
            }
        }
    }

    // Call this method to start the fixing process
    public void FixChair()
    {
        isBeingFixed = true;
    }
}
