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
		private Label controlLabel;

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

		[UxmlAttribute, CreateProperty]
		public string Control
		{
			get => controlLabel.text;
			set => controlLabel.text = value;
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

			controlLabel = new Label("Q");
			controlLabel.name = "control";

			Add(icon);
			Add(chargesLabel);
			Add(cooldownLabel);
			Add(controlLabel);

			ClearAbility();
		}

		// Public accessors
		public VisualElement Icon => icon;
		public Label ChargesLabel => chargesLabel;
		public Label CooldownLabel => cooldownLabel;
		public Label ControlLabel => controlLabel;

		// Public Methods
		public void ClearAbility()
		{
			IconTexture = null;
			Charges = string.Empty;
			Cooldown = string.Empty;
		}
	}
}
