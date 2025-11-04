using Midevil.Ability;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Collections.LowLevel.Unsafe;

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
		public string CoolDown
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
			chargesLabel.text = ability.RemainingCharges.ToString("F1");
			cooldownLabel.text = ability.CooldownTimer.ToString("F1");

			if (!ability.isConsumable)
				chargesLabel.visible = false;

			cooldownLabel.visible = false;

			// Subscribe to live updates
			ability.OnChargesChanged += newValue =>
			{
				chargesLabel.text = newValue.ToString("F1");
			};

			ability.OnCooldownChanged += newValue =>
			{
				if (newValue <= 0)
					cooldownLabel.visible = false;
				else
					cooldownLabel.visible = true;

				cooldownLabel.text = newValue.ToString("F1");
			};
		}

		public void ClearItem()
		{
			IconTexture = null;
			Charges = string.Empty;

			UnsetClickHandler();
		}
	}
}
