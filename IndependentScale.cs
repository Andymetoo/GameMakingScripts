using UnityEngine;

public class IndependentScale : MonoBehaviour
{
    private Transform parentTransform;
    private Vector3 initialScale;

    void Start()
    {
        // Store the initial scale
        initialScale = transform.localScale;

        // Store the reference to the parent transform
        parentTransform = transform.parent;
    }

    void Update()
    {
        if (parentTransform)
        {
            // Invert the parent's scale effect
            transform.localScale = new Vector3(
                initialScale.x / parentTransform.localScale.x,
                initialScale.y / parentTransform.localScale.y,
                initialScale.z / parentTransform.localScale.z
            );
        }
    }
}
