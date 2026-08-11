using UnityEngine;

[DisallowMultipleComponent]
public sealed class GameState : MonoBehaviour
{
    public bool PuzzleSolved { get; private set; }
    public bool HasEnteredClassroom { get; private set; }
    public event System.Action PuzzleCompleted;

    public bool HasKeycard { get; private set; }
    public string CurrentObjective { get; private set; }

    private void Awake()
    {
        HasKeycard = false;
        PuzzleSolved = false;
        HasEnteredClassroom = false;
        CurrentObjective = "Найдите ключ-карту";
    }

    public bool TryEnterClassroom()
    {
        if (!HasKeycard || HasEnteredClassroom)
            return false;

        HasEnteredClassroom = true;
        CurrentObjective = "Найдите свой рюкзак";
        return true;
    }

    public bool TrySolvePuzzle()
    {
        if (PuzzleSolved)
            return false;

        PuzzleSolved = true;
        PuzzleCompleted?.Invoke();
        return true;
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
