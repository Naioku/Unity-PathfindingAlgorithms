namespace InteractionSystem
{
    public interface IInteractable
    {
        void Interact(Interaction.InteractionDataSystem interactionDataSystem, Interaction.InteractionDataArgs interactionDataArgs);
    }
}