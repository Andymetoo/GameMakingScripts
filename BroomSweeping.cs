using UnityEngine;
public class BroomSweeping : MonoBehaviour
{
    [SerializeField] private Transform broomTransform;
    [SerializeField] private float sweepSpeed = 2.0f;
    [SerializeField] private float sweepAmount = 0.05f;
    [SerializeField] private Vector3 sweepDirection = new Vector3(0, 0, 1);
    [SerializeField] private LayerMask dirtLayer; // Layer for the dirt smudges

    private bool isSweeping = false;
    private Vector3 originalLocalPosition;

    void Start()
    {
        originalLocalPosition = broomTransform != null ? broomTransform.localPosition : Vector3.zero;
    }

    void Update()
    {
        if (IsActionKeyPressed() && ToolSwitcher.ActiveToolIndex != 1 && ToolSwitcher.ActiveToolIndex != 3) //These are the tools that will NOT bob when mouse is held down.
        {
            isSweeping = true;
        }
        if (IsActionKeyReleased())
        {
            isSweeping = false;
            broomTransform.localPosition = originalLocalPosition; // Reset position
        }

        if (isSweeping)
        {
            SweepMotion();
            CheckForDirt();
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

    void CheckForDirt()
    {
        RaycastHit hit;
        if (Physics.Raycast(broomTransform.position, broomTransform.forward, out hit, 1f, dirtLayer))
        {
            DirtSmudge dirtSmudge = hit.collider.GetComponent<DirtSmudge>();
            if (dirtSmudge != null)
            {
                dirtSmudge.StartSweeping();
            }
        }
    }
}