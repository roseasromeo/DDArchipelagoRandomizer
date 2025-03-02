using HarmonyLib;
using MagicUI.Core;
using MagicUI.Elements;
using System.Collections;
using UnityEngine;
using AGM = DDoor.AlternativeGameModes;

namespace DDoor.ArchipelagoRandomizer;

[HarmonyPatch]
internal class UIManager : MonoBehaviour
{
	private static UIManager instance;
	private LayoutRoot apMenuRoot;
	private SaveMenu saveMenu;
	private readonly float apMenuFadeDuration = 0.4f;
	private bool isAPMenuFading;
	private readonly float elementPadding = 15;

	public static UIManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameObject("APUIManager").AddComponent<UIManager>();
			}

			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private struct APMenuElements
	{
		public static TextInput urlTextInput;
		public static TextInput portTextInput;
		public static TextInput slotNameTextInput;
		public static TextInput passwordTextInput;

		public static TextObject HeadingText
		{
			get
			{
				return new TextObject(instance.apMenuRoot, nameof(HeadingText))
				{
					Text = "Archipelago Connection Settings",
					FontSize = 90,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Padding = new Padding(instance.elementPadding),
				};
			}
		}

		public static StackLayout URLPortStack
		{
			get
			{
				StackLayout stack = new StackLayout(instance.apMenuRoot, nameof(URLPortStack))
				{
					Orientation = Orientation.Horizontal,
				};
				stack.Children.Add(URLTextInput);
				stack.Children.Add(PortTextInput);
				return stack;
			}
		}

		public static TextInput URLTextInput
		{
			get
			{
				urlTextInput = new TextInput(instance.apMenuRoot, nameof(URLTextInput))
				{
					MinWidth = 400,
					FontSize = 50,
					Placeholder = "URL (eg. archipelago.gg)",
					Padding = new Padding(instance.elementPadding),
				};
				return urlTextInput;
			}
		}

		public static TextInput PortTextInput
		{
			get
			{
				portTextInput = new TextInput(instance.apMenuRoot, nameof(PortTextInput))
				{
					MinWidth = 200,
					FontSize = 50,
					Placeholder = "Port (eg. 38281)",
					Padding = new Padding(instance.elementPadding),
				};
				return portTextInput;
			}
		}

		public static StackLayout SlotNamePasswordStack
		{
			get
			{
				StackLayout stack = new StackLayout(instance.apMenuRoot, nameof(SlotNamePasswordStack))
				{
					Orientation = Orientation.Horizontal,
				};
				stack.Children.Add(SlotNameTextInput);
				stack.Children.Add(PasswordTextInput);
				return stack;
			}
		}

		public static TextInput SlotNameTextInput
		{
			get
			{
				slotNameTextInput = new TextInput(instance.apMenuRoot, nameof(SlotNameTextInput))
				{
					MinWidth = 200,
					FontSize = 50,
					Placeholder = "Player name",
					Padding = new Padding(instance.elementPadding),
				};
				return slotNameTextInput;
			}
		}

		public static TextInput PasswordTextInput
		{
			get
			{
				passwordTextInput = new TextInput(instance.apMenuRoot, nameof(PasswordTextInput))
				{
					MinWidth = 400,
					FontSize = 50,
					Placeholder = "Password",
					Padding = new Padding(instance.elementPadding),
				};
				return passwordTextInput;
			}
		}

		public static StackLayout ButtonsStack
		{
			get
			{
				StackLayout stack = new StackLayout(instance.apMenuRoot, nameof(ButtonsStack))
				{
					Orientation = Orientation.Horizontal,
					HorizontalAlignment = HorizontalAlignment.Center,
				};
				stack.Children.Add(ConnectButton);
				stack.Children.Add(BackButton);
				return stack;
			}
		}

		public static Button ConnectButton
		{
			get
			{
				Button button = new Button(instance.apMenuRoot, nameof(ConnectButton))
				{
					Content = "Connect",
					FontSize = 70,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Bottom,
					Padding = new Padding(instance.elementPadding, instance.elementPadding * 3),
				};
				button.Click += instance.ClickedConnect;
				return button;
			}
		}

		public static Button BackButton
		{
			get
			{
				Button button = new Button(instance.apMenuRoot, nameof(BackButton))
				{
					Content = "Back",
					FontSize = 70,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Bottom,
					Padding = new Padding(instance.elementPadding, instance.elementPadding * 3),
				};
				button.Click += instance.ClickedBack;
				return button;
			}
		}
	}

	public void CreateAPMenu()
	{
		apMenuRoot = new(false, "AP Menu");
		StackLayout mainStack = new StackLayout(apMenuRoot, "MainStack")
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
		};
		mainStack.Children.Add(APMenuElements.HeadingText);
		mainStack.Children.Add(APMenuElements.URLPortStack);
		mainStack.Children.Add(APMenuElements.SlotNamePasswordStack);
		mainStack.Children.Add(APMenuElements.ButtonsStack);
		StartCoroutine(DoAPMenuFade(true));
	}

	private void ClickedConnect(Button button)
	{
		string url = APMenuElements.urlTextInput.Text;
		string slotName = APMenuElements.slotNameTextInput.Text;
		string password = APMenuElements.passwordTextInput.Text;

		if (int.TryParse(APMenuElements.portTextInput.Text, out int port))
		{
			Archipelago.APConnectionInfo connectionInfo = new()
			{
				URL = url,
				Port = port,
				SlotName = slotName,
				Password = password
			};
			ArchipelagoRandomizerMod.Instance.EnableMod(connectionInfo);
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
}