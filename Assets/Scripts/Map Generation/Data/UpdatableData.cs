using UnityEngine;

namespace Assets.Scripts.Map_Generation
{
    public class UpdatableData : ScriptableObject
    {

        public event System.Action OnValuesUpdated;
        public bool AutoUpdate;

#if UNITY_EDITOR

        protected virtual void OnValidate()
        {
            if (AutoUpdate)
                UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
        }

        public void NotifyOfUpdatedValues()
        {
            UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;

            OnValuesUpdated?.Invoke();
        }

#endif
    }
}
