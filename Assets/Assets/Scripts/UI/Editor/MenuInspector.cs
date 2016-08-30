using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;

[CustomEditor(typeof(GameMenu)), CanEditMultipleObjects]

public class MenuInspector : Editor {
    SerializedProperty UI_audio;
    SerializedProperty UI_audioText;
    SerializedProperty UI_shadow;
    SerializedProperty UI_shadowQuality;
    SerializedProperty UI_antiAliasing;
    SerializedProperty UI_vSync;
    SerializedProperty UI_resolution;
    SerializedProperty UI_fullScreen;

    SerializedProperty UI_MW_Switch;

    SerializedProperty blocker;

    SerializedProperty controlsMenu;

    SerializedProperty popupMenu;

    SerializedProperty conflictMenu;
    SerializedProperty conflictText;
    SerializedProperty switchButton;
    SerializedProperty cancelButton;

    SerializedProperty quitButton;

    SerializedProperty categoryT;
    SerializedProperty bindT;

    SerializedProperty menuTyp;

    MenuType menuType;

    //Main Menu Values

    SerializedProperty minResolutionWidth;
    SerializedProperty minResolutionHeight;
    SerializedProperty newGameSceneName;
    SerializedProperty newGameButton;
    SerializedProperty UI_playerName;

    SerializedProperty saveFileName;

    SerializedProperty binds;

    SerializedProperty cancelKey;

    SerializedProperty unBindableKeys;

    //Pause Menu Values

    SerializedProperty mainMenuSceneName;
    SerializedProperty continueButton;
    SerializedProperty mainMenu;
    SerializedProperty GameUI;

    bool UI;
    bool templates;
    bool menuT = true;

    void OnEnable()
    {
        UI_audio = serializedObject.FindProperty("UI_audio");
        UI_audioText = serializedObject.FindProperty("UI_audioText");
        UI_shadow = serializedObject.FindProperty("UI_shadow");
        UI_shadowQuality = serializedObject.FindProperty("UI_shadowQuality");
        UI_antiAliasing = serializedObject.FindProperty("UI_antiAliasing");
        UI_vSync = serializedObject.FindProperty("UI_vSync");
        UI_resolution = serializedObject.FindProperty("UI_resolution");
        UI_fullScreen = serializedObject.FindProperty("UI_fullScreen");

        UI_MW_Switch = serializedObject.FindProperty("UI_MW_Switch");

        blocker = serializedObject.FindProperty("blocker");

        controlsMenu = serializedObject.FindProperty("controlsMenu");

        popupMenu = serializedObject.FindProperty("popupMenu");

        conflictMenu = serializedObject.FindProperty("conflictMenu");
        conflictText = serializedObject.FindProperty("conflictText");
        switchButton = serializedObject.FindProperty("switchButton");
        cancelButton = serializedObject.FindProperty("cancelButton");

        quitButton = serializedObject.FindProperty("UI_audio");

        categoryT = serializedObject.FindProperty("categoryT");
        bindT = serializedObject.FindProperty("bindT");

        menuTyp = serializedObject.FindProperty("menuType");

        minResolutionWidth = serializedObject.FindProperty("minResolutionWidth");
        minResolutionHeight = serializedObject.FindProperty("minResolutionHeight");
        newGameSceneName = serializedObject.FindProperty("newGameSceneName");
        newGameButton = serializedObject.FindProperty("newGameButton");
        UI_playerName = serializedObject.FindProperty("UI_playerName");

        saveFileName = serializedObject.FindProperty("saveFileName");

        binds = serializedObject.FindProperty("binds");

        cancelKey = serializedObject.FindProperty("cancelKey");

        unBindableKeys = serializedObject.FindProperty("unBindableKeys");

        mainMenuSceneName = serializedObject.FindProperty("mainMenuSceneName");
        continueButton = serializedObject.FindProperty("continueButton");
        mainMenu = serializedObject.FindProperty("mainMenu");
        GameUI = serializedObject.FindProperty("GameUI");
    }

