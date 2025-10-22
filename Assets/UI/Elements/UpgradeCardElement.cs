using System;
using UnityEngine.UIElements;

namespace Midevil.UI.Elements
{
	[UxmlElement]
	public partial class ClickableElement : VisualElement
	{
		private EventCallback<ClickEvent> _clickHandler;

		public void SetClickHandler(Action<ClickableElement> onClick)
		{
			if (_clickHandler != null)
				UnregisterCallback(_clickHandler);

			_clickHandler = evt => onClick(this);
			RegisterCallback(_clickHandler);
		}
	}
}
