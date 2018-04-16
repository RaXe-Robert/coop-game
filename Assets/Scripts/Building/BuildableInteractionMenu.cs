using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BuildableInteractionMenu : MonoBehaviour
{
    private static BuildableInteractionMenu instance = null;
    public static BuildableInteractionMenu Instance {
        get
        {
            if (instance == null)
            {
                UnityEngine.Object resource = Resources.Load("BuildableInteractionMenu");
                if (resource)
                {
                    GameObject go = Instantiate(resource) as GameObject;
                    instance = go.GetComponent<BuildableInteractionMenu>();
                }
            }
            return instance;
        }
    }

    private Canvas canvas = null;
    [SerializeField] private Button button_UseAction = null;
    [SerializeField] private Button button_PickupAction = null;

    private BuildableWorldObject target;
    public int TargetInstanceID => target?.GetInstanceID() ?? 0;

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
        if (target == null)
            return;

        transform.position = target.transform.position;
    }

    public void Show(BuildableWorldObject invoker, UnityAction[] actions)
    {
        Hide();

        target = invoker;

        if (canvas.worldCamera == null)
            canvas.worldCamera = PlayerNetwork.PlayerObject?.GetComponent<PlayerCameraController>()?.CameraReference;

        button_UseAction.onClick.RemoveAllListeners();
        button_UseAction.onClick.AddListener(actions[0]);
        button_PickupAction.onClick.RemoveAllListeners();
        button_PickupAction.onClick.AddListener(actions[1]);
        
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        target = null;

        gameObject.SetActive(false);
    }

}
