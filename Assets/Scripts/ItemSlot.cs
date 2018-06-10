using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] protected Image image;
    [SerializeField] protected Text stackSizeText;
    [SerializeField] protected Image textBackground;

    protected CanvasGroup canvasGroup;
    protected Transform initialParentTransform;

    protected Item currentItem;
    public virtual Item CurrentItem
    {
        get
        {
            return currentItem;
        }
        set
        {
            currentItem = value;
            UpdateUI();
        }
    }

    protected virtual void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Clear()
    {
        currentItem = null;
        image.sprite = null;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        //We only want draggin on left mousebutton
        if (eventData.button != PointerEventData.InputButton.Left || currentItem == null)
            return;

        initialParentTransform = transform.parent;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        transform.SetParent(transform.parent.parent.parent);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        //We only want draggin on left mousebutton
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (currentItem == null)
            return;

        transform.position = eventData.position;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        //We only want draggin on left mousebutton
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        transform.SetParent(initialParentTransform);
        transform.localPosition = Vector3.zero;
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem == null)
            return;

        Tooltip.Instance.Show(currentItem.Name, currentItem.Description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance.Hide();
    }

    protected void UpdateUI()
    {
        image.sprite = currentItem?.Sprite;

        if (currentItem == null)
            image.color = new Color(255, 255, 255, 0);
        else image.color = new Color(255, 255, 255, 1);

        if (currentItem?.StackSize > 1)
        {
            stackSizeText.text = currentItem.StackSize.ToString();
            textBackground.enabled = true;
        }
        else
        {
            textBackground.enabled = false;
            stackSizeText.text = "";
        }
    }
}