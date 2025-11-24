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
		private Label statusLabel;
		private Label killsLabel;

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
		public string Status
		{
			get => statusLabel.text;
			set => statusLabel.text = value;
		}

		[UxmlAttribute]
		public string Kills
		{
			get => killsLabel.text;
			set => killsLabel.text = value;
		}

		public BloodVaultEntryElement()
		{
			// Root styling 
			AddToClassList("blood-vault-entry");

			// Create children
			profile = new VisualElement() { name = "Icon" };
			nameLabel = new Label() { name = "Name" };
			statusLabel = new Label() { name = "Status" };
			killsLabel = new Label() { name = "Kills" };

			var textContainer = new VisualElement() { name = "TextContainer" };
			textContainer.Add(nameLabel);
			textContainer.Add(statusLabel);
			textContainer.Add(killsLabel);

			Add(profile);
			Add(textContainer);
		}

		// Public accessors
		public VisualElement Profile => profile;
		public Label NameLabel => nameLabel;
		public Label StatusLabel => statusLabel;
		public Label KillsLabel => KillsLabel;

		// Public Methods
		public void SetEntry(BloodVaultEntry entry)
		{
			ProfileTexture = RefManager.Instance.GetIcon(entry.identity.profileIcon);
			Name = entry.identity.characterName;
			Status = entry.status.ToString();
			Kills = entry.identity.lifeTimeResult.kills.ToString();
		}
	}
}
