using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private float minSpeed = 0.1f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float minEmissionRate = 0.1f;
    [SerializeField] private float maxEmissionRate = 100f;
    [SerializeField] private Slider waterLevelSlider; // UI Slider for water level
    [SerializeField] private float maxWaterLevel = 100f; // Maximum water level
    [SerializeField] private float waterUsageRate = 0.1f; // Rate at which water is used
    [SerializeField] private GameObject sink; // Assign sink object in the inspector
    [SerializeField] private ParticleSystem sinkParticleSystem; // Assign sink particle system
    [SerializeField] private ToolSwitcher toolSwitcher;


    private GameObject heldItem = null; 
    private Animator handAnimator;
    private bool isClothActive = false; // Added this line
    private GameObject potentialPickupItem = null; // New field to store potential item for pickup
    private float currentWaterLevel;
    private bool isLookingAtSink = false;
    private float wateringCanCooldown = 0.5f; // Cooldown time in seconds
    private float currentCooldownTimer = 0f;

    void Start()
    {
        if (currentTool == ToolType.HandGrabber) 
        {
            handAnimator = toolTransform.GetComponent<Animator>();
        }

            // Initialize water level
        currentWaterLevel = maxWaterLevel;
        waterLevelSlider.maxValue = maxWaterLevel;
        waterLevelSlider.value = currentWaterLevel;

        toolSwitcher = FindObjectOfType<ToolSwitcher>();
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
                AdjustWaterFlowBasedOnToolAngle();
                UpdateWaterLevel();
                HandleWateringCanRefill();
                break;
        }

        if (currentTool == ToolType.Cloth)
            {
                ClothAction();
            }

        HandleToolSwitch();

        if (currentTool == ToolType.WateringCan && toolSwitcher.isSwitching)
        {
            currentCooldownTimer = wateringCanCooldown;
        }

        if (currentCooldownTimer > 0)
        {
            currentCooldownTimer -= Time.deltaTime;
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

    /*void WateringCanAction() 
    {
        if (IsActionButtonHeld() && currentWaterLevel > 0) 
        {

            // Start playing water particle system
            if (waterParticleSystem != null) // Check if waterParticleSystem is assigned
            {
                waterParticleSystem.Play();
                waterLevelSlider.gameObject.SetActive(true);
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
                waterLevelSlider.gameObject.SetActive(false);

            }
        }
    }*/

    private void UpdateWaterLevel()
    {
         if (currentTool == ToolType.WateringCan && toolSwitcher != null && !toolSwitcher.isSwitching)
        {
            if (waterParticleSystem.isPlaying)
            {
                currentWaterLevel -= waterUsageRate * Time.deltaTime;
                waterLevelSlider.value = currentWaterLevel;

                if (currentWaterLevel <= 0)
                {
                    waterParticleSystem.Stop();
                    currentWaterLevel = 0; // Prevent negative values
                }
            }
        }

        // Refill water at a sink
        if (IsAtSink() && IsActionKeyPressed())
        {
            RefillWater();
        }
    }

   private void RefillWater()
    {
        //currentWaterLevel = maxWaterLevel;
        //waterLevelSlider.value = currentWaterLevel;
    }
private bool IsAtSink()
    {
        // Implement logic to check if the player is at a sink
        // For example, you could check for proximity to a sink object
        return true; // Placeholder
    }

    private void HandleWateringCanRefill()
    {
        Ray ray = new Ray(toolTransform.position, toolTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3f))
        {
            //Debug.Log("Hit: " + hit.collider.gameObject.name); // Log the name of the hit object

            isLookingAtSink = hit.collider.gameObject == sink;
            //Debug.Log("Looking at Sink: " + isLookingAtSink); // Log whether the player is looking at the sink

            if (isLookingAtSink && IsActionButtonHeld() && currentWaterLevel < maxWaterLevel)
            {
                //Debug.Log("Trying to refill...");
                //waterLevelSlider.gameObject.SetActive(true);
                currentWaterLevel += (waterUsageRate * 2) * Time.deltaTime; // Refill rate
                currentWaterLevel = Mathf.Min(currentWaterLevel, maxWaterLevel); // Clamp to max level
                waterLevelSlider.value = currentWaterLevel;

                if (!sinkParticleSystem.isPlaying)
                    sinkParticleSystem.Play(); // Start sink particle effect
    }
            else
            {
                if (sinkParticleSystem.isPlaying)
                    sinkParticleSystem.Stop(); // Stop sink particle effect
            }
        }
        else
        {
            //Debug.Log("No hit"); // Log if the raycast hits nothing
        }
    }

    private void HandleToolSwitch()
    {
        if (currentTool == ToolType.WateringCan && !waterLevelSlider.gameObject.activeSelf)
        {
            ShowWaterLevelSlider();
        }
        else if (currentTool != ToolType.WateringCan && waterLevelSlider.gameObject.activeSelf)
        {
            HideWaterLevelSlider();
        }
    }

    private void ShowWaterLevelSlider()
    {
        waterLevelSlider.gameObject.SetActive(true);
    }

    private void HideWaterLevelSlider()
    {
        waterLevelSlider.gameObject.SetActive(false);
    }


    void PerformWateringRaycasts() {
    // Similar to SprayWater() but checking for plants
    }

    void AdjustWaterFlowBasedOnToolAngle()
    {
        if (toolSwitcher != null && toolSwitcher.isSwitching)
        {   
            //Debug.Log("isSwitching is true.");
            return; // Skip if a tool switch is in progress
        }

        if (currentCooldownTimer > 0)
        {
            waterUsageRate = 0;
        }

        if (currentCooldownTimer <= 0)
        {
            waterUsageRate = 20;
        }

        float angle = Vector3.Angle(toolTransform.forward, Vector3.down);

        float invertedNormalizedAngle = 1 - Mathf.Clamp01(angle / 90f);

        var mainModule = waterParticleSystem.main;
        mainModule.startSpeed = Mathf.Lerp(minSpeed, maxSpeed, invertedNormalizedAngle);

        var emissionModule = waterParticleSystem.emission;
        emissionModule.rateOverTime = Mathf.Lerp(minEmissionRate, maxEmissionRate, invertedNormalizedAngle);

        if (angle <= 70 && !waterParticleSystem.isPlaying && currentWaterLevel > 0 && !isLookingAtSink && toolSwitcher.isSwitching == false && currentCooldownTimer <= 0)
        {
            //waterLevelSlider.gameObject.SetActive(true);
            waterParticleSystem.Play();
        }
        else if (angle > 70 && waterParticleSystem.isPlaying || currentWaterLevel <= 0)
        {
            //waterLevelSlider.gameObject.SetActive(false);
            waterParticleSystem.Stop();
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
