using UnityEngine;

[DisallowMultipleComponent]
public sealed class GameState : MonoBehaviour
{
    public bool HasKeycard { get; private set; }
    public string CurrentObjective { get; private set; }

    private void Awake()
    {
        HasKeycard = false;
        CurrentObjective = "Найдите ключ-карту";
    }

    public bool TryCollectKeycard()
    {
        if (HasKeycard)
            return false;

        HasKeycard = true;
        CurrentObjective = "Доберитесь до кабинета";
        return true;
    }
}
