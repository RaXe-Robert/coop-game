using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private Text damageText;
    [SerializeField] private Text movementSpeedText;
    [SerializeField] private Text timeBetweenAttacks;
    [SerializeField] private Text defenseText;

    private PlayerStatsComponent stats;

    private void Start()
    {
        stats = FindObjectOfType<PlayerStatsComponent>();
        stats.OnValueChangedCallback += UpdateUI;

        UpdateUI();
    }

    private void UpdateUI()
    {
        damageText.text = $"Damage: {stats.MinDamage} - {stats.MaxDamage}";
        movementSpeedText.text = $"MovementSpeed: {stats.MovementSpeed}";
        timeBetweenAttacks.text = $"Time between attacks: {stats.TimeBetweenAttacks}";
        defenseText.text = $"Defense: {stats.Defense}";
    }
}
