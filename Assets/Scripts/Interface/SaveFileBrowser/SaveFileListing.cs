using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SaveFileListing : MonoBehaviour {

    [SerializeField] private Text nameText;
    [SerializeField] private Button selectButton;

    private SaveDataManifest saveDataManifest;

    public void Setup(SaveDataManifest manifest, UnityAction<SaveDataManifest> selectionCallback)
    {
        saveDataManifest = manifest;
        nameText.text = saveDataManifest.Name;

        selectButton.onClick.AddListener(() => selectionCallback(saveDataManifest));
    }
}
