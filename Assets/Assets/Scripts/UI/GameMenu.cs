using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Collections;

using System.Collections.Generic;

public class GameMenu : MonoBehaviour {

    private int category;
    private int index;

    private ConflictRespone conflict = new ConflictRespone();

    //generic values

    [SerializeField]
	Slider UI_audio;
    [SerializeField]
    Text UI_audioText;
	[SerializeField]
	Dropdown UI_shadow;
	[SerializeField]
	Dropdown UI_shadowQuality;
	[SerializeField]
	Dropdown UI_antiAliasing;
	[SerializeField]
	Dropdown UI_vSync;
	[SerializeField]
	Dropdown UI_resolution;
	[SerializeField]
	Toggle UI_fullScreen;

    [SerializeField]
    Toggle UI_MW_Switch;

    [SerializeField]
    GameObject blocker;

	[SerializeField]
	ScrollRect controlsMenu;

	[SerializeField]
	GameObject popupMenu;

	[SerializeField]
    GameObject conflictMenu;
	[SerializeField]
	Text conflictText;
	[SerializeField]
	Button switchButton;
	[SerializeField]
	Button cancelButton;

    [SerializeField]
    Button quitButton;

	[SerializeField]
	GameObject categoryT;
	[SerializeField]
	GameObject bindT;

	Resolution[] validResolutions;
	Transform sun;
	Light[] suns = new Light[4];

    [SerializeField]
	private MenuType menuType;

    //main menu properties
    [SerializeField]
    int minResolutionWidth;
    [SerializeField]
    int minResolutionHeight;
    [SerializeField]
    string newGameSceneName;
    [SerializeField]
    Button newGameButton;
	[SerializeField]
	InputField UI_playerName;

	[SerializeField]
	string saveFileName;

    [SerializeField]
    KeyBinds[] binds;

    [SerializeField]
    KeyCode cancelKey;

    [SerializeField]
    KeyCode[] unBindableKeys;

    //pause menu properties
    [SerializeField]
    string mainMenuSceneName;
    [SerializeField]
    Button continueButton;
    [SerializeField]
    GameObject mainMenu;
    [SerializeField]
    GameObject GameUI;

    bool paused;

    //static values
    static bool InitScript;
    static int minResolutionWidthS;
    static int minResolutionHeightS;
	static KeyBinds[] sBinds;
    static KeyCode[] sUnBindableKeys;
    static KeyCode sCancelKey;
    static string savePath;

    //script variables
    [SerializeField]
    KeyCode pauseKey;

    void Start () {
		sun = GameObject.Find ("Sun").transform;
        Controls.listeners = new ControlableBehaviour[0];
		if (!InitScript) 
		{
			//firt time running script
			minResolutionWidthS = minResolutionWidth;
			minResolutionHeightS = minResolutionHeight;
            savePath = Path.Combine(Application.dataPath, saveFileName);

            if (menuType == MenuType.PauseMenu)
            {
                binds = LoadBinds();
            }
			sBinds = binds;
            sUnBindableKeys = unBindableKeys;
            sCancelKey = cancelKey;
		}

		for (int i = 0; i < sun.childCount; i++)
			suns [i] = sun.GetChild (i).GetComponent<Light> ();
		
		LoadValues ();
		SetUpUI ();

		if(!InitScript)
		{
			InitScript = true;
		}
        pauseKey = Controls.controls["Menu.Pause"];
    }

	void LoadValues()
	{
        Game.audio = PlayerPrefs.GetInt ("audio", 1);
        Game.shadow = PlayerPrefs.GetInt ("shadow", 2);
        Game.shadowQuality = PlayerPrefs.GetInt ("shadowQuality", 3);
        Game.antiAliasing = PlayerPrefs.GetInt ("antiAliasing", 2);
        Game.vSync = PlayerPrefs.GetInt ("vSync", 2);
        Game.resolution = PlayerPrefs.GetInt ("resolution", Screen.resolutions.Length - 1);
        Game.fullScreen = Tools.IntToBool (PlayerPrefs.GetInt ("fullScreen", 1));
        Game.playerName = PlayerPrefs.GetString ("playerName", "Player");

        Game.MW_Switch = Tools.IntToBool(PlayerPrefs.GetInt("MW_Switch", 1));

		if (File.Exists (savePath)) {
			KeyBinds[] savedBinds = LoadBinds ();

			if (ValidateBinds (savedBinds))
			{
				sBinds = savedBinds;
				binds = savedBinds;
			}
			else
			{
				print ("Unvalid Controls .... Reseting Defaults");
				SaveBinds ();
			}
		}

		else
			SaveBinds ();
	}

