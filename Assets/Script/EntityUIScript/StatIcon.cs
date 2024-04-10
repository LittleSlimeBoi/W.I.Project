using UnityEngine;
using UnityEngine.UI;

public class StatIcon : MonoBehaviour
{
    public Image empty;
    public GameObject filled;

    public void Fill()
    {
        filled.SetActive(true);
    }

    public void Empty()
    {
        filled.SetActive(false);
    }
}
