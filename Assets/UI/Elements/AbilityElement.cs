using UnityEngine;
using UnityEngine.UIElements;

namespace Midevil.UI.Elements
{
	[UxmlElement]
	public partial class AbilityElement : ClickableElement
	{
		private VisualElement icon;
		private Label chargesLabel;
		private Label cooldownLabel;

		[UxmlAttribute]
		public Texture2D IconTexture
		{
			get => icon.style.backgroundImage.value.texture;
			set => icon.style.backgroundImage = new StyleBackground(value);
		}

		[UxmlAttribute]
		public string Charges
		{
			get => chargesLabel.text;
			set => chargesLabel.text = value;
		}

		[UxmlAttribute]
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
			IconTexture = ability.data.icon;
			Charges = ability.RemainingCharges.ToString();
			Cooldown = ability.CooldownTimer.ToString("f1");

			if (!ability.isConsumable)
				chargesLabel.visible = false;

			cooldownLabel.visible = false;

			// Subscribe to live updates
			ability.OnChargesChanged = UpdateCharges;
			ability.OnCooldownChanged = UpdateCooldown;
		}

		public void ClearItem()
		{
			IconTexture = null;
			Charges = string.Empty;
			Cooldown = string.Empty;

			UnsetClickHandler();
		}

		private void UpdateCharges(int value)
		{
			Charges = value.ToString();
		}

		private void UpdateCooldown(float value)
		{
			if (value <= 0)
				cooldownLabel.visible = false;
			else
				cooldownLabel.visible = true;

			Cooldown = value.ToString("f1");
		}
	}
}
