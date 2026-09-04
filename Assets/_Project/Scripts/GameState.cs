using UnityEngine;

[DisallowMultipleComponent]
public sealed class GameState : MonoBehaviour
{
    public bool HasBackpack { get; private set; }
    public event System.Action BackpackCollected;

    public bool PuzzleSolved { get; private set; }
    public bool HasEnteredClassroom { get; private set; }
    public event System.Action PuzzleCompleted;

    public bool HasKeycard { get; private set; }
    public bool HasLeftLibrary { get; private set; }
    public string CurrentObjective { get; private set; }

    private void Awake()
    {
        HasKeycard = false;
        HasLeftLibrary = false;
        HasBackpack = false;
        PuzzleSolved = false;
        HasEnteredClassroom = false;
        CurrentObjective = "Найдите ключ-карту";
    }

    public bool TryEnterClassroom()
    {
        if (!HasKeycard || HasEnteredClassroom || HasBackpack)
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

    public bool TryCollectBackpack()
    {
        if (!PuzzleSolved || HasBackpack)
            return false;

        HasBackpack = true;
        CurrentObjective = "Покиньте университет";
        BackpackCollected?.Invoke();
        return true;
    }

    public bool TryCollectKeycard()
    {
        if (HasKeycard)
            return false;

        HasKeycard = true;
        if (!HasBackpack && !HasEnteredClassroom)
            CurrentObjective = "Покиньте библиотеку";
        return true;
    }

    public bool TryLeaveLibrary()
    {
        if (!HasKeycard || HasLeftLibrary || HasEnteredClassroom || HasBackpack)
            return false;

        HasLeftLibrary = true;
        CurrentObjective = "Доберитесь до кабинета";
        return true;
    }
}
