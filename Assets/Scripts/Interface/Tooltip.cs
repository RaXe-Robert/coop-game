using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private Text title;
    [SerializeField] private Text description;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void LateUpdate()
    {
        if (panel.activeSelf)
            panel.transform.position = Input.mousePosition + new Vector3(0, 50, 0);
    }

    /// <summary>
    /// Shows the tooltip with the given text if the focus request is succesfull
    /// </summary>
    public void Show(string title, string description = "")
    {
        panel.SetActive(true);
        this.title.text = title;
        this.description.text = description;

        if (description == string.Empty)
            this.description.gameObject.SetActive(false);
    }

    /// <summary>
    /// Hides the tooltip and removes the focus lock
    /// </summary>
    public void Hide()
    {
        title.text = "";
        panel.SetActive(false);
    }
}
