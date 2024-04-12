using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CancelPanel : MonoBehaviour, IPointerDownHandler
{
    public Image panel;
    public Color darken;

    public static event Action OnCancel;

    private void Start()
    {
        DropZone.OnCardDrop += UpdateOnCardDrop;
    }

    private void OnDestroy()
    {
        DropZone.OnCardDrop -= UpdateOnCardDrop;
    }

    void UpdateOnCardDrop()
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
        OnCancel?.Invoke();
    }
}
