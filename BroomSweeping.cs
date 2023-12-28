using UnityEngine;

public class BroomSweeping : MonoBehaviour
{
    [SerializeField] private Transform broomTransform;
    [SerializeField] private ParticleSystem sweepParticles; // Particle system for sweeping effect
    [SerializeField] private float sweepSpeed = 2.0f;
    [SerializeField] private float sweepAmount = 0.05f;
    [SerializeField] private Vector3 sweepDirection = new Vector3(0, 0, 1);
    [SerializeField] private LayerMask dirtLayer; // Layer for the dirt smudges
    [SerializeField] private Vector3 particleOffset = new Vector3(0, 0, 0); // Adjust these values in the inspector
    

    private bool isSweeping = false;
    private Vector3 originalLocalPosition;

    void Start()
    {
        originalLocalPosition = broomTransform != null ? broomTransform.localPosition : Vector3.zero;
        if (sweepParticles != null)
        {
            sweepParticles.Stop(); // Ensure the particle system is stopped at the start
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        DirtSmudge dirtSmudge = other.GetComponent<DirtSmudge>();
        if (dirtSmudge != null && isSweeping)
        {
            dirtSmudge.StartSweeping();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DirtSmudge dirtSmudge = other.GetComponent<DirtSmudge>();
        if (dirtSmudge != null)
        {
            dirtSmudge.StopSweeping();
        }
    }

    void Update()
    {
        // Check if the action key is pressed and the current tool is the broom
        if (IsActionKeyPressed() && ToolSwitcher.ActiveToolIndex != 1 && ToolSwitcher.ActiveToolIndex != 3 && ToolSwitcher.ActiveToolIndex != 4)
        {
            isSweeping = true;
            // Calculate position for the particle system relative to the broom's transform
            if (sweepParticles != null)
            {
                Vector3 relativePosition = broomTransform.TransformDirection(particleOffset);
                sweepParticles.transform.position = broomTransform.position + relativePosition;
                if (!sweepParticles.isPlaying)
                {
                    sweepParticles.Play(); // Start the particle effect
                }
            }
        }

        // Check if the action key is released
        if (IsActionKeyReleased())
        {
            isSweeping = false;
            broomTransform.localPosition = originalLocalPosition; // Reset position
            if (sweepParticles != null)
            {
                sweepParticles.Stop(); // Stop the particle effect
            }
        }

        // Handle the sweeping motion if the broom is sweeping
        if (isSweeping)
        {
            SweepMotion();
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

    void SweepMotion()
    {
        float sweepFactor = Mathf.Sin(Time.time * sweepSpeed) * sweepAmount;
        broomTransform.localPosition = originalLocalPosition + sweepFactor * sweepDirection.normalized;
    }

    //commented out because this is the old raycast method.
    /*void CheckForDirt()
    {
        // Global Offset: This offset remains constant in world space
        Vector3 globalOffset = new Vector3(0, 0.28f, -0.3f); // Adjust as needed
        Vector3 rayOrigin = broomTransform.position + globalOffset;

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, broomTransform.forward, out hit, 2f, dirtLayer))
        {
            DirtSmudge dirtSmudge = hit.collider.GetComponent<DirtSmudge>();
            if (dirtSmudge != null)
            {
                dirtSmudge.StartSweeping();
            }
        }

        // Debug raycast
        Debug.DrawRay(rayOrigin, broomTransform.forward * 2f, Color.green, 1.0f);
    }*/
}
