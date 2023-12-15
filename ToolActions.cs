using UnityEngine;

public class ToolActions : MonoBehaviour
{
    public enum ToolType
    {
        None,
        Broom,
        SprayBottle,
        Cloth,
        HandGrabber,
        WateringCan
    }

    [SerializeField] private ToolType currentTool = ToolType.None;
    [SerializeField] private Transform toolTransform;
    [SerializeField] private LayerMask smudgeLayer; // Layer for the smudges
    [SerializeField] private ParticleSystem primarySprayParticles;
    [SerializeField] private ParticleSystem secondarySprayParticles;
    [SerializeField] private ParticleSystem waterParticleSystem;

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
            case ToolType.WateringCan:
                WateringCanAction();
                break;
        }
        if (currentTool == ToolType.Cloth)
            {
                ClothAction();
            }
            
    }

    private bool IsActionKeyPressed()
    {
        return Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E);
    }

    private bool IsActionKeyReleased()
    {
        return Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.E);
    }

    private bool IsActionButtonHeld()
{
    return Input.GetMouseButton(0) || Input.GetKey(KeyCode.E);
}


    void BroomAction()
    {
        // Implement broom functionality
    }

    void SprayAction()
    {
        if (IsActionKeyPressed())
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
        if (IsActionKeyPressed())
        {
            isClothActive = true; // Start cloth action
        }
        else if (IsActionKeyReleased())
        {
            isClothActive = false; // Stop cloth action
        }

        if (isClothActive)
        {
            PerformClothRaycast();
        }
    }


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
        // Check if the player is currently interacting with a bed
        BedInteraction bed = potentialPickupItem?.GetComponent<BedInteraction>();
        
        if (IsActionButtonHeld())
        {
            if (bed != null)
            {
                // Start making the bed only if the potential pickup item is a bed
                bed.StartMaking();
            }
            else if (heldItem == null && potentialPickupItem != null && potentialPickupItem.CompareTag("Trash"))
            {
                // If it's not a bed and we're not holding an item, try to pick up trash
                PickUpItem(potentialPickupItem);
            }
        }
        else
        {
            if (bed != null)
            {
                // Stop making the bed if the action button is released
                bed.StopMaking();
            }
            else if (heldItem != null)
            {
                // If we're not making a bed and holding an item, drop the trash
                DropTrash();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (currentTool == ToolType.HandGrabber)
        {
            if (other.CompareTag("Trash") || other.CompareTag("Bed"))
            {
                potentialPickupItem = other.gameObject;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (currentTool == ToolType.HandGrabber && other.gameObject == potentialPickupItem)
        {
            potentialPickupItem = null;
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

    void WateringCanAction() 
    {
        if (IsActionButtonHeld()) 
        {

            // Start playing water particle system
            if (waterParticleSystem != null) // Check if waterParticleSystem is assigned
            {
                waterParticleSystem.Play();
                PerformWateringRaycasts();
                Debug.Log("Watering can is on.");
            }
        } 
        else 
        {
            // Stop playing water particle system
            if (waterParticleSystem != null) // Check if waterParticleSystem is assigned
            {
                waterParticleSystem.Stop();
            }
        }
    }

    void PerformWateringRaycasts() {
    // Similar to SprayWater() but checking for plants
    }

    private void PlayParticleSystem(ParticleSystem ps)
    {
        if (ps != null)
        {
            ps.Play();
        }
    }
}
