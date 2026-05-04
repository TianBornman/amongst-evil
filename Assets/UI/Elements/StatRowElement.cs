using Unity.Properties;
using UnityEngine.UIElements;

namespace Midevil.UI.Elements
{
	public enum StatFormat
	{
		Flat,
		Percent,
		PerSecond,
		Int,
	}

	[UxmlElement]
	public partial class StatRowElement : VisualElement
	{
		private Label labelText;
		private Label baseText;
		private Label deltaText;
		private Label totalText;

		private string statLabel = string.Empty;
		private StatFormat statFormat = StatFormat.Flat;

		[UxmlAttribute, CreateProperty]
		public string Label
		{
			get => statLabel;
			set
			{
				statLabel = value ?? string.Empty;
				if (labelText != null) labelText.text = statLabel;
			}
		}

		[UxmlAttribute, CreateProperty]
		public StatFormat Format
		{
			get => statFormat;
			set => statFormat = value;
		}

		public StatRowElement()
		{
			AddToClassList("stat-row");

			labelText = new Label(statLabel);
			labelText.AddToClassList("stat-row__label");

			baseText = new Label(string.Empty);
			baseText.AddToClassList("stat-row__base");

			deltaText = new Label(string.Empty);
			deltaText.AddToClassList("stat-row__delta");

			totalText = new Label(string.Empty);
			totalText.AddToClassList("stat-row__total");

			Add(labelText);
			Add(baseText);
			Add(deltaText);
			Add(totalText);
		}

		public void SetValues(float baseValue, float totalValue)
		{
			baseText.text = FormatValue(baseValue);
			totalText.text = FormatValue(totalValue);

			float diff = totalValue - baseValue;
			deltaText.RemoveFromClassList("stat-row__delta--positive");
			deltaText.RemoveFromClassList("stat-row__delta--negative");
			deltaText.RemoveFromClassList("stat-row__delta--neutral");

			const float epsilon = 0.0001f;
			if (diff > epsilon)
			{
				deltaText.text = "+" + FormatValue(diff);
				deltaText.AddToClassList("stat-row__delta--positive");
			}
			else if (diff < -epsilon)
			{
				deltaText.text = FormatValue(diff);
				deltaText.AddToClassList("stat-row__delta--negative");
			}
			else
			{
				deltaText.text = "+" + FormatValue(0);
				deltaText.AddToClassList("stat-row__delta--neutral");
			}
		}

		private string FormatValue(float value)
		{
			return statFormat switch
			{
				StatFormat.Percent => (value * 100f).ToString("0.#") + "%",
				StatFormat.PerSecond => value.ToString("0.##") + "/s",
				StatFormat.Int => value.ToString("F0"),
				_ => value.ToString("0.#"),
			};
		}
	}
}