	void SetUpUI()
	{
		UI_audio.value = Game.audio;
        if (Game.audio == 0)
            UI_audioText.text = "Disabled";
        else
            UI_audioText.text = Game.audio + "";
        UI_audio.onValueChanged.AddListener (SetAudio);

		UI_shadow.value = Game.shadow;
		UI_shadow.onValueChanged.AddListener (SetShadow);
		ApplyShadow ();

		UI_shadowQuality.value = Game.shadowQuality;
		UI_shadowQuality.onValueChanged.AddListener (SetShadowQuality);
		ApplyShadowQuality ();

		UI_antiAliasing.value = Game.antiAliasing;
		UI_antiAliasing.onValueChanged.AddListener (SetAntiAliasing);
		QualitySettings.antiAliasing = Game.antiAliasing;

		UI_vSync.value = Game.vSync;
		UI_vSync.onValueChanged.AddListener (SetVSync);
		QualitySettings.vSyncCount = Game.vSync;

        #if UNITY_EDITOR
        Game.resolution = 0;
		#endif

		validResolutions = GetValidResolutions ();

		if (Game.resolution > validResolutions.Length)
            Game.resolution = validResolutions.Length - 1;

		for (int i = 0; i < validResolutions.Length; i++)
			UI_resolution.options.Add (new Dropdown.OptionData (validResolutions [i].width + " x " + validResolutions [i].height));
		
		UI_resolution.value = Game.resolution;
		UI_resolution.captionText.text = UI_resolution.options [Game.resolution].text;
		UI_resolution.onValueChanged.AddListener (SetResolution);

		UI_fullScreen.isOn = Game.fullScreen;
		UI_fullScreen.onValueChanged.AddListener (SetFullScreen);

        UI_MW_Switch.isOn = Game.MW_Switch;
        UI_MW_Switch.onValueChanged.AddListener(SetMW_Switch);


        if (menuType == MenuType.MainMenu) {
			quitButton.onClick.AddListener(Quit);
            newGameButton.onClick.AddListener(() => LoadLevel(newGameSceneName));
            UI_playerName.text = Game.playerName;
			UI_playerName.onValueChanged.AddListener (SetPlayerName);
		}
		else {
            continueButton.onClick.AddListener(Continue);
			quitButton.onClick.AddListener(()=>LoadLevel(mainMenuSceneName));
		}

        SetUpBinds();
    }

	void SetUpBinds()
	{
        Button[] categoryButtons = new Button[sBinds.Length];
        RectTransform[] categoryRects = new RectTransform[sBinds.Length];

        switchButton.onClick.AddListener(SetSwitch);
        cancelButton.onClick.AddListener(() => SetEnable(conflictMenu, false));
        cancelButton.onClick.AddListener(() => SetEnable(blocker, false));

        for (int i = 0; i < sBinds.Length; i++)
        {
            GameObject categoryObject = Instantiate(categoryT);

            RectTransform categoryRect = categoryObject.GetComponent<RectTransform>();
            categoryRect.name = sBinds[i].category;
            categoryRect.SetParent(controlsMenu.GetComponent<RectTransform>());
            categoryRect.anchoredPosition = new Vector2(i * (categoryRect.sizeDelta.x + 10), 0);

            Text categoryText = categoryRect.Find("Text").GetComponent<Text>();
            categoryText.text = sBinds[i].category;

            Button categoryB = categoryObject.GetComponent<Button>();
            categoryButtons[i] = categoryB;
            SetScrollHeight(categoryB, (sBinds[i].binds.Length * (bindT.GetComponent<RectTransform>().sizeDelta.y + 10)) + 10);

            GameObject categoryGroup = new GameObject();
            categoryGroup.SetActive(false);

            RectTransform categoryGRect = categoryGroup.AddComponent<RectTransform>();
            categoryGRect.name = sBinds[i].category + " Group";
            categoryGRect.SetParent(controlsMenu.content);

            categoryGRect.pivot = new Vector2(0.5f, 0.5f);

            categoryGRect.anchorMin = Vector2.zero;
            categoryGRect.anchorMax = Vector2.one;

            categoryGRect.offsetMin = Vector2.zero;
            categoryGRect.offsetMax = Vector2.zero;

            categoryRects[i] = categoryGRect;
        }

        if (categoryRects.Length > 0)
        {
            categoryButtons[0].onClick.Invoke();
            categoryRects[0].gameObject.SetActive(true);
        }

        for (int i = 0; i < sBinds.Length; i++)
        {
            for (int x = 0; x < sBinds.Length; x++)
            {
                if (i == x)
                    SetEnable(categoryButtons[i], categoryRects[x].gameObject, true);
                else
                    SetEnable(categoryButtons[i], categoryRects[x].gameObject, false);
            }
            SetUpBind(i, categoryRects[i]);
        }
    }

