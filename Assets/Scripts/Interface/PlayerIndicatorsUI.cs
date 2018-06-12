using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerIndicatorsUI : MonoBehaviour
{
    [SerializeField]
    private GameObject indicatorTemplate;

    private Transform localPlayerTransform;
    private PlayerCameraController playerCamera;

    [SerializeField]
    private float distanceFromPivot = 30f;

    [SerializeField]
    private float indicatorDrawTreshold = 5f;

    private Vector3 startPos => transform.position + Vector3.up * distanceFromPivot;
    private Quaternion startRot => Quaternion.identity;

    private Dictionary<int, PlayerIndicator> playerIndicators;

    private void Awake()
    {
        playerIndicators = new Dictionary<int, PlayerIndicator>();
    }
    
    private void Start()
    {
        localPlayerTransform = PlayerNetwork.LocalPlayer.transform;
        playerCamera = localPlayerTransform.GetComponent<PlayerCameraController>();

        PlayerNetwork.OnOtherPlayerCreated += OnOtherPlayerCreated;

        StartCoroutine(UpdateIndicators());
    }

    private void OnOtherPlayerCreated(PhotonView photonView)
    {
        if (!playerIndicators.ContainsKey(photonView.viewID))
            playerIndicators.Add(photonView.viewID, new PlayerIndicator(photonView.transform, Instantiate(indicatorTemplate, transform).transform));
    }

    private IEnumerator UpdateIndicators()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        while (true)
        {
            yield return waitForEndOfFrame;

            if (InputManager.GetButton("Show Player Indicators"))
                DrawIndicators();
            else
                HideIndicators();
        }
    }
    
    private void DrawIndicators()
    {
        Vector3 cameraForward = playerCamera.CameraReference.transform.forward;
        cameraForward.y = 0;
        
        Vector3 localPlayerPos = localPlayerTransform.position;
        localPlayerPos.y = 0;

        foreach (var kvp in playerIndicators)
        {
            if (kvp.Value.PlayerTransform == null || Vector3.Distance(localPlayerPos, kvp.Value.PlayerTransform.position) <= indicatorDrawTreshold)
            {
                kvp.Value.Indicator.gameObject.SetActive(false);
                continue;
            }

            kvp.Value.Indicator.gameObject.SetActive(true);

            Vector3 playerToPointToPos = kvp.Value.PlayerTransform.position;
            playerToPointToPos.y = 0;
            
            Vector3 targetDir = playerToPointToPos - localPlayerPos;
            float angle = Vector3.SignedAngle(targetDir, cameraForward, Vector3.up);

            kvp.Value.Indicator.transform.position = startPos;
            kvp.Value.Indicator.transform.rotation = startRot;
            kvp.Value.Indicator.transform.RotateAround(transform.position, Vector3.forward, angle);
        }
    }

    private void HideIndicators()
    {
        foreach (var kvp in playerIndicators)
        {
            kvp.Value.Indicator.gameObject.SetActive(false);
            continue;
        }
    }

    public class PlayerIndicator
    {
        public readonly Transform PlayerTransform;
        public readonly Transform Indicator;

        public PlayerIndicator(Transform playerTransform, Transform indicator)
        {
            PlayerTransform = playerTransform;
            Indicator = indicator;
        }
    }
}
