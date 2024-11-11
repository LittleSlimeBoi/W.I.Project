using UnityEngine;

[CreateAssetMenu(fileName = "New Card Info", menuName = "Card")]
public class CardInfo: ScriptableObject
{
    public int cardID;
    public int cost;
    public int[] range = new int[25];
    public bool playerBind;
    public int pos;
    public bool isAOE;
    public int atkPower;
    public Sprite cardDes;
    public string placeHolderDes;
}
