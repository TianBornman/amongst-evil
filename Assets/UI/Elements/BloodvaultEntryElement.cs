using Midevil.Models;
using UnityEngine;
using UnityEngine.UIElements;

namespace Midevil.UI.Elements
{
	[UxmlElement]
	public partial class BloodVaultEntryElement : ClickableElement
	{
		private VisualElement profile;
		private Label nameLabel;
		private Label causeOfDeathLabel;
		private Label timeOfDeathLabel;

		[UxmlAttribute]
		public Texture2D ProfileTexture
		{
			get => profile.style.backgroundImage.value.texture;
			set => profile.style.backgroundImage = new StyleBackground(value);
		}

		[UxmlAttribute]
		public string Name
		{
			get => nameLabel.text;
			set => nameLabel.text = value;
		}


		[UxmlAttribute]
		public string CauseOfDeath
		{
			get => causeOfDeathLabel.text;
			set => causeOfDeathLabel.text = value;
		}

		[UxmlAttribute]
		public string TimeOfDeath
		{
			get => timeOfDeathLabel.text;
			set => timeOfDeathLabel.text = value;
		}

		public BloodVaultEntryElement()
		{
			// Root styling 
			AddToClassList("blood-vault-entry");

			// Create children
			profile = new VisualElement() { name = "Icon" };
			nameLabel = new Label() { name = "Name" };
			causeOfDeathLabel = new Label() { name = "Cause" };
			timeOfDeathLabel = new Label() { name = "Time" };

			var textContainer = new VisualElement() { name = "TextContainer" };
			textContainer.Add(nameLabel);
			textContainer.Add(causeOfDeathLabel);
			textContainer.Add(timeOfDeathLabel);

			Add(profile);
			Add(textContainer);
		}

		// Public accessors
		public VisualElement Profile => profile;
		public Label NameLabel => nameLabel;
		public Label CauseOfDeathLabel => causeOfDeathLabel;
		public Label TimeOfDeathLabel => timeOfDeathLabel;

		// Public Methods
		public void SetEntry(BloodVaultEntry entry)
		{
			ProfileTexture = entry.identity.profileIcon;
			Name = entry.identity.name;
			CauseOfDeath = entry.causeOfDeath;
			TimeOfDeath = entry.timeOfDeath.ToString("g");
		}
	}
}
