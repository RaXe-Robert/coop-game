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

    private Vector3 position = new Vector3();
    private float defaultX;
    private float defaultY;
    private float distanceFromHorizontal;
    private float distanceFromVertical;
    private float preferredWidth;
    private float preferredHeight;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void LateUpdate()
    {
        if (panel.activeSelf)
            panel.transform.position = CalculatePosition();
    }

    private Vector3 CalculatePosition()
    {
        defaultX = Input.mousePosition.x;
        defaultY = Input.mousePosition.y + 50F;
        position.z = Input.mousePosition.z;

        distanceFromHorizontal = defaultX;
        distanceFromVertical = defaultY;

        preferredWidth = title.preferredWidth;
        preferredHeight = title.preferredHeight + description.preferredHeight + 50F;

        if (title.preferredWidth < description.preferredWidth)
            preferredWidth = description.preferredWidth;
        
        //Calculate distance from left or right border.
        if (defaultX < Screen.width / 2)
            distanceFromHorizontal = defaultX;
        else if (defaultX > Screen.width / 2)
            distanceFromHorizontal = Screen.width - defaultX;
        
        //Calculate the distance from upper or bottom border.
        if (defaultY < Screen.height / 2)
            distanceFromVertical = defaultY;
        else if (defaultY > Screen.height / 2)
            distanceFromVertical = Screen.height - defaultY;        

        if (distanceFromHorizontal < preferredWidth)
        {
            if (defaultX < Screen.width / 2)
                position.x = preferredWidth + distanceFromHorizontal;
            else if (defaultX > Screen.width / 2)
                position.x = Screen.width - preferredWidth - distanceFromHorizontal;
        }
        else
            position.x = defaultX;

        if (distanceFromVertical < preferredHeight)
        {
            if (defaultY < Screen.height / 2) //Eventually replace defaultY with (preferredHeight + distanceFromVertical) to let the bottom border have extra offset.
                position.y = defaultY;
            else if (defaultY > Screen.height / 2)
                position.y = Screen.height - preferredHeight - distanceFromVertical;
        }
        else
            position.y = defaultY;
        
        return position;      
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
