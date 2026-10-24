using System.Collections;
using UnityEngine;

public class SaveCoordinator : MonoBehaviour
{
    [SerializeField] private SettingsController settings;
    [SerializeField, Min(0.05f)] private float saveDelay = 0.25f;

    public GameSaveData Data { get; private set; }

    private Coroutine _saveRoutine;

    private void Start()
    {
        if (!SaveService.TryLoad(out GameSaveData data))
        {
            data = new GameSaveData();
        }

        Data = data;
        settings.Apply(Data);
        settings.Changed += QueueSave;
    }

    private void OnDestroy()
    {
        if (settings != null)
        {
            settings.Changed -= QueueSave;
        }

        if (_saveRoutine != null)
        {
            StopCoroutine(_saveRoutine);
        }
    }

    public void Save()
    {
        settings.CaptureTo(Data);
        SaveService.Save(Data);
    }

    private void QueueSave()
    {
        if (_saveRoutine != null)
        {
            StopCoroutine(_saveRoutine);
        }

        _saveRoutine = StartCoroutine(SaveAfterDelay());
    }

    private IEnumerator SaveAfterDelay()
    {
        yield return new WaitForSecondsRealtime(saveDelay);
        _saveRoutine = null;
        Save();
    }
}
