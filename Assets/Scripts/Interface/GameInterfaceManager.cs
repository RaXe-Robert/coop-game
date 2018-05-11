﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameInterface { Inventory, Crafting, Equipment, EscapeMenu, Controls, DeathScreen }

public class GameInterfaceManager : MonoBehaviour
{
    public static GameInterfaceManager Instance { get; private set; }

    [SerializeField] private GameObject escapeMenuUI;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject craftingUI;
    [SerializeField] private GameObject equipmentUI;
    [SerializeField] private GameObject controlsUI;
    [SerializeField] private GameObject deathScreen;

    private Dictionary<GameInterface, GameObject> interfaceGameObjectDictionary;
    private PlayerCombatController playerCombatController;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        playerCombatController = PlayerNetwork.PlayerObject.GetComponent<PlayerCombatController>();
        interfaceGameObjectDictionary = new Dictionary<GameInterface, GameObject>
        {
            {GameInterface.EscapeMenu, escapeMenuUI },
            {GameInterface.Crafting, craftingUI },
            {GameInterface.Equipment, equipmentUI },
            {GameInterface.Inventory, inventoryUI },
            {GameInterface.Controls, controlsUI },
            {GameInterface.DeathScreen, deathScreen }
        };
    }

    public void ToggleGameInterface(GameInterface interfaceToToggle)
    {
        if (playerCombatController.IsDead) return;

        switch (interfaceToToggle)
        {
            case GameInterface.Crafting:
                ToggleGivenDisableOthers(GameInterface.Crafting);
                break;
            case GameInterface.Equipment:
                ToggleGivenDisableOthers(GameInterface.Equipment);
                break;
            case GameInterface.Inventory:
                ToggleGivenDisableOthers(GameInterface.Inventory);
                break;
            case GameInterface.EscapeMenu:
                if (IsAnyInterfaceOpen())
                    CloseAllInterfaces();
                else
                    ToggleGivenDisableOthers(GameInterface.EscapeMenu);
                break;
            case GameInterface.DeathScreen:
                ToggleGivenDisableOthers(GameInterface.DeathScreen);
                break;
        }

        Tooltip.Instance.Hide();
    }

    public bool IsInterfaceOpen(GameInterface gameInterface)
    {
        return interfaceGameObjectDictionary[gameInterface].activeSelf;
    }

    public bool IsAnyInterfaceOpen()
    {
        foreach (var panel in interfaceGameObjectDictionary)
        {
            if (panel.Value.activeSelf)
                return true;
        }
        return false;
    }

    public void CloseAllInterfaces()
    {
        foreach (var panel in interfaceGameObjectDictionary)
        {
            if (panel.Value.activeSelf)
                panel.Value.SetActive(false);
        }
    }

    private void ToggleGivenDisableOthers(GameInterface gameInterface)
    {
        foreach (var panel in interfaceGameObjectDictionary)
        {
            if (panel.Key != gameInterface)
                panel.Value.SetActive(false);
            else
                panel.Value.SetActive(!panel.Value.activeSelf);
        }
    }
}
