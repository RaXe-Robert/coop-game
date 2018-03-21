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

    //Makes sure that only one object at a time can control the tooltip
    private object focusLock = null;

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
    public void Show(object requestor, string title, string description = "")
    {
        if (focusLock != null && focusLock != requestor)
            return;

        if (RequestFocus(requestor) == false)
            return;

        panel.SetActive(true);
        this.title.text = title;
        this.description.text = description;

        if (description == string.Empty)
            this.description.gameObject.SetActive(false);
    }

    /// <summary>
    /// Hides the tooltip and removes the focus lock
    /// </summary>
    public void Hide(object requestor)
    {
        if (focusLock != null && focusLock != requestor)
            return;

        focusLock = null;

        title.text = "";
        panel.SetActive(false);
    }


    /// <summary>
    /// Request the focus of this tooltip instance. And makes sure no other objects can access the tooltip at the same time.
    /// </summary>
    /// <param name="focusObject">The object that wants to take control.</param>
    /// <returns>True if focus was succesfully granted.</returns>
    private bool RequestFocus(object focusObject)
    {
        if (focusLock == null)
        {
            focusLock = focusObject;
            return true;
        }

        return false;
    }
}
