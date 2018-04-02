﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameInterface { Inventory, Crafting, Equipment, EscapeMenu }

public class GameInterfaceManager : MonoBehaviour
{

    public static GameInterfaceManager Instance { get; private set; }

    [SerializeField] private GameObject escapeMenuUI;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject craftingUI;
    [SerializeField] private GameObject equipmentUI;

    private Dictionary<GameInterface, GameObject> interfaceGameObjectDictionary;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        interfaceGameObjectDictionary = new Dictionary<GameInterface, GameObject>
        {
            {GameInterface.EscapeMenu, escapeMenuUI },
            {GameInterface.Crafting, craftingUI },
            {GameInterface.Equipment, equipmentUI },
            {GameInterface.Inventory, inventoryUI }
        };
    }

    public void ToggleGameInterface(GameInterface interfaceToToggle)
    {
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

    private void CloseAllInterfaces()
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
            {
                panel.Value.SetActive(false);
            }
            else
            {
                panel.Value.SetActive(!panel.Value.activeSelf);
            }
        }
    }
}