	void SetUpBind(int cat,RectTransform parent)
	{
        for (int i = 0; i < sBinds[cat].binds.Length; i++)
        {
            GameObject bindO = Instantiate(bindT);

            RectTransform bindRect = bindO.GetComponent<RectTransform>();
            bindRect.name = sBinds[cat].binds[i].name;
            bindRect.SetParent(parent);
            bindRect.anchoredPosition = new Vector2(0, i * -(bindT.GetComponent<RectTransform>().sizeDelta.y + 10));
            bindRect.anchoredPosition = new Vector2(0, bindRect.anchoredPosition.y - 10);
            bindRect.offsetMin = new Vector2(15, bindRect.offsetMin.y);
            bindRect.offsetMax = new Vector2(-15, bindRect.offsetMax.y);

            Text keyT = bindRect.Find("name").GetComponent<Text>();
            keyT.text = sBinds[cat].binds[i].name;

            Button keyB = bindRect.Find("keyCode").GetComponent<Button>();
            SetEnable(keyB, popupMenu, true);

            SetBeginEdit(keyB, cat, i);

            Text keyBT = bindRect.Find("keyCode/Text").GetComponent<Text>();
            keyBT.text = sBinds[cat].binds[i].keyCode.ToString();
            sBinds[cat].binds[i].bindText = keyBT;

            if (!InitScript)
                Controls.controls.Add(sBinds[cat].category+"."+sBinds[cat].binds[i].name, sBinds[cat].binds[i].keyCode);
        }
    }

