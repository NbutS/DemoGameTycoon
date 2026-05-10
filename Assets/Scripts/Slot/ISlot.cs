namespace Assembly_CSharp.Assets.Scripts.Slot
{
    public interface ISlot
    {
        void OnInit();
        void OnUpdate();
        SlotStateType GetSlotState();
        void ChangeState(SlotStateType treeType);
    }

    public enum SlotStateType
    {
        None,
        Close,
        Opening,
        Ready,
        NotReady,
        NotAvailable,
    }
}
