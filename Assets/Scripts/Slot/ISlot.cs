namespace Assembly_CSharp.Assets.Scripts.Slot
{
    public interface ISlot
    {
        void OnInit();
        void OnUpdate();
        SlotStateType GetSlotState();
        void ChangeState(SlotStateType treeType);
    }

}
