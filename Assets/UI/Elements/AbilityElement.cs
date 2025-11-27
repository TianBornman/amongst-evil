using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Midevil.UI.Elements
{
	[UxmlElement]
	public partial class AbilityElement : BindableElement
	{
		private VisualElement icon;
		private Label chargesLabel;
		private Label cooldownLabel;

		[UxmlAttribute, CreateProperty]
		public Texture2D IconTexture
		{
			get => icon.style.backgroundImage.value.texture;
			set => icon.style.backgroundImage = new StyleBackground(value);
		}

		[UxmlAttribute, CreateProperty]
		public string Charges
		{
			get => chargesLabel.text;
			set => chargesLabel.text = value;
		}

		[UxmlAttribute, CreateProperty]
		public string Cooldown
		{
			get => cooldownLabel.text;
			set => cooldownLabel.text = value;
		}

		public AbilityElement()
		{
			// Root styling 
			AddToClassList("ability-slot");

			// Create children
			icon = new VisualElement();

			chargesLabel = new Label("5");
			chargesLabel.name = "charges";

			cooldownLabel = new Label("2");
			cooldownLabel.name = "cooldown";

			Add(icon);
			Add(chargesLabel);
			Add(cooldownLabel);

			ClearItem();
		}

		// Public accessors
		public VisualElement Icon => icon;
		public Label ChargesLabel => chargesLabel;
		public Label CooldownLabel => cooldownLabel;

		// Public Methods
		public void SetAbility(Ability.Ability ability)
		{
			if (!ability.isConsumable)
				chargesLabel.visible = false;

			cooldownLabel.visible = false;
		}

		public void ClearItem()
		{
			IconTexture = null;
			Charges = string.Empty;
			Cooldown = string.Empty;
		}
	}
}
