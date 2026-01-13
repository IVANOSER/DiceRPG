using UnityEngine;

public static class UltimateConfigLoader
{
    private static UltimateConfig _cached;

    // шлях ВІД Resources, БЕЗ .json
    private const string RESOURCE_PATH = "Data/Configs/ultimates";

    public static UltimateConfig Get()
    {
        if (_cached != null)
            return _cached;

        TextAsset jsonAsset = Resources.Load<TextAsset>(RESOURCE_PATH);

        if (jsonAsset == null)
        {
            Debug.LogError($"ultimates.json not found in Resources at: {RESOURCE_PATH}");
            _cached = new UltimateConfig(); // дефолт, щоб гра не падала
            return _cached;
        }

        _cached = JsonUtility.FromJson<UltimateConfig>(jsonAsset.text);
        return _cached;
    }

    public static void ClearCache()
    {
        _cached = null;
    }
}
