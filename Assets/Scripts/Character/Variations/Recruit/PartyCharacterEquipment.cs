using Midevil.Item;

public class RecruitCharacterEquipment : CharacterEquipment
{
	// Override Methods
	public override void EquipItem(ItemStats item)
	{
		base.EquipItem(item);

		InventoryManager.Instance.RemoveItem(item);
		UiManager.Instance.UpdateCharacterPanels();
	}

	public override void UnequipItem(ItemStats item)
	{
		base.UnequipItem(item);

		InventoryManager.Instance.AddItem(item);
		UiManager.Instance.UpdateCharacterPanels();
	}
}
