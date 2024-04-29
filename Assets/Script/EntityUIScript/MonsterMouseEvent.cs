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

        foreach(GridTile tile in gameObject.GetComponent<Monster>().atkArea)
        {
            tile.ActivateSelectHighlight(true);
        }

        foreach (GridTile tile in gameObject.GetComponent<Monster>().moveArea)
        {
            tile.ActivateSelectHighlight(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<Monster>().hpBar.gameObject.SetActive(false);

        foreach (GridTile tile in gameObject.GetComponent<Monster>().atkArea)
        {
            tile.ActivateSelectHighlight(false);
        }

        foreach (GridTile tile in gameObject.GetComponent<Monster>().moveArea)
        {
            tile.ActivateSelectHighlight(false);
        }
    }
}
