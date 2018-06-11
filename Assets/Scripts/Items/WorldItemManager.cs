using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class WorldItemManager : Photon.MonoBehaviour {

    private static WorldItemManager instance;
    public static WorldItemManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("WorldItemManager");
                instance = go.AddComponent<WorldItemManager>();

                var photonId = PhotonNetwork.AllocateViewID();
                instance.photonView.viewID = photonId;
            }
            return instance;
        }
        private set { instance = value; }
    }

    [SerializeField]
    private int secondsToDespawnItem = 120;

    private Dictionary<int, CountdownHandler> itemWorldObjects;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
        {
            UnityEngine.Debug.LogWarning("Removed a second instance of WorldItemManager");
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        itemWorldObjects = new Dictionary<int, CountdownHandler>();
        StartCoroutine(DespawnItems());
    }

    public void AddItem(int photonId)
    {
        itemWorldObjects.Add(photonId, new CountdownHandler(secondsToDespawnItem));
    }

    public void RemoveItem(int photonId)
    {
        photonView.RPC(nameof(DestroyWorldItem), PhotonTargets.AllBuffered, photonId);
    }

    private IEnumerator DespawnItems()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1);

        List<int> finishedTimers = new List<int>();

        while (true)
        {
            if (itemWorldObjects.Count > 0)
            {
                finishedTimers.Clear();

                foreach (var kvp in itemWorldObjects)
                {
                    kvp.Value.Tick();
                    if (kvp.Value.IsFinished)
                        finishedTimers.Add(kvp.Key);
                }

                for (int i = finishedTimers.Count - 1; i >= 0; i--)
                {
                    if (itemWorldObjects.ContainsKey(finishedTimers[i]))
                    {
                        RemoveItem(finishedTimers[i]);
                    }
                }
            }

            yield return waitForSeconds;
        }
    }

    [PunRPC]
    private void DestroyWorldItem(int photonViewId)
    {
        PhotonView photonView = PhotonView.Find(photonViewId);
        if (photonView != null)
            Destroy(photonView.gameObject);

        if (itemWorldObjects.ContainsKey(photonViewId))
            itemWorldObjects.Remove(photonViewId);
    }

    public class CountdownHandler
    {
        public bool IsFinished { get; private set; }

        private readonly float totalMilliseconds;

        private Stopwatch stopwatch;
        
        /// <param name="countdownTime">In seconds.</param>
        public CountdownHandler(float countdownTime)
        {
            totalMilliseconds = countdownTime * 1000;
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public void Tick()
        {
            if (totalMilliseconds <= stopwatch.ElapsedMilliseconds)
                IsFinished = true;
        }
    }
}