    void TogglePause()
    {
        paused = !paused;
        Game.paused = paused;
        if(paused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

        mainMenu.SetActive(paused);
        GameUI.SetActive(!paused);
    }

    void Update()
    {
        if(menuType == MenuType.PauseMenu)
        {
            if (Input.GetKeyDown(pauseKey))
            {
                if (!paused)
                    TogglePause();
                else if (paused && mainMenu.activeInHierarchy)
                    TogglePause();
            }
        }
    }

    void Continue()
    {
        if (paused)
            TogglePause();
    }

    void SetSwitch()
    {
        KeyCode temp = new KeyCode();

        temp = sBinds[category].binds[index].keyCode;

        sBinds[category].binds[index].keyCode = sBinds[conflict.category].binds[conflict.index].keyCode;

        sBinds[conflict.category].binds[conflict.index].keyCode = temp;

        RefreshField(category, index);
        RefreshField(conflict.category, conflict.index);
        conflictMenu.SetActive(false);
        blocker.SetActive(false);
    }

    void SetScrollHeight(Button eventButton, float height)
    {
        eventButton.onClick.AddListener(() => SetScrollHeight(height));
    }

    void SetScrollHeight(float height)
    {
        controlsMenu.content.anchoredPosition = Vector2.zero;
        controlsMenu.content.offsetMin = new Vector2(controlsMenu.content.offsetMin.x, -height);
    }

    void SetEnable(Button eventButton, GameObject eventObject, bool active)
    {
        eventButton.onClick.AddListener(() => SetEnable(eventObject, active));
    }

    void SetEnable(GameObject eventObject, bool active)
    {
        eventObject.SetActive(active);
    }

    void SetBeginEdit(Button eventButton, int cat, int ind)
    {
        eventButton.onClick.AddListener(() => SetBeginEdit(cat, ind));
    }

    void SetBeginEdit(int cat, int ind)
    {
        category = cat;
        index = ind;
        blocker.SetActive(true);
    }

    void RefreshField(int cat, int ind)
    {
        binds = sBinds;
        sBinds[cat].binds[ind].bindText.text = sBinds[cat].binds[ind].keyCode.ToString();

        Controls.controls[sBinds[cat].category + "." + sBinds[cat].binds[ind].name] =  sBinds[cat].binds[ind].keyCode;
        Controls.RefreshListeners();

        pauseKey = Controls.controls["Menu.Pause"];
        SaveBinds();
    }

    ConflictRespone checkConflict(int cat, KeyCode newKeyCode)
    {
        for (int i = 0; i < sBinds[cat].binds.Length; i++)
        {
            if (sBinds[cat].binds[i].keyCode == newKeyCode)
            {
                if (i != index)
                    return new ConflictRespone(true, cat, i);
            }
        }

        return new ConflictRespone(false, 0, 0);
    }

    void SetAudio(float newAudio)
	{
        Game.audio = Mathf.RoundToInt (newAudio);
		PlayerPrefs.SetInt ("audio", Game.audio);
        if(Game.audio == 0)
            UI_audioText.text = "Disabled";
        else
            UI_audioText.text = Game.audio + "";
    }

	void SetShadow(int newShadow)
	{
        Game.shadow = newShadow;
		PlayerPrefs.SetInt ("shadow", Game.shadow);
		ApplyShadow ();
	}

	void ApplyShadow()
	{
		for (int i = 0; i < suns.Length; i++) {
			suns [i].shadows = (LightShadows)Game.shadow;
		}
	}

	void SetShadowQuality(int newShadowQuality)
	{
        Game.shadowQuality = newShadowQuality;
		PlayerPrefs.SetInt ("shadowQuality", Game.shadowQuality);
		ApplyShadowQuality ();
	}

	void ApplyShadowQuality()
	{
		for (int i = 0; i < suns.Length; i++) {
			suns [i].gameObject.SetActive(false);
		}
		suns [Game.shadowQuality].gameObject.SetActive(true);
	}

	void SetAntiAliasing(int newAntiAliasing)
	{
        Game.antiAliasing = newAntiAliasing;
		PlayerPrefs.SetInt ("antiAliasing", Game.antiAliasing);
		QualitySettings.antiAliasing = Game.antiAliasing;
	}

	void SetVSync(int newVSync)
	{
        Game.vSync = newVSync;
		PlayerPrefs.SetInt ("vSync", Game.vSync);
		QualitySettings.vSyncCount = Game.vSync;
	}

	void SetResolution(int newResolution)
	{
        Game.resolution = newResolution;
		PlayerPrefs.SetInt ("resolution", Game.resolution);
		Screen.SetResolution (validResolutions [Game.resolution].width, validResolutions [Game.resolution].height, Game.fullScreen);
	}

	void SetFullScreen(bool newFullScreen)
	{
        Game.fullScreen = newFullScreen;
		PlayerPrefs.SetInt ("fullScreen", Tools.BoolToInt(Game.fullScreen));
		Screen.SetResolution (validResolutions [Game.resolution].width, validResolutions [Game.resolution].height, Game.fullScreen);
	}

	void SetPlayerName(string newPlayerName)
	{
        Game.playerName = newPlayerName;
		PlayerPrefs.SetString ("playerName", Game.playerName);
	}

    void SetMW_Switch(bool newMW_Switch)
    {
        Game.MW_Switch = newMW_Switch;
        PlayerPrefs.SetInt("MW_Switch", Tools.BoolToInt(Game.MW_Switch));
    }

    public void Quit()
    {
        Application.Quit();
    }

	void LoadLevel(string levelName)
	{
        Game.paused = false;
		SceneManager.LoadScene (levelName);
	}

	Resolution[] GetValidResolutions()
	{
		List<Resolution> allValid = new List<Resolution> ();
		for (int i = 0; i < Screen.resolutions.Length; i++) {
			if(Screen.resolutions[i].width >= minResolutionWidthS && Screen.resolutions[i].height >= minResolutionHeightS)
				allValid.Add(Screen.resolutions[i]);
		}
		return allValid.ToArray ();
	}

    void OnGUI()
    {
        Event eve = Event.current;
        KeyCode eveKey = eve.keyCode;

        if(eve.isMouse)
        {
            for (int i = 0; i < 6 ; i++)
            {
                if (Input.GetMouseButton(i))
                    eveKey = (KeyCode)Enum.Parse(typeof(KeyCode),"Mouse" + i);
            }
        }

        if (eve.shift)
        {
            if(Input.GetKey(KeyCode.LeftShift))
                eveKey = KeyCode.LeftShift;
            if (Input.GetKey(KeyCode.RightShift))
                eveKey = KeyCode.RightShift;
        }

        if (popupMenu.activeInHierarchy && eveKey != KeyCode.None)
        {
            if (eveKey == sCancelKey)
            {
                popupMenu.SetActive(false);
                return;
            }
            else if (ContainsKey(eveKey) || eveKey == KeyCode.BackQuote)
            {
                return;
            }

            conflict = checkConflict(category, eveKey);
            if (conflict.found)
            {
                conflictMenu.SetActive(true);
                conflictText.text = '[' + eveKey.ToString() + ']' + " Is Already Assigned With \n" + sBinds[conflict.category].binds[conflict.index].name;
            }
            else
            {
                sBinds[category].binds[index].keyCode = eveKey;
                blocker.SetActive(false);
            }
            RefreshField(category, index);
            popupMenu.SetActive(false);
        }
    }

    bool ContainsKey(KeyCode _key)
    {
        for (int i = 0; i < sUnBindableKeys.Length; i++)
        {
            if (_key == sUnBindableKeys[i])
                return true;
        }

        return false;
    }

    //save and load

    void SaveBinds()
	{
		string saveText = "";
		for (int x = 0; x < sBinds.Length; x++) {
			for (int y = 0; y < sBinds[x].binds.Length; y++) {
				saveText += "  name=" + sBinds [x].binds [y].name + '\n' + "  keyCode=" + (int)sBinds [x].binds [y].keyCode + '\n';
			}
			saveText += "Category=" + sBinds [x].category + '\n' + '\n';
		}
		File.WriteAllText (savePath, saveText);
	}

	KeyBinds[] LoadBinds()
	{
		List<KeyBinds> savedBinds = new List<KeyBinds> ();
		List<KeyBind> savedBind = new List<KeyBind> ();

		KeyBind tempBind = new KeyBind("",KeyCode.None);

		int keyCodeInd = 0;

		string[] savedText = File.ReadAllLines (savePath);

		for (int i = 0; i < savedText.Length; i++) {
			if (savedText [i].Contains ("Category")) {
				savedBinds.Add (new KeyBinds (Tools.GetValue(savedText[i]),savedBind.ToArray()));
				savedBind.Clear ();
			}
			if (savedText [i].Contains ("name")) {
				tempBind.name = Tools.GetValue (savedText [i]);
			}
			if (savedText [i].Contains ("keyCode")) {
				if (int.TryParse (Tools.GetValue(savedText [i]), out keyCodeInd)) {
					if (Enum.IsDefined (typeof(KeyCode), keyCodeInd)) {
						tempBind.keyCode = (KeyCode)keyCodeInd;
						savedBind.Add (new KeyBind (tempBind.name, tempBind.keyCode));
					}
				}
			}
		}

		return savedBinds.ToArray ();
	}

	bool ValidateBinds(KeyBinds[] savedBinds)
	{
		if (sBinds.Length != savedBinds.Length)
			return false;
		for (int x = 0; x < sBinds.Length; x++) {
			if (sBinds [x].binds.Length != savedBinds [x].binds.Length)
				return false;
			for (int y = 0; y < sBinds [x].binds.Length; y++) {
				if (sBinds [x].binds [y].name != savedBinds [x].binds [y].name)
					return false;
			}
		}

		return true;
	}
}

public static class Controls
{
	internal static Dictionary<string,KeyCode> controls = new Dictionary<string, KeyCode>();
    internal static ControlableBehaviour[] listeners = new ControlableBehaviour[0];

