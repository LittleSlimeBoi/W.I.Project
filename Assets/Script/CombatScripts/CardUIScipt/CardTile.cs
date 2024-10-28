using UnityEngine;
using UnityEngine.UI;

public class CardTile : MonoBehaviour
{
    public string tileName;
    [SerializeField] private Image tileImage;
    [SerializeField] private Color blank, atk, move, both, bind;

    public void colorTile(int tileType)
    {
        switch (tileType)
        {
            case 1:
                tileImage.color = atk;
                break;
            case 2:
                tileImage.color = move;
                break;
            case 3:
                tileImage.color = both;
                break;
            case 4:
                tileImage.color = bind;
                break;
            default:
                tileImage.color = blank;
                break;
        }
    }
}
