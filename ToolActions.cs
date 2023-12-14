using UnityEngine;

public class ToolActions : MonoBehaviour
{
    public enum ToolType
    {
        None,
        Broom,
        SprayBottle,
        Cloth,
        HandGrabber
    }

    [SerializeField] private ToolType currentTool = ToolType.None;
    [SerializeField] private Transform toolTransform;
    [SerializeField] private LayerMask smudgeLayer; // Layer for the smudges
    [SerializeField] private ParticleSystem primarySprayParticles;
    [SerializeField] private ParticleSystem secondarySprayParticles;

    private GameObject heldItem = null; 
    private Animator handAnimator;
    private bool isClothActive = false; // Added this line
    private GameObject potentialPickupItem = null; // New field to store potential item for pickup

    void Start()
    {
        if (currentTool == ToolType.HandGrabber) 
        {
            handAnimator = toolTransform.GetComponent<Animator>();
        }
    }

    void Update()
    {
        switch (currentTool)
        {
            case ToolType.Broom:
                BroomAction();
                break;
            case ToolType.SprayBottle:
                SprayAction();
                break;
            case ToolType.Cloth:
                ClothAction();
                break;
            case ToolType.HandGrabber:
                HandGrabberAction();
                break;
        }
        if (currentTool == ToolType.Cloth)
            {
                ClothAction();
            }
    }

    void BroomAction()
    {
        // Implement broom functionality
    }

    void SprayAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlayParticleSystem(primarySprayParticles);
            PlayParticleSystem(secondarySprayParticles);
            SprayWater();   // Call SprayWater to handle the logic for making smudges wet
        }



    }

    void SprayWater()
    {
        int numberOfRays = 5; // Number of rays in the spray
        float maxAngle = 10f; // Maximum deviation angle in degrees

        for (int i = 0; i < numberOfRays; i++)
        {
            // Calculate a slightly random direction within the cone
            Vector3 deviation = UnityEngine.Random.insideUnitCircle * maxAngle;
            deviation.z = 0;
            Quaternion rotation = Quaternion.Euler(deviation);
            Vector3 rayDirection = rotation * -toolTransform.forward;

            RaycastHit hit;
            if (Physics.Raycast(toolTransform.position, rayDirection, out hit, 1f, smudgeLayer))
            {
                WindowSmudge smudge = hit.collider.GetComponent<WindowSmudge>();
                if (smudge != null)
                {
                    smudge.MakeWet();
                }
            }

            // Debugging the raycast (optional)
            Debug.DrawRay(toolTransform.position, rayDirection * 1f, Color.blue, 1f);
        }
    }

    void ClothAction()
{
    if (Input.GetMouseButtonDown(0))
    {
        isClothActive = true; // Start cloth action
    }
    else if (Input.GetMouseButtonUp(0))
    {
        isClothActive = false; // Stop cloth action
    }

    if (isClothActive)
    {
        PerformClothRaycast();
    }
}

    /* void ClothAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 rayOrigin = toolTransform.position + new Vector3(0, 0, 0f); // Adjust the offset
            Vector3 rayDirection = -toolTransform.forward;
            float rayLength = 1f; // Same as the raycast length

            // Draw the ray for debugging
            Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.green, 1f);

            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength, smudgeLayer))
            {
                WindowSmudge smudge = hit.collider.GetComponent<WindowSmudge>();
                if (smudge != null && smudge.IsWet)
                {
                    smudge.StartSweeping();
                }
            }
        }
    } */

    void PerformClothRaycast()
    {
        Vector3 rayOrigin = toolTransform.position + new Vector3(0, 0, 0f); // Adjust the offset
        Vector3 rayDirection = -toolTransform.forward;
        float rayLength = 1f; // Same as the raycast length

        // Draw the ray for debugging
        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.green, 1f);

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength, smudgeLayer))
            {
                WindowSmudge smudge = hit.collider.GetComponent<WindowSmudge>();
                if (smudge != null && smudge.IsWet)
                {
                    smudge.StartSweeping();
                }
            }
    }



    void HandGrabberAction()
    {
        if (Input.GetMouseButtonDown(0) && heldItem == null && potentialPickupItem != null)
        {
            // Pick up the item when mouse button is pressed down
            PickUpItem(potentialPickupItem);
            if (handAnimator != null)
                handAnimator.Play("Hand", -1, 0f); // Play hand grabbing animation
        }

        if (Input.GetMouseButtonUp(0) && heldItem != null)
        {
            // Drop the item when mouse button is released
            DropTrash();
            if (handAnimator != null)
                handAnimator.Play("Hand", -1, 1f); // Play hand opening animation in reverse
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (currentTool == ToolType.HandGrabber && other.CompareTag("Trash"))
        {
            potentialPickupItem = other.gameObject; // Mark as potential pickup item
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (currentTool == ToolType.HandGrabber && other.gameObject == potentialPickupItem)
        {
            potentialPickupItem = null; // Clear the potential pickup item
        }
    }

    void PickUpItem(GameObject item)
    {
        heldItem = item;
        heldItem.transform.SetParent(toolTransform);
        heldItem.transform.localPosition = new Vector3(0, 0, 10);

        Rigidbody rb = heldItem.GetComponent<Rigidbody>();
        Collider col = heldItem.GetComponent<Collider>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }
        if (col != null)
        {
            col.enabled = false; // Disable collider to prevent pushing
        }
    }



    void DropTrash()
    {
        if (heldItem != null)
        {
            Rigidbody rb = heldItem.GetComponent<Rigidbody>();
            Collider col = heldItem.GetComponent<Collider>(); // Get the collider

            if (rb != null)
            {
                rb.isKinematic = false; // Re-enable physics
            }
            if (col != null)
            {
                col.enabled = true; // Re-enable the collider
            }

            heldItem.transform.SetParent(null); // Release the item
            heldItem = null;
            potentialPickupItem = null; // Reset potential item for pickup
        }
    }


    private void PlayParticleSystem(ParticleSystem ps)
    {
        if (ps != null)
        {
            ps.Play();
        }
    }
}
