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
    [SerializeField] private GameObject outline;

    [SerializeField] private TextMeshProUGUI textDes;

    public CardMouseEvent mouseEvent;

    public static Player player;

    public int handIndex;

    private void Start()
    {
        PaintCardGrid();
        cost.text = info.cost.ToString();
        description.sprite = info.cardDes;

        player = transform.root.Find("Grid Area/Player Side/Player Icon").GetComponent<Player>();

        textDes.text = info.placeHolderDes;
    }

    public void AssignValueToTile(int posX, int posY)
    {
        if (info.playerBind)
        {
            for (int i = 0; i < 25; i++)
            {
                switch (info.range[i])
                {
                    case 1:
                        CalculateAssignment(i, player.otherSide, true, posX, posY);
                        break;
                    case 2:
                        CalculateAssignment(i, player.mySide, true, posX, posY);
                        break;
                    case 3:
                        CalculateAssignment(i, player.mySide, true, posX, posY);
                        CalculateAssignment(i, player.otherSide, true, posX, posY);
                        break;
                }
            }
        }
        else
        {
            for (int i = 0; i < 25; i++)
            {
                switch (info.range[i])
                {
                    case 1:
                        CalculateAssignment(i, player.otherSide, true, posX, posY);
                        break;
                    case 2:
                        CalculateAssignment(i, player.mySide, true, posX, posY);
                        break;
                    case 3:
                        CalculateAssignment(i, player.mySide, true, posX, posY);
                        CalculateAssignment(i, player.otherSide, true, posX, posY);
                        break;
                }
            }
        }

        // Update visual
        player.mySide.UpdateGridOnCardPlay(true);
        player.otherSide.UpdateGridOnCardPlay(true);
    }

    public void UnassignValueToTile(int posX, int posY)
    {
        if (info.playerBind)
        {
            for (int i = 0; i < 25; i++)
            {
                switch (info.range[i])
                {
                    case 1:
                        CalculateAssignment(i, player.otherSide, false, posX, posY);
                        break;
                    case 2:
                        CalculateAssignment(i, player.mySide, false, posX, posY);
                        break;
                    case 3:
                        CalculateAssignment(i, player.mySide, false, posX, posY);
                        CalculateAssignment(i, player.otherSide, false, posX, posY);
                        break;
                }
            }
        }
        else
        {
            for (int i = 0; i < 25; i++)
            {
                switch (info.range[i])
                {
                    case 1:
                        CalculateAssignment(i, player.otherSide, false, posX, posY);
                        break;
                    case 2:
                        CalculateAssignment(i, player.mySide, false, posX, posY);
                        break;
                    case 3:
                        CalculateAssignment(i, player.mySide, false, posX, posY);
                        CalculateAssignment(i, player.otherSide, false, posX, posY);
                        break;
                }
            }
        }

        // Update visual
        player.mySide.UpdateGridOnCardPlay(false);
        player.otherSide.UpdateGridOnCardPlay(false);
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
            map.grid[x, y].Selectable = assign;
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
        CardMouseEvent.canHover = true;
    }

    public void RemoveCard()
    {
        transform.SetParent(null);
        Destroy(gameObject);
    }
}
