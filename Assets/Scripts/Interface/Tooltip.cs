using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private Text text;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    /// <summary>
    /// Shows the tooltip with the given text
    /// </summary>
    public void Show(string text)
    {
        panel.SetActive(true);
        this.text.text = text;
    }

    /// <summary>
    /// Hides the tooltip
    /// </summary>
    public void Hide()
    {
        text.text = "";
        panel.SetActive(false);
    }

    void Update()
    {
        if(panel.activeSelf)
            panel.transform.position = Input.mousePosition + new Vector3(0, 50, 0);
    }
}
