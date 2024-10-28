using UnityEngine;

public class StatBar : MonoBehaviour
{
    public CharacterCombatManager character;
    [SerializeField] private StatType statName;
    [SerializeField] private StatIcon container;
    private StatIcon[] statBar;
    private int maxStat, currentStat;

    private void Awake()
    {
        maxStat = character.GetMaxStat(statName);
        statBar = new StatIcon[maxStat];

        for (int i = 0; i < character.GetMaxStat(statName); i++)
        {
            statBar[i] = Instantiate(container);
            statBar[i].transform.SetParent(this.transform);
            statBar[i].transform.localScale = new Vector3(1, 1);
        }
        UpdateStatBar();
    }

    public void UpdateStatBar()
    {
        if (gameObject.activeInHierarchy)
        {
            currentStat = character.GetCurrentStat(statName);
            // Fill container
            for (int i = 0; i < currentStat; i++)
            {
                statBar[i].Fill();
            }

            // Empty container
            for (int i = Mathf.Max(currentStat, 0); i < maxStat; i++)
            {
                statBar[i].Empty();
            }
        }
    }

    public enum StatType
    {
        HP,
        Mana
    }
}
