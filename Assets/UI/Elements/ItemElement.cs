using Midevil.Item;
using UnityEngine;
using UnityEngine.UIElements;

namespace Midevil.UI.Elements
{
	[UxmlElement]
	public partial class ItemElement : ClickableElement
	{
		private VisualElement icon;
		private Label label;

		private Texture2D defaultIcon;
		private string defaultTitle = string.Empty;

		private bool isEquipSlot = false;

		[UxmlAttribute]
		public Texture2D IconTexture
		{
			get => icon.style.backgroundImage.value.texture;
			set => icon.style.backgroundImage = new StyleBackground(value);
		}

		[UxmlAttribute]
		public Texture2D DefaultIconTexture
		{
			get => defaultIcon;
			set => defaultIcon = value;
		}

		[UxmlAttribute]
		public string Title
		{
			get => label.text;
			set => label.text = value;
		}

		[UxmlAttribute]
		public string DefaultTitle
		{
			get => defaultTitle;
			set => defaultTitle = value;
		}

		[UxmlAttribute]
		public bool IsEquipSlot
		{
			get => isEquipSlot;
			set
			{
				isEquipSlot = value;

				if (isEquipSlot)
				{
					RemoveFromClassList("item-slot");
					AddToClassList("equip-slot");
				}
			}
		}

		public bool IsEmpty => IconTexture == defaultIcon && Title == defaultTitle;

		public ItemElement()
		{
			AddToClassList("item-slot");

			// Create children
			icon = new VisualElement();
			label = new Label("Title");

			Add(icon);
			Add(label);

			ClearItem();
		}

		// Public accessors
		public VisualElement Icon => icon;
		public Label Label => label;

		// Public Methods
		public void SetItem(ItemStats item)
		{
			IconTexture = item.icon;
			Title = item.name;

			if (item.type != ItemType.Relic)
				SetClickHandler(evt => EquipItem(item));
		}

		public void ClearItem()
		{
			IconTexture = defaultIcon;
			Title = defaultTitle;

			UnsetClickHandler();
		}

		// Private Methods
		private void EquipItem(ItemStats item)
		{
			// TODO: THis will need to be on a per characeter basis
			//if (isEquipSlot)
			//	PlayerManager.Instance.UnequipItem(item);
			//else
			//	PlayerManager.Instance.EquipItem(item);
		}
	}
}