    public override void OnInspectorGUI()
    {
        GUIContent label = new GUIContent("UI");

        UI = EditorGUILayout.Foldout(UI, label);

        if (UI)
        {
            label.text = "Audio Slider";
            EditorGUILayout.PropertyField(UI_audio, label);

            label.text = "Audio Text Vlaue";
            EditorGUILayout.PropertyField(UI_audioText, label);

            label.text = "Shadow Dropdown";
            EditorGUILayout.PropertyField(UI_shadow, label);

            label.text = "Shadow Quality Dropdown";
            EditorGUILayout.PropertyField(UI_shadowQuality, label);

            label.text = "AntiAliasing Dropdown";
            EditorGUILayout.PropertyField(UI_antiAliasing, label);

            label.text = "V-Sync Dropdown";
            EditorGUILayout.PropertyField(UI_vSync, label);

            label.text = "Resolution Dropdown";
            EditorGUILayout.PropertyField(UI_resolution, label);

            label.text = "Fullscreen Toggle";
            EditorGUILayout.PropertyField(UI_fullScreen, label);

            label.text = "Blocker";
            EditorGUILayout.PropertyField(blocker, label);

            label.text = "Controls Menu";
            EditorGUILayout.PropertyField(controlsMenu, label);

            label.text = "Popup Menu";
            EditorGUILayout.PropertyField(popupMenu, label);

            label.text = "Conflict Menu";
            EditorGUILayout.PropertyField(conflictMenu, label);

            label.text = "Conflict Text";
            EditorGUILayout.PropertyField(conflictText, label);

            label.text = "Switch Button";
            EditorGUILayout.PropertyField(switchButton, label);

            label.text = "Cancel Button";
            EditorGUILayout.PropertyField(cancelButton, label);

            label.text = "Quit Button";
            EditorGUILayout.PropertyField(quitButton, label);

            label.text = "MW_Switch Toggle";
            EditorGUILayout.PropertyField(UI_MW_Switch, label);
        }

        label.text = "Templates";
        templates = EditorGUILayout.Foldout(templates, label);

        if (templates)
        {
            label.text = "Category Template";
            EditorGUILayout.PropertyField(categoryT, label);

            label.text = "Bind Template";
            EditorGUILayout.PropertyField(bindT, label);
        }

        label.text = "Menu Type";
        menuT = EditorGUILayout.Foldout(menuT, label);

        if (menuT)
        {
            menuType = (MenuType)menuTyp.enumValueIndex;

            menuType = (MenuType)EditorGUILayout.EnumPopup(label, menuType);

            menuTyp.enumValueIndex = (int)menuType;

            if (menuType == MenuType.MainMenu)
            {
                label.text = "Minimum Resolution X";
                minResolutionWidth.intValue = EditorGUILayout.IntField(label, minResolutionWidth.intValue);

                label.text = "Minimum Resolution Y";
                minResolutionHeight.intValue = EditorGUILayout.IntField(label, minResolutionHeight.intValue);

                label.text = "NewGame SceneName";
                newGameSceneName.stringValue = EditorGUILayout.TextField(label, newGameSceneName.stringValue);

                label.text = "NewGame Button";
                EditorGUILayout.PropertyField(newGameButton, label);

                label.text = "PlayerName Input";
                EditorGUILayout.PropertyField(UI_playerName, label);

                label.text = "Controls SaveFile Name";
                saveFileName.stringValue = EditorGUILayout.TextField(label, saveFileName.stringValue);

                label.text = "Binds";
                EditorGUILayout.PropertyField(binds, label, true);

                label.text = "Cancel Key";
                EditorGUILayout.PropertyField(cancelKey, label);

                label.text = "UnBindable Keys";
                EditorGUILayout.PropertyField(unBindableKeys, label, true);
            }

            else
            {
                label.text = "MainMenu SceneName";
                mainMenuSceneName.stringValue = EditorGUILayout.TextField(label, mainMenuSceneName.stringValue);

                label.text = "Contine Button";
                EditorGUILayout.PropertyField(continueButton, label);

                label.text = "Start Menu";
                EditorGUILayout.PropertyField(mainMenu, label);

                label.text = "Game UI";
                EditorGUILayout.PropertyField(GameUI, label);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
