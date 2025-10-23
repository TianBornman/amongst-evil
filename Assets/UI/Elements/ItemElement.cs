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

		[UxmlAttribute]
		public Texture2D IconTexture
		{
			get => icon.style.backgroundImage.value.texture;
			set => icon.style.backgroundImage = new StyleBackground(value);
		}

		[UxmlAttribute]
		public string Title
		{
			get => label.text;
			set => label.text = value;
		}


		public ItemElement()
		{
			// Root styling 
			AddToClassList("item-slot");

			// Create children
			icon = new VisualElement();
			label = new Label("Title");

			Add(icon);
			Add(label);
		}

		// Public accessors
		public VisualElement Icon => icon;
		public Label Label => label;

		// Public Methods
		public void SetItem(ItemStats item)
		{
			IconTexture = item.icon;
			Title = item.name;

			SetClickHandler(evt => EquipItem(item));
		}

		public void ClearItem()
		{
			IconTexture = null;
			Title = string.Empty;

			UnsetClickHandler();
		}

		// Private Methods
		private void EquipItem(ItemStats item)
		{
			PlayerManager.Instance.EquipItem(item);
			ClearItem();
		}
	}
}
