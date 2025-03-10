using MagicUI.Core;
using MagicUI.Elements;
using System.Collections;
using UnityEngine;

namespace DDoor.ArchipelagoRandomizer.UI;

internal class NotificationPopup : CustomUI
{
	private readonly Padding padding = new(25);
	private readonly int fontSize = 50;
	private readonly float showDuration = 5;
	private TextObject textObj;

	protected override string Name { get; set; } = "NotificationPopup";
	protected override VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Bottom;
	protected override float FadeDuration { get; set; } = 1f;
	protected override bool Persist { get; set; } = true;

	public void Show(string message)
	{
		if (layoutRoot == null)
		{
			Create();
		}

		textObj.Text = message;
		base.Show();

		Plugin.Instance.StartCoroutine(HideAfterTime());
	}

	public override void Hide()
	{
		base.Hide();
	}

	protected override void Create()
	{
		base.Create();

		textObj = new TextObject(layoutRoot, $"{Name}_Text")
		{
			Text = "Hello, world!",
			FontSize = fontSize,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Padding = padding,
		};
		rootPanel.Child = textObj;
	}

	private IEnumerator HideAfterTime()
	{
		yield return new WaitForSeconds(showDuration);
		Hide();
	}
}