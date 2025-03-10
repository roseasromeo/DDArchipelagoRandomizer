using DDoor.ArchipelagoRandomizer.UI;

namespace DDoor.ArchipelagoRandomizer;

internal class UIManager
{
	public static readonly UIManager instance = new();
	private static readonly ConnectionMenu connectionMenu = new();
	private static readonly NotificationPopup notificationHandler = new();

	public static UIManager Instance => instance;

	private UIManager()
	{
		CustomUI.CacheBackgroundSprite();
	}

	public void ShowConnectionMenu()
	{
		connectionMenu.Show();
	}

	public void HideConnectionMenu()
	{
		connectionMenu.Hide();
	}

	public void ShowNotification(string message)
	{
		notificationHandler.Show(message);
	}
}