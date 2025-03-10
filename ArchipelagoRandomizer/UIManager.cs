using DDoor.ArchipelagoRandomizer.UI;

namespace DDoor.ArchipelagoRandomizer;

internal class UIManager
{
	public static readonly UIManager instance = new();
	private readonly NotificationPopup notificationHandler = new();

	public static UIManager Instance => instance;

	private UIManager()
	{
		CustomUI.CacheBackgroundSprite();
	}

	public void ShowNotification(string message)
	{
		notificationHandler.Show(message);
	}
}