    internal static void AddListener(ControlableBehaviour newBehaviour)
    {
        ControlableBehaviour[] newListeners = new ControlableBehaviour[listeners.Length + 1];
        for (int i = 0; i < listeners.Length; i++)
        {
            newListeners[i] = listeners[i];
        }
        newListeners[listeners.Length] = newBehaviour;
        listeners = newListeners;
    }

    internal static void RefreshListeners()
    {
        for (int i = 0; i < listeners.Length; i++)
        {
            listeners[i].RefreshBinds();
        }
    }
}

public static class Game
{
    static internal int audio;
    static internal int shadow;
    static internal int shadowQuality;
    static internal int antiAliasing;
    static internal int vSync;
    static internal int resolution;
    static internal bool fullScreen;
    static internal string playerName;

    static internal bool paused;

    static internal bool MW_Switch = true;
}

[System.Serializable]
public class KeyBinds
{
    [SerializeField]
	internal string category;
    [SerializeField]
	internal KeyBind[] binds;

	public KeyBinds(string newCategory, KeyBind[] newBinds)
	{
		category = newCategory;
		binds = newBinds;
	}
}

public class ConflictRespone
{
    internal bool found;
    internal int category;
    internal int index;

    public ConflictRespone()
    {
        found = false;
        category = 0;
        index = 0;
    }

    public ConflictRespone(bool newFound, int newCategory, int newIndex)
    {
        found = newFound;
        category = newCategory;
        index = newIndex;
    }
}

[System.Serializable]
public class KeyBind
{
    [SerializeField]
	internal string name;
    [SerializeField]
	internal KeyCode keyCode;

    internal Text bindText;

    public KeyBind(string newName, KeyCode newKeyCode)
	{
		name = newName;
		keyCode = newKeyCode;
	}
}

abstract public class ControlableBehaviour : MonoBehaviour
{
    abstract public void RefreshBinds();
}

public enum MenuType
{
	MainMenu,
	PauseMenu
}