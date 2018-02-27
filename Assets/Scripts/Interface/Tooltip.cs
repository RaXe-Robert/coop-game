using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance { get; private set; }

    [SerializeField] GameObject panel;
    [SerializeField] Text text;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void Show(string text)
    {
        panel.SetActive(true);
        this.text.text = text;
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        panel.transform.position = Input.mousePosition + new Vector3(0, 50, 0);
    }
}
