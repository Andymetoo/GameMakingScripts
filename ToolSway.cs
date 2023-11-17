using UnityEngine;

public class ToolSway : MonoBehaviour
{
    public float amount = 0.02f;
    public float maxAmount = 0.03f;
    public float smoothAmount = 6f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        float movementX = -Input.GetAxis("Horizontal") * amount;
        float movementY = -Input.GetAxis("Vertical") * amount;
        movementX = Mathf.Clamp(movementX, -maxAmount, maxAmount);
        movementY = Mathf.Clamp(movementY, -maxAmount, maxAmount);

        Vector3 finalPosition = new Vector3(initialPosition.x + movementX, initialPosition.y + movementY, initialPosition.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition, Time.deltaTime * smoothAmount);

        Quaternion finalRotation = new Quaternion(initialRotation.x + movementY, initialRotation.y + movementX, initialRotation.z, initialRotation.w);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, finalRotation, Time.deltaTime * smoothAmount);
    }
}
