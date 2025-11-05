using Midevil.Effect;
using UnityEngine;
using UnityEngine.UIElements;

namespace Midevil.UI.Elements
{
	[UxmlElement]
	public partial class EffectElement : ClickableElement
	{
		private VisualElement icon;
		private Label countLabel;

		[UxmlAttribute]
		public Texture2D IconTexture
		{
			get => icon.style.backgroundImage.value.texture;
			set => icon.style.backgroundImage = new StyleBackground(value);
		}

		[UxmlAttribute]
		public string Count
		{
			get => countLabel.text;
			set => countLabel.text = value;
		}

		public EffectElement()
		{
			// Root styling 
			AddToClassList("effect");

			// Create children
			icon = new VisualElement();
			countLabel = new Label("5");

			Add(icon);
			Add(countLabel);
		}

		// Public accessors
		public VisualElement Icon => icon;
		public Label CountLabel => countLabel;

		// Public Methods
		public void SetEffect(Effect.Effect effect)
		{
			IconTexture = effect.icon;
			Count = effect.StackCount.ToString();

			if (effect.stackCount <= 1)
				countLabel.visible = false;

			//// Subscribe to live updates
			effect.OnCountChanged = UpdateCount;
		}

		private void UpdateCount(int value)
		{
			Count = value.ToString();
		}
	}
}
