using UnityEngine;

public class SprayBottle : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow, 1f);
            if (Physics.Raycast(ray, out hit))
            {
                WindowSmudge smudge = hit.collider.GetComponent<WindowSmudge>();
                if (smudge != null)
                {
                    smudge.MakeWet();
                    Debug.Log("Made a smudge wet");
                }
            }
        }
    }
}