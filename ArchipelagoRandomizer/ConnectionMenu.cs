using HarmonyLib;
using MagicUI.Core;
using MagicUI.Elements;
using System.Collections;
using UnityEngine;
using AGM = DDoor.AlternativeGameModes;

namespace DDoor.ArchipelagoRandomizer;

[HarmonyPatch]
internal class ConnectionMenu : MonoBehaviour
{
	private static ConnectionMenu instance;
	private LayoutRoot apMenuRoot;
	private SaveMenu saveMenu;
	private static APMenuElements apMenuElements;
	private readonly float apMenuFadeDuration = 0.4f;
	private bool isAPMenuFading;
	private readonly float elementPadding = 15;

	public static ConnectionMenu Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameObject("APUIManager").AddComponent<ConnectionMenu>();
			}

			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		apMenuElements = new();
	}

	public void HideAPMenuAndStartGame()
	{
		if (apMenuRoot != null)
		{
			StartCoroutine(DoAPMenuFade(false));
		}

		if (saveMenu == null)
		{
			saveMenu = TitleScreen.instance.saveMenu;
		}

		SaveSlot slot = saveMenu.slots[saveMenu.index].GetComponent<SaveSlot>();
		slot.LoadSave();
	}

	private void CreateAPMenu()
	{
		apMenuRoot = new(false, "AP Menu");
		StackLayout mainStack = new StackLayout(apMenuRoot, "MainStack")
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
		};
		mainStack.Children.Add(apMenuElements.HeadingText);
		mainStack.Children.Add(apMenuElements.URLPortStack);
		mainStack.Children.Add(apMenuElements.SlotNamePasswordStack);
		mainStack.Children.Add(apMenuElements.ButtonsStack);

		PrefillAPMenu(TitleScreen.instance.saveMenu.index);
		StartCoroutine(DoAPMenuFade(true));
	}

	private void PrefillAPMenu(int saveSlot)
	{
		Archipelago.APConnectionInfo connectioninfo = Archipelago.Instance.GetConnectionInfoForFile(saveSlot);

		if (connectioninfo == null)
		{
			return;
		}

		apMenuElements.URLTextInput.Text = connectioninfo.URL;
		apMenuElements.PortTextInput.Text = connectioninfo.Port.ToString();
		apMenuElements.SlotNameTextInput.Text = connectioninfo.SlotName;
		apMenuElements.PasswordTextInput.Text = connectioninfo.Password;
	}

	private void ClickedConnect(Button button)
	{
		string url = apMenuElements.URLTextInput.Text;
		string slotName = apMenuElements.SlotNameTextInput.Text;
		string password = apMenuElements.PasswordTextInput.Text;

		if (int.TryParse(apMenuElements.PortTextInput.Text, out int port))
		{
			Archipelago.APConnectionInfo connectionInfo = new()
			{
				URL = url,
				Port = port,
				SlotName = slotName,
				Password = password
			};
			ArchipelagoRandomizerMod.Instance.EnableMod(connectionInfo, saveMenu.index);
		}
	}

	private void ClickedBack(Button button)
	{
		StartCoroutine(DoAPMenuFade(false));
		StartCoroutine(ShowSaveMenu());
	}

	private void ShowAPMenu(SaveMenu saveMenu)
	{
		this.saveMenu = saveMenu;

		if (apMenuRoot == null)
		{
			CreateAPMenu();
		}
		else
		{
			apMenuRoot.Canvas.SetActive(true);
			PrefillAPMenu(saveMenu.index);
			StartCoroutine(DoAPMenuFade(true));
		}
	}

	private IEnumerator DoAPMenuFade(bool fadeIn)
	{
		isAPMenuFading = true;

		if (fadeIn)
		{
			apMenuRoot.Opacity = 0;
			apMenuRoot.BeginFade(1, apMenuFadeDuration);
		}
		else
		{
			apMenuRoot.BeginFade(0, apMenuFadeDuration);
		}

		yield return new WaitForSeconds(apMenuFadeDuration);

		if (apMenuRoot.Opacity <= 0)
		{
			apMenuRoot.Canvas.SetActive(false);
		}

		isAPMenuFading = false;
	}

	private IEnumerator ShowSaveMenu()
	{
		while (isAPMenuFading)
		{
			yield return null;
		}

		saveMenu.GainFocus(true);
		// Nullify transitionButton to ensure we return to title screen on back input
		saveMenu.transitionButton = null;
		saveMenu.openSubMenu();
	}

	[HarmonyPrefix, HarmonyPatch(typeof(SaveMenu), nameof(SaveMenu.startGame))]
	private static bool StartGamePatch(SaveMenu __instance)
	{
		if (AGM.AlternativeGameModes.SelectedModeName == "ARCHIPELAGO")
		{
			Instance.ShowAPMenu(__instance);
			return false;
		}

		return true;
	}

	private struct APMenuElements
	{
		private TextObject headingText;
		private StackLayout urlPortStack;
		private TextInput urlTextInput;
		private TextInput portTextInput;
		private StackLayout slotNamePasswordStack;
		private TextInput slotNameTextInput;
		private TextInput passwordTextInput;
		private StackLayout buttonsStack;
		private Button connectButton;
		private Button backButton;

		public TextObject HeadingText
		{
			get
			{
				headingText ??= new TextObject(instance.apMenuRoot, nameof(HeadingText))
				{
					Text = "Archipelago Connection Settings",
					FontSize = 90,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Padding = new Padding(instance.elementPadding),
				};

				return headingText;
			}
		}

		public StackLayout URLPortStack
		{
			get
			{
				if (urlPortStack == null)
				{
					urlPortStack = new StackLayout(instance.apMenuRoot, nameof(URLPortStack))
					{
						Orientation = Orientation.Horizontal,
					};
					urlPortStack.Children.Add(URLTextInput);
					urlPortStack.Children.Add(PortTextInput);
				}

				return urlPortStack;
			}
		}

		public TextInput URLTextInput
		{
			get
			{
				urlTextInput ??= new TextInput(instance.apMenuRoot, nameof(URLTextInput))
				{
					MinWidth = 400,
					FontSize = 50,
					Placeholder = "URL (eg. archipelago.gg)",
					Padding = new Padding(instance.elementPadding),
				};

				return urlTextInput;
			}
		}

		public TextInput PortTextInput
		{
			get
			{
				portTextInput ??= new TextInput(instance.apMenuRoot, nameof(PortTextInput))
				{
					MinWidth = 200,
					FontSize = 50,
					Placeholder = "Port (eg. 38281)",
					Padding = new Padding(instance.elementPadding),
				};

				return portTextInput;
			}
		}

		public StackLayout SlotNamePasswordStack
		{
			get
			{
				if (slotNamePasswordStack == null)
				{
					slotNamePasswordStack = new StackLayout(instance.apMenuRoot, nameof(SlotNamePasswordStack))
					{
						Orientation = Orientation.Horizontal,
					};
					slotNamePasswordStack.Children.Add(SlotNameTextInput);
					slotNamePasswordStack.Children.Add(PasswordTextInput);
				}

				return slotNamePasswordStack;
			}
		}

		public TextInput SlotNameTextInput
		{
			get
			{
				slotNameTextInput ??= new TextInput(instance.apMenuRoot, nameof(SlotNameTextInput))
				{
					MinWidth = 200,
					FontSize = 50,
					Placeholder = "Player name",
					Padding = new Padding(instance.elementPadding),
				};

				return slotNameTextInput;
			}
		}

		public TextInput PasswordTextInput
		{
			get
			{
				passwordTextInput ??= new TextInput(instance.apMenuRoot, nameof(PasswordTextInput))
				{
					MinWidth = 400,
					FontSize = 50,
					Placeholder = "Password",
					Padding = new Padding(instance.elementPadding),
				};

				return passwordTextInput;
			}
		}

		public StackLayout ButtonsStack
		{
			get
			{
				if (buttonsStack == null)
				{
					buttonsStack = new StackLayout(instance.apMenuRoot, nameof(ButtonsStack))
					{
						Orientation = Orientation.Horizontal,
						HorizontalAlignment = HorizontalAlignment.Center,
					};
					buttonsStack.Children.Add(ConnectButton);
					buttonsStack.Children.Add(BackButton);
				}

				return buttonsStack;
			}
		}

		public Button ConnectButton
		{
			get
			{
				if (connectButton == null)
				{
					connectButton = new Button(instance.apMenuRoot, nameof(ConnectButton))
					{
						Content = "Connect",
						FontSize = 70,
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Bottom,
						Padding = new Padding(instance.elementPadding, instance.elementPadding * 3),
					};
					connectButton.Click += instance.ClickedConnect;
				}

				return connectButton;
			}
		}

		public Button BackButton
		{
			get
			{
				if (backButton == null)
				{
					backButton = new Button(instance.apMenuRoot, nameof(BackButton))
					{
						Content = "Back",
						FontSize = 70,
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Bottom,
						Padding = new Padding(instance.elementPadding, instance.elementPadding * 3),
					};
					backButton.Click += instance.ClickedBack;
				}

				return backButton;
			}
		}
	}
}