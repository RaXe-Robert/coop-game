using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class AutoStartRoom : MonoBehaviour
{
    private const string cEditorPrefAutoStartRoom = "AutoStartRoom";

    private static bool AutoCreateRoom
    {
        get { return EditorPrefs.GetBool(cEditorPrefAutoStartRoom, false); }
        set { EditorPrefs.SetBool(cEditorPrefAutoStartRoom, value); }
    }

    [MenuItem("Testing/Photon/Enable auto create room", true)]
    private static bool ShowEnableAutoCreateRoom()
    {
        return !AutoCreateRoom;
    }
    [MenuItem("Testing/Photon/Enable auto create room")]
    private static void EnableAutoCreateRoom()
    {
        AutoCreateRoom = true;
    }

    [MenuItem("Testing/Photon/Disable auto create room", true)]
    private static bool ShowDisableAutoCreateRoom()
    {
        return AutoCreateRoom;
    }
    [MenuItem("Testing/Photon/Disable auto create room")]
    private static void DisableAutoCreateRoom()
    {
        AutoCreateRoom = false;
    }
}

