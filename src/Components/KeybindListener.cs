using UnityEngine;
using BepInEx.Configuration;

namespace AUnlocker.Components;

public class KeybindListener : MonoBehaviour
{
    private static readonly KeyCode[] ReloadKeys =
    {
        KeyCode.F5, KeyCode.F6, KeyCode.F7, KeyCode.F8, KeyCode.F9, KeyCode.F10, KeyCode.F11, KeyCode.F12
    };

    private static readonly string[] AprilFoolsModes =
    {
        "None", "Horse", "Seeker", "Long", "LongHorse", "Classic"
    };

    public AUnlocker Plugin { get; internal set; }

    private bool _showOptions;
    private Rect _window = new(0, 0, 700, 650);
    private Vector2 _scrollPosition;
    private static GUISkin _optionsSkin;
    private static GUIStyle _titleStyle;
    private static GUIStyle _sectionStyle;
    private static GUIStyle _unsafeSectionStyle;
    private static GUIStyle _hintStyle;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) _showOptions = !_showOptions;

        var reloadKey = AUnlocker.ReloadConfigKeybind.Value;
        if (reloadKey == KeyCode.F1 || !Input.GetKeyDown(reloadKey)) return;
        ReloadConfig();
    }

    public void OnGUI()
    {
        if (!_showOptions) return;

        var previousSkin = GUI.skin;
        GUI.skin = GetOptionsSkin();

        _window.width = Mathf.Min(760f, Mathf.Max(360f, Screen.width - 32f));
        _window.height = Mathf.Min(820f, Mathf.Max(360f, Screen.height - 32f));
        _window.x = (Screen.width - _window.width) / 2f;
        _window.y = (Screen.height - _window.height) / 2f;
        GUI.Box(_window, GUIContent.none);
        GUILayout.BeginArea(new Rect(_window.x + 16f, _window.y + 12f, _window.width - 32f, _window.height - 24f));
        try
        {
            DrawOptionsWindow();
        }
        finally
        {
            GUILayout.EndArea();
            GUI.skin = previousSkin;
        }
    }

    private void DrawOptionsWindow()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("AUnlocker Options", _titleStyle, GUILayout.ExpandWidth(true));
        if (GUILayout.Button("X", GUILayout.Width(38f), GUILayout.Height(34f))) _showOptions = false;
        GUILayout.EndHorizontal();
        GUILayout.Label("Changes are saved to AUnlocker.cfg.", _hintStyle);
        GUILayout.Space(10f);

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true,
            GUILayout.Height(Mathf.Max(160f, _window.height - 160f)));

        DrawSection("General");
        DrawKeybind("Reload config key", AUnlocker.ReloadConfigKeybind);
        DrawFloat("Button size", AUnlocker.ButtonSize, 0.5f, 2f);

        DrawSection("Account");
        DrawToggle("Remove guest restrictions", AUnlocker.UnlockGuest);
        DrawToggle("Remove minor restrictions", AUnlocker.UnlockMinor);
        DrawToggle("Remove disconnect penalty", AUnlocker.RemovePenalty);

        DrawSection("Chat");
        DrawToggle("Chat keyboard shortcuts", AUnlocker.ChatKeyboardShortcuts);
        DrawToggle("Allow symbols, URLs, and email addresses", AUnlocker.AllowSymbols);
        DrawToggle("Increase character limit", AUnlocker.HigherCharacterLimit);
        DrawToggle("Reduce chat cooldown", AUnlocker.LowerChatCooldown);
        DrawInt("Chat history limit", AUnlocker.ChatHistoryLimit, 1, 500);

        DrawSection("Cosmetics");
        DrawToggle("Unlock all cosmetics", AUnlocker.UnlockCosmetics);
        DrawToggle("Don't show cosmetics in-game", AUnlocker.DontShowCosmeticsInGame);

        DrawSection("Other");
        DrawInt("FPS cap", AUnlocker.UnlockFPS, 1, 1000);
        DrawToggle("Disable telemetry", AUnlocker.DisableTelemetry);
        DrawChoice("April Fools mode", AUnlocker.AprilFoolsMode, AprilFoolsModes);
        DrawToggle("Show more lobby info", AUnlocker.MoreLobbyInfo);
        DrawToggle("Always show lobby timer", AUnlocker.AlwaysShowLobbyTimer);
        DrawToggle("Show task panel in meetings", AUnlocker.ShowTaskPanelInMeetings);

        DrawSection("Unsafe", true);
        GUILayout.Label("These options can get you kicked by anti-cheat.");
        DrawToggle("No character limit", AUnlocker.NoCharacterLimit);
        DrawToggle("No chat cooldown", AUnlocker.NoChatCooldown);
        DrawToggle("No options limits", AUnlocker.NoOptionsLimits);

        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reload config", GUILayout.Height(38f))) ReloadConfig();
        if (GUILayout.Button("Return to game (F1)", GUILayout.Height(38f))) _showOptions = false;
        GUILayout.EndHorizontal();
    }

    private static void DrawSection(string title, bool unsafeSection = false)
    {
        GUILayout.Space(8f);
        GUILayout.Label(title, unsafeSection ? _unsafeSectionStyle : _sectionStyle);
    }

    private static GUISkin GetOptionsSkin()
    {
        if (_optionsSkin != null) return _optionsSkin;

        _optionsSkin = Instantiate(GUI.skin);
        var panel = CreateTexture(new Color(0.055f, 0.09f, 0.15f, 0.98f));
        var button = CreateTexture(new Color(0.12f, 0.2f, 0.29f, 1f));
        var buttonHover = CreateTexture(new Color(0.08f, 0.42f, 0.46f, 1f));
        var buttonActive = CreateTexture(new Color(0.06f, 0.32f, 0.36f, 1f));
        var accent = CreateTexture(new Color(0.21f, 0.82f, 0.78f, 1f));

        _optionsSkin.box.normal.background = panel;
        _optionsSkin.button.normal.background = button;
        _optionsSkin.button.hover.background = buttonHover;
        _optionsSkin.button.active.background = buttonActive;
        _optionsSkin.button.normal.textColor = Color.white;
        _optionsSkin.button.hover.textColor = Color.white;
        _optionsSkin.button.fontSize = 15;
        _optionsSkin.button.fontStyle = FontStyle.Bold;
        _optionsSkin.toggle.normal.textColor = new Color(0.88f, 0.92f, 0.96f);
        _optionsSkin.toggle.onNormal.textColor = new Color(0.35f, 0.94f, 0.82f);
        _optionsSkin.toggle.fontSize = 15;
        _optionsSkin.toggle.fixedHeight = 30f;
        _optionsSkin.label.normal.textColor = new Color(0.88f, 0.92f, 0.96f);
        _optionsSkin.label.fontSize = 15;
        _optionsSkin.horizontalSliderThumb.normal.background = accent;
        _optionsSkin.horizontalSliderThumb.hover.background = accent;
        _optionsSkin.horizontalSliderThumb.fixedWidth = 18f;
        _optionsSkin.horizontalSliderThumb.fixedHeight = 18f;
        _optionsSkin.verticalScrollbar.fixedWidth = 14f;

        _titleStyle = new GUIStyle(_optionsSkin.label)
        {
            fontSize = 24,
            fontStyle = FontStyle.Bold,
            fixedHeight = 36f
        };
        _titleStyle.normal.textColor = Color.white;

        _sectionStyle = new GUIStyle(_optionsSkin.label)
        {
            fontSize = 18,
            fontStyle = FontStyle.Bold
        };
        _sectionStyle.normal.textColor = new Color(0.28f, 0.86f, 0.82f);

        _unsafeSectionStyle = new GUIStyle(_sectionStyle);
        _unsafeSectionStyle.normal.textColor = new Color(1f, 0.45f, 0.4f);

        _hintStyle = new GUIStyle(_optionsSkin.label)
        {
            fontSize = 13,
            wordWrap = true
        };
        _hintStyle.normal.textColor = new Color(0.62f, 0.72f, 0.8f);

        return _optionsSkin;
    }

    private static Texture2D CreateTexture(Color color)
    {
        var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false)
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }

    private void ReloadConfig()
    {
        Plugin.Config.Reload();
        AUnlocker.Log.LogInfo("Configuration reloaded.");
    }

    private static void DrawToggle(string label, ConfigEntry<bool> setting)
    {
        setting.Value = GUILayout.Toggle(setting.Value, label);
    }

    private static void DrawFloat(string label, ConfigEntry<float> setting, float minimum, float maximum)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"{label}: {setting.Value:0.00}", GUILayout.Width(220));
        setting.Value = GUILayout.HorizontalSlider(setting.Value, minimum, maximum);
        GUILayout.EndHorizontal();
    }

    private static void DrawInt(string label, ConfigEntry<int> setting, int minimum, int maximum)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"{label}: {setting.Value}", GUILayout.Width(220));
        if (GUILayout.Button("-", GUILayout.Width(36))) setting.Value = Mathf.Max(minimum, setting.Value - 1);
        if (GUILayout.Button("+", GUILayout.Width(36))) setting.Value = Mathf.Min(maximum, setting.Value + 1);
        GUILayout.EndHorizontal();
    }

    private static void DrawChoice(string label, ConfigEntry<string> setting, string[] choices)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"{label}: {setting.Value}", GUILayout.Width(220));
        if (GUILayout.Button("<", GUILayout.Width(36))) setting.Value = GetChoice(setting.Value, choices, -1);
        if (GUILayout.Button(">", GUILayout.Width(36))) setting.Value = GetChoice(setting.Value, choices, 1);
        GUILayout.EndHorizontal();
    }

    private static string GetChoice(string current, string[] choices, int direction)
    {
        var index = 0;
        for (var i = 0; i < choices.Length; i++)
        {
            if (choices[i] == current)
            {
                index = i;
                break;
            }
        }

        index = (index + direction + choices.Length) % choices.Length;
        return choices[index];
    }

    private static void DrawKeybind(string label, ConfigEntry<KeyCode> setting)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"{label}: {setting.Value}", GUILayout.Width(220));
        if (GUILayout.Button("Change", GUILayout.Width(80)))
        {
            var index = 0;
            for (var i = 0; i < ReloadKeys.Length; i++)
            {
                if (ReloadKeys[i] == setting.Value)
                {
                    index = i;
                    break;
                }
            }

            setting.Value = ReloadKeys[(index + 1) % ReloadKeys.Length];
        }

        GUILayout.EndHorizontal();
    }
}
