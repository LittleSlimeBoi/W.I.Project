using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CancelPanel : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private DropZone dropZone;
    public Image panel;
    public Color darken;

    void Update()
    {
        if (CardMouseEvent.isDropped)
        {
            panel.color = darken;
            panel.raycastTarget = true;
        }
        else
        {
            panel.color = Color.clear;
            panel.raycastTarget = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dropZone.Cancel();
    }
}
