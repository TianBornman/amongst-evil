using Midevil.Ability;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Midevil.UI.Elements
{
	[UxmlElement]
	public partial class AbilityElement : ClickableElement
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
		public string Charges
		{
			get => label.text;
			set => label.text = value;
		}

		public AbilityElement()
		{
			// Root styling 
			AddToClassList("ability-slot");

			// Create children
			icon = new VisualElement();
			label = new Label("5");

			Add(icon);
			Add(label);

			ClearItem();
		}

		// Public accessors
		public VisualElement Icon => icon;
		public Label Label => label;

		// Public Methods
		public void SetAbility(Ability.Ability ability)
		{
			IconTexture = ability.data.icon;
			label.text = ability.RemainingCharges.ToString();

			// Subscribe to live updates
			ability.OnChargesChanged += newValue =>
			{
				label.text = newValue.ToString();
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
