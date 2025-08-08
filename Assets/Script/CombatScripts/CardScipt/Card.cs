using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardInfo info;
    [SerializeField] private CardTile tile;
    [SerializeField] private CardTile[] cardGrid = new CardTile[25];
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private Image description;
    [SerializeField] private TextMeshProUGUI textDes;

    public CardMouseEvent mouseEvent;

    public static PlayerCombatManager player;

    public int handIndex;

    public void AssignValueToTile(bool assign, int posX, int posY)
    {
        for (int i = 0; i < 25; i++)
        {
            switch (info.range[i])
            {
                case 1:
                    CalculateAssignment(i, player.otherSide, assign, posX, posY);
                    break;
                case 2:
                    CalculateAssignment(i, player.mySide, assign, posX, posY);
                    break;
                case 3:
                    CalculateAssignment(i, player.mySide, assign, posX, posY);
                    CalculateAssignment(i, player.otherSide, assign, posX, posY);
                    break;
            }
        }

        // Update visual
        player.mySide.UpdateGridOnCardPlay(assign);
        player.otherSide.UpdateGridOnCardPlay(assign);
    }

    public int TranslateCardToGridX(int index, GridMap map, bool playerBind, int x) // x = player.GetPosX()
    {
        if (playerBind)
        {
            return (index % 5) - (info.pos % 5) + x + (-player.mySide.anchorPosX + map.anchorPosX);
        }
        else
        {
            return (index % 5) - 2 + player.mySide.anchorPosX + (-player.mySide.anchorPosX + map.anchorPosX);
        }
    }
    public int TranslateCardToGridY(int index, GridMap map, bool playerBind, int y) // y = player.GetPosY()
    {
        if (playerBind)
        {
            return (index / 5) - (info.pos / 5) + y + (-player.mySide.anchorPosY + map.anchorPosY);
        }
        else
        {
            return (index / 5) - 2 + player.mySide.anchorPosY + (-player.mySide.anchorPosY + map.anchorPosY);
        }
    }
    public void CalculateAssignment(int index, GridMap map, bool assign, int posX, int posY)
    {
        int x = TranslateCardToGridX(index, map, info.playerBind, posX);
        int y = TranslateCardToGridY(index, map, info.playerBind, posY);
        if (map.IsValidPosition(x, y))
        {
            map.grid[x, y].IsSelectable = assign;
            map.selectableTileCount += assign ? 1 : -1;
        }
    }

    public void PaintCardGrid()
    {
        for(int j = 0; j < 5; j++)
        {
            for (int i = 0; i < 5; i++)
            {
                cardGrid[(5 * i) + j].colorTile(info.range[(5 * i) + j]);
                cardGrid[(5 * i) + j].name = $"Card Tile {i} {j}";
            }
        }
    }

    public void InitCard(CardInfo cardInfo, int index)
    {
        info = cardInfo;
        handIndex = index;
        PaintCardGrid();
        cost.text = info.cost.ToString();
        description.sprite = info.cardDes;
        textDes.text = info.placeHolderDes;
        CardMouseEvent.canHover = true;
    }

    public void RemoveCard()
    {
        transform.SetParent(null);
        Destroy(gameObject);
    }
}
