using UnityEngine;
using UnityEngine.EventSystems;

public class MonsterMouseEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.GetComponent<MonsterCombatManager>().hpBar.gameObject.SetActive(true);
        gameObject.GetComponent<MonsterCombatManager>().hpBar.UpdateStatBar();

        foreach(GridTile tile in gameObject.GetComponent<MonsterCombatManager>().atkArea)
        {
            tile.ActivateSelectHighlight(true);
        }

        foreach (GridTile tile in gameObject.GetComponent<MonsterCombatManager>().moveArea)
        {
            tile.ActivateSelectHighlight(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<MonsterCombatManager>().hpBar.gameObject.SetActive(false);

        foreach (GridTile tile in gameObject.GetComponent<MonsterCombatManager>().atkArea)
        {
            tile.ActivateSelectHighlight(false);
        }

        foreach (GridTile tile in gameObject.GetComponent<MonsterCombatManager>().moveArea)
        {
            tile.ActivateSelectHighlight(false);
        }
    }
}
