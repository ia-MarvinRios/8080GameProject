using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct UINode
{
    internal GameObject previous;
    internal GameObject next;
}

public class GUIBrain : MonoBehaviour
{
    public static GUIBrain Instance { get; private set; }

    List<UINode> uiNodes;
    int currentIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        uiNodes = new List<UINode>();
    }

    private void OnDisable()
    {
        EndInteraction();
    }

    public void StartNewInteraction(GameObject uiMenu)
    {
        uiNodes.Clear();

        uiNodes.Add(new UINode { previous = null, next = uiMenu });
        currentIndex = 0;

        uiMenu.SetActive(true);
    }
    public void OpenNewMenu(GameObject uiMenu)
    {
        uiNodes[^1].next.SetActive(false);
        uiNodes.Add(new UINode { previous = uiNodes[^1].next, next = uiMenu });
        currentIndex++;
        uiMenu.SetActive(true);
    }
    public void CloseCurrentMenu()
    {
        uiNodes[^1].next.SetActive(false);
        uiNodes.RemoveAt(uiNodes.Count - 1);
        currentIndex--;
        if (uiNodes.Count > 0)
        {
            uiNodes[^1].next.SetActive(true);
        }
        else
        {
            Debug.Log("No more UI Menus to go back to.");
        }
    }
    public void GoBack()
    {
        if (uiNodes.Count < 2)
        {
            Debug.Log("No previous UI Menu to go back to.");
            return;
        }
        uiNodes[currentIndex + 1].previous.SetActive(false);
        uiNodes[currentIndex].previous.SetActive(true);
        currentIndex--;
    }
    public void GoForward()
    {
        if (uiNodes.Count < 2)
        {
            Debug.Log("No next UI Menu to go forward to.");
            return;
        }

        uiNodes[currentIndex + 1].previous.SetActive(false);
        uiNodes[currentIndex].next.SetActive(true);
        currentIndex++;
    }

    public void EndInteraction()
    {
        for (int i = uiNodes.Count - 1; i >= 0; i--)
        {
            uiNodes[i].next.SetActive(false);
        }
        uiNodes.Clear();
    }
}
