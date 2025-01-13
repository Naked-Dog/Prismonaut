public interface IInteractable
{
    bool IsInteractable { get; }
    bool InteractOnEnter { get; }
    bool DestroyOnInteract { get; }
    void Interact();
}
