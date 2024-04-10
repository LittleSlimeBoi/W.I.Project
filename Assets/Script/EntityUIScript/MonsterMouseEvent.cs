using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MonsterMouseEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.GetComponent<Monster>().hpBar.gameObject.SetActive(true);
        gameObject.GetComponent<Monster>().hpBar.UpdateStatBar();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<Monster>().hpBar.gameObject.SetActive(false);
    }
}
