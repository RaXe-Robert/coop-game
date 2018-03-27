using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private Text damageText;
    [SerializeField] private Text movementSpeedText;
    [SerializeField] private Text attacksPerSecond;
    [SerializeField] private Text defenseText;

    private PlayerStats stats;

    private void Start()
    {
        stats = FindObjectOfType<PlayerStats>();
        stats.OnValueChangedCallback += UpdateUI;

        UpdateUI();
    }

    private void UpdateUI()
    {
        damageText.text = $"Damage: {stats.MinDamage} - {stats.MaxDamage}";
        movementSpeedText.text = $"MovementSpeed: {stats.MovementSpeed}";
        attacksPerSecond.text = $"Attacks per Second: {stats.AttacksPerSecond}";
        defenseText.text = $"Defense: {stats.Defense}";
    }
}
