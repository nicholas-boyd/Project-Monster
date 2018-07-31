using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class EquippedItemPanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public const string ItemPanelDragNotification = "ITEM_PANEL_DRAG";
    public EquipSlots equipSlots;
    public Panel panel;
    public Image icon;
    public Text nameLabel;
    public Equippable obj;

    public bool dragOnSurfaces = true;

    private GameObject equippableObj;
    private Transform parent;
    private Vector3 startPosition;
    private RectTransform dragPlane;

    void Start()
    {
        Display(null);
    }

    public void Display(Equippable obj)
    {
        if (obj == null)
        {
            icon.sprite = null;
            icon.color = Color.clear;
            nameLabel.text = "";
            this.obj = null;
        }
        else
        {
            icon.color = Color.white;
            icon.preserveAspect = true;
            icon.sprite = obj.GetComponent<SpriteRenderer>().sprite;
            nameLabel.text = obj.name;
            this.obj = obj;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.localPosition;
        parent = transform.parent;
        if (obj == null || nameLabel.text.Equals(""))
            return;

        var canvas = FindInParents<Canvas>(gameObject);
        if (canvas == null)
            return;

        // We have clicked something that can be dragged.
        // What we want to do is create an icon for this.
        equippableObj = this.gameObject;

        equippableObj.transform.SetParent(canvas.transform, false);
        equippableObj.transform.SetAsLastSibling();

        if (dragOnSurfaces)
            dragPlane = transform as RectTransform;
        else
            dragPlane = canvas.transform as RectTransform;

        SetDraggedPosition(eventData);
    }

    public void OnDrag(PointerEventData data)
    {
        if (obj == null || nameLabel.text.Equals(""))
            return;
        if (equippableObj != null)
            SetDraggedPosition(data);
    }

    private void SetDraggedPosition(PointerEventData data)
    {
        if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
            dragPlane = data.pointerEnter.transform as RectTransform;

        var rt = equippableObj.GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(dragPlane, data.position, data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            rt.rotation = dragPlane.rotation;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.PostNotification(ItemPanelDragNotification, this.gameObject);
        transform.localPosition = startPosition;
        transform.parent = parent;
    }

    static public T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();

        if (comp != null)
            return comp;

        Transform t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }
}
