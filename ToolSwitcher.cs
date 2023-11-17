using System.Collections;
using UnityEngine;

public class ToolSwitcher : MonoBehaviour
{
    public GameObject[] tools;
    private GameObject currentTool;
    private Vector3 toolUsePosition = new Vector3(0.19f, 0.54f, 0.84f); // Relative to the tool's current position
    private Vector3 toolHidePosition = new Vector3(0.19f, -1.00f, 0.84f); // Relative to the tool's current position

    void Start()
    {
        foreach (GameObject tool in tools)
        {
            tool.transform.localPosition = toolHidePosition;
            tool.SetActive(false);
        }
    }

    void Update()
    {
        for (int i = 0; i < tools.Length; i++)
        {
            KeyCode keyToCheck = KeyCode.Alpha1 + i;
            if (i == 9) keyToCheck = KeyCode.Alpha0; // Special case for 10th tool

            if (Input.GetKeyDown(keyToCheck))
            {
                StartCoroutine(SwitchTool(tools[i]));
            }
        }
    }

    IEnumerator SwitchTool(GameObject newTool)
    {
        if (currentTool != null)
        {
            yield return MoveTool(currentTool, toolHidePosition);
            currentTool.SetActive(false);
        }

        newTool.SetActive(true);
        yield return MoveTool(newTool, toolUsePosition);
        currentTool = newTool;
    }

    IEnumerator MoveTool(GameObject tool, Vector3 targetPosition)
    {
        float timeToMove = 0.5f; // Time to move the tool
        float elapsedTime = 0;
        Vector3 startPosition = tool.transform.localPosition;

        while (elapsedTime < timeToMove)
        {
            tool.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        tool.transform.localPosition = targetPosition;
    }
}
