using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameInterface { Inventory, Crafting, Equipment }

public class GameInterfaceManager : MonoBehaviour {

    public static GameInterfaceManager Instance { get; private set; }

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
                craftingUI.SetActive(!craftingUI.activeSelf);
                break;
            case GameInterface.Equipment:
                equipmentUI.SetActive(!equipmentUI.activeSelf);
                break;
            case GameInterface.Inventory:
                inventoryUI.SetActive(!inventoryUI.activeSelf);
                break;
        }

        Tooltip.Instance.Hide();
    }

    private bool IsInterfaceOpen(GameInterface gameInterface)
    {
        return interfaceGameObjectDictionary[gameInterface].activeSelf;
    }
}
