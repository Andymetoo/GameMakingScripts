using System;
using System.Collections;
using UnityEngine;

public class ToolSwitcher : MonoBehaviour
{
    public static int ActiveToolIndex { get; private set; } = -1; // -1 means no tool is active
    public GameObject[] tools;
    [SerializeField] private float timeToMove = 0.5f;
    private GameObject currentTool;
    public bool isSwitching = false;

    void Start()
    {
        foreach (GameObject tool in tools)
        {
            tool.transform.localPosition = new Vector3(0.19f, -1.00f, 0.84f);
            tool.SetActive(false);
        }
    }

    void Update()
    {
        if (isSwitching) return;

        for (int i = 0; i < tools.Length; i++)
        {
            KeyCode keyToCheck = KeyCode.Alpha1 + i;
            if (i == 9) keyToCheck = KeyCode.Alpha0;

            if (Input.GetKeyDown(keyToCheck))
            {
                StartCoroutine(SwitchTool(tools[i]));
            }
        }
    }

    IEnumerator SwitchTool(GameObject newTool)
    {
        isSwitching = true;

        if (currentTool != null)
        {
            yield return MoveTool(currentTool, new Vector3(0.19f, -1.00f, 0.84f));
            currentTool.SetActive(false);
        }

        newTool.SetActive(true);
        yield return MoveTool(newTool, new Vector3(0.19f, 0.54f, 0.84f));
        currentTool = newTool;
        ActiveToolIndex = Array.IndexOf(tools, newTool);

        isSwitching = false;
    }

    IEnumerator MoveTool(GameObject tool, Vector3 targetPosition)
    {
        float elapsedTime = 0;
        Vector3 startPosition = tool.transform.localPosition;

        while (elapsedTime < timeToMove)
        {
            tool.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        tool.transform.localPosition = targetPosition;
    }
}
