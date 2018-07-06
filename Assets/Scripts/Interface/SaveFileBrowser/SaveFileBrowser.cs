using System.Collections.Generic;
using UnityEngine;

public class SaveFileBrowser : MonoBehaviour {

    [SerializeField] private GameObject layoutGroup;
    [SerializeField] private GameObject saveFileListingPrefab;

    private List<SaveFileListing> saveFileListingButtons = new List<SaveFileListing>();

    public SaveDataManifest SelectedSave { get; private set; }

    private void OnEnable()
    {
        RefreshSaveFileList();
    }

    private void RefreshSaveFileList()
    {
        ClearCurrentList();

        SaveDataManifest[] saveManifests = SaveDataManager.Instance.LoadAllManifests();

        foreach (SaveDataManifest saveManifest in saveManifests)
            AddSaveManifest(saveManifest);
    }

    private void AddSaveManifest(SaveDataManifest saveDataManifest)
    {
        GameObject saveFileListingObj = Instantiate(saveFileListingPrefab, layoutGroup.transform, false);

        SaveFileListing saveFileListing = saveFileListingObj.GetComponent<SaveFileListing>();
        saveFileListing.Setup(saveDataManifest, OnSaveSelected);
        
        saveFileListingButtons.Add(saveFileListing);
    }

    private void ClearCurrentList()
    {
        for (int i = saveFileListingButtons.Count - 1; i >= 0; i--)
        {
            GameObject saveFileListingObj = saveFileListingButtons[i].gameObject;
            saveFileListingButtons.Remove(saveFileListingButtons[i]);
            Destroy(saveFileListingObj);
        }

        saveFileListingButtons.Clear();
    }

    private void OnSaveSelected(SaveDataManifest saveDataManifest)
    {
        SelectedSave = saveDataManifest;
    }
}
