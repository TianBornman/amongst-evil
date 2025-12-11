using Midevil.Item;

public class RecruitCharacterEquipment : CharacterEquipment
{
	// Override Methods
	public override void EquipItem(ItemStats item)
	{
		base.EquipItem(item);

		InventoryManager.Instance.RemoveItem(item);
		HubUiManager.Instance.UpdateRecruitmentUI(character);
	}

	public override void UnequipItem(ItemStats item)
	{
		base.UnequipItem(item);

		InventoryManager.Instance.AddItem(item);
		HubUiManager.Instance.UpdateRecruitmentUI(character);
	}
}
