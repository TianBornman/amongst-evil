using System;
using UnityEngine.UIElements;

namespace Midevil.UI.Elements
{
	[UxmlElement]
	public partial class ClickableElement : VisualElement
	{
		private EventCallback<ClickEvent> _clickHandler;

		// Public Methods
		public void SetClickHandler(Action<ClickableElement> onClick)
		{
			UnsetClickHandler();

			_clickHandler = evt => onClick(this);
			RegisterCallback(_clickHandler);
		}

		public void UnsetClickHandler()
		{
			if (_clickHandler != null)
			{
				UnregisterCallback(_clickHandler);
				_clickHandler = null;
			}
		}
	}
}
