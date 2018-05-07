using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class BuildableInteractionMenu : MonoBehaviour
{
    private static BuildableInteractionMenu instance = null;
    public static BuildableInteractionMenu Instance
    {
        get
        {
            if (instance == null)
            {
                var resource = Resources.Load("BuildableInteractionMenu");
                if (resource)
                {
                    GameObject go = Instantiate(resource) as GameObject;
                    instance = go.GetComponent<BuildableInteractionMenu>();
                }
                else Debug.Log("Cant find BuildableInteractionMenu in Resources");
            }
            return instance;
        }
    }

    private Canvas canvas = null;

    [SerializeField] private List<Button> interactionButtons = new List<Button>();

    public BuildableWorldObject Target { get; private set; }
    public int TargetInstanceID => Target?.GetInstanceID() ?? 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        canvas = GetComponent<Canvas>();
    }

    private void Update()
    {
        if (Target == null)
        {
            Hide();
            return;
        }

        transform.position = Target.transform.position + Vector3.up;
    }

    public void Show(BuildableWorldObject invoker, UnityAction[] actions)
    {
        if (actions == null) return;
        Hide();

        Target = invoker;

        if (canvas.worldCamera == null)
            canvas.worldCamera = PlayerNetwork.PlayerObject?.GetComponent<PlayerCameraController>()?.CameraReference;

        if (actions.Length > interactionButtons.Count)
            Debug.LogWarning("[BuildableInteractionMenu] More actions than interaction buttons.");

        for (int i = 0; i < interactionButtons.Count; i++)
        {
            if (i >= actions.Length)
                break;

            interactionButtons[i].gameObject.SetActive(true);
            interactionButtons[i].onClick.AddListener(actions[i]);
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        Target = null;

        foreach (Button button in interactionButtons)
        {
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }
}
