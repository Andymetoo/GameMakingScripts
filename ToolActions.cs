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
    [SerializeField] private Slider plantWateringSlider;
    [SerializeField] private ParticleSystem broomSweepParticles; // Reference to the broom's particle system



    private GameObject heldItem = null; 
    private Animator handAnimator;
    private bool isClothActive = false; // Added this line
    private GameObject potentialPickupItem = null; // New field to store potential item for pickup
    private float currentWaterLevel;
    private bool isLookingAtSink = false;
    private float wateringCanCooldown = 1.2f; // Cooldown time in seconds
    private float currentCooldownTimer = 0f;
    public Room room; // Assign this in the inspector

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

        plantWateringSlider.gameObject.SetActive(false);

        Plant[] plants = FindObjectsOfType<Plant>();
        foreach (var plant in plants)
        {
            plant.OnWateringBarDeactivationNeeded += NotifyWateringBarDeactivation;
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


        if (currentTool == ToolType.WateringCan)
        {
            float angle = Vector3.Angle(toolTransform.forward, Vector3.down);
            if (angle <= 70 && !toolSwitcher.isSwitching && currentCooldownTimer <= 0 && currentWaterLevel > 0)
            {
                PerformWateringRaycasts();
            }
        }

        if (potentialPickupItem != null && !potentialPickupItem.activeInHierarchy)
        {
            potentialPickupItem = null;
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
                if (IsActionButtonHeld() && currentTool == ToolType.Broom)
        {
            // Activate the particle system when the broom is in use
            if (!broomSweepParticles.isPlaying)
            {
                broomSweepParticles.Play();
            }

            // Implement the logic for broom sweeping here
            // ...
        }
        else
        {
            // Stop the particle system when the broom is not in use
            if (broomSweepParticles.isPlaying)
            {
                broomSweepParticles.Stop();
            }
        }
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
        // Clear potentialPickupItem if it's no longer active
        if (potentialPickupItem != null && !potentialPickupItem.activeInHierarchy)
        {
            potentialPickupItem = null;
        }

        BedInteraction bed = potentialPickupItem?.GetComponent<BedInteraction>();
        FurnitureInteraction furniture = potentialPickupItem?.GetComponent<FurnitureInteraction>();

        // For beds and furniture, start interaction on press and continue if held
        if (IsActionKeyPressed())
        {
            if (bed != null)
            {
                bed.StartMaking();
            }
            else if (furniture != null)
            {
                furniture.FixFurniture();
            }
        }
        else if (IsActionKeyReleased())
        {
            if (bed != null)
            {
                bed.StopMaking();
            }
            else if (furniture != null)
            {
                furniture.StopFixing();
            }
        }

        // For trash, pick up on press and drop on release
        if (IsActionKeyPressed() && heldItem == null && potentialPickupItem != null && potentialPickupItem.CompareTag("Trash"))
        {
            PickUpItem(potentialPickupItem);
        }
        else if (IsActionKeyReleased() && heldItem != null)
        {
            DropTrash();
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
        // Check if the item is valid and not destroyed
        if (item == null || !item.activeInHierarchy)
        {
            return; // Exit the method if the item is invalid or destroyed
        }

        heldItem = item;
        heldItem.transform.SetParent(toolTransform);
        heldItem.transform.localPosition = new Vector3(0, 0, 10);

        // Disable physics and collider of the held item
        Rigidbody rb = heldItem.GetComponent<Rigidbody>();
        Collider col = heldItem.GetComponent<Collider>();
        if (rb != null) rb.isKinematic = true;
        if (col != null) col.enabled = false;

        // Subscribe to the disposal event of the trash item (if it's a trash item)
        TrashItem trashItem = item.GetComponent<TrashItem>();
        if (trashItem != null)
        {
            trashItem.IsHeld = true;
            trashItem.OnDisposal += () => {
                if (potentialPickupItem == item) {
                    potentialPickupItem = null;
                }
            };
        }
    }

    void HandleTrashDisposal()
    {
    heldItem = null;
    potentialPickupItem = null;
    }


    void DropTrash()
    {
        if (heldItem != null && heldItem.activeInHierarchy)
        {
            Rigidbody rb = heldItem.GetComponent<Rigidbody>();
            Collider col = heldItem.GetComponent<Collider>();

            if (rb != null) rb.isKinematic = false;
            if (col != null) col.enabled = true;

            TrashItem trashItem = heldItem.GetComponent<TrashItem>();
            if (trashItem != null)
            {
                trashItem.IsHeld = false;
                trashItem.Drop();
                trashItem.OnDisposal -= HandleTrashDisposal; // Unsubscribe from the event
            }

            heldItem.transform.SetParent(null);
            heldItem = null;
        }
        potentialPickupItem = null;
    }

   public void ForceDropItem()
    {
        if (heldItem != null)
        {
            // Ensure any cleanup or state reset related to dropping the item is done here
            TrashItem trashItem = heldItem.GetComponent<TrashItem>();
            if (trashItem != null)
            {
                trashItem.IsHeld = false;
                trashItem.Drop();
                trashItem.OnDisposal -= HandleTrashDisposal;
            }

            Rigidbody rb = heldItem.GetComponent<Rigidbody>();
            Collider col = heldItem.GetComponent<Collider>();
            if (rb != null) rb.isKinematic = false;
            if (col != null) col.enabled = true;

            heldItem.transform.SetParent(null);
            heldItem = null;
        }
        potentialPickupItem = null;
    }
    

    private void UpdateWaterLevel()
    {
         if (currentTool == ToolType.WateringCan && toolSwitcher != null && !toolSwitcher.isSwitching)
        {
            if (waterParticleSystem.isPlaying && currentCooldownTimer <= 0)
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


    void PerformWateringRaycasts()
    {
        if (currentTool != ToolType.WateringCan) return;

        Vector3 rayOriginOffset = new Vector3(0, 0, -0.8f); // Adjust this offset as needed
        Vector3 rayOrigin = toolTransform.position + toolTransform.TransformDirection(rayOriginOffset);
        Vector3 rayDirection = toolTransform.forward;
        float rayLength = 3f; // Adjust the ray length as needed

        // Draw the ray for debugging
        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.green, 3f);
        Debug.Log("Watering can ray is casting.");

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength)) {
                Plant plant = hit.collider.GetComponent<Plant>();
                if (plant != null) {
                    plant.WaterPlant(Time.deltaTime); // Water the plant incrementally
                    UpdatePlantWateringSlider(plant); // Ensure this is called correctly
                }
            }
        }

            public void NotifyWateringBarDeactivation()
        {
            // This method is called from the Plant script when the timer reaches zero
            plantWateringSlider.gameObject.SetActive(false);
        }

    private void UpdateRoomCompletion(Plant plant) {
    Room room = plant.GetComponentInParent<Room>(); // Find the room the plant belongs to
    if (room != null) {
        room.UpdateCompletionUI(); // Update room completion
    }
}
    void UpdatePlantWateringSlider(Plant plant) {
        if (plantWateringSlider != null) {
            plantWateringSlider.gameObject.SetActive(true);
            plantWateringSlider.value = plant.GetWaterLevelPercentage();
        }
    }

    void OnDestroy()
    {
        Plant[] plants = FindObjectsOfType<Plant>();
        foreach (var plant in plants)
        {
            plant.OnWateringBarDeactivationNeeded -= NotifyWateringBarDeactivation;
        }
    }

    void AdjustWaterFlowBasedOnToolAngle()
    {
        // Ensure that the watering can doesn't act while switching tools.
        if (toolSwitcher != null && toolSwitcher.isSwitching)
        {
            if (waterParticleSystem.isPlaying)
            {
                waterParticleSystem.Stop();
            }
            return;
        }

        // If the cooldown timer is still running, don't start the water particle system.
        if (currentCooldownTimer > 0)
        {
            currentCooldownTimer -= Time.deltaTime;
            if (waterParticleSystem.isPlaying)
            {
                waterParticleSystem.Stop();
            }
            return;
        }

        float angle = Vector3.Angle(toolTransform.forward, Vector3.down);
        float invertedNormalizedAngle = 1 - Mathf.Clamp01(angle / 90f);

        var mainModule = waterParticleSystem.main;
        mainModule.startSpeed = Mathf.Lerp(minSpeed, maxSpeed, invertedNormalizedAngle);

        var emissionModule = waterParticleSystem.emission;
        emissionModule.rateOverTime = Mathf.Lerp(minEmissionRate, maxEmissionRate, invertedNormalizedAngle);

        // Control the water particle system based on the angle and water level.
        if (angle <= 70 && !waterParticleSystem.isPlaying && currentWaterLevel > 0 && !isLookingAtSink && currentCooldownTimer <= 0)
        {
            waterParticleSystem.Play();
        }
        else if (angle > 70 && waterParticleSystem.isPlaying || currentWaterLevel <= 0)
        {
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
