using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Description: Intercepts the creation of new C# scripts to populate organizational placeholders.
/// Coordination: Matches placeholders in MonoBehaviourScript.cs.txt.
/// Deployment: This must stay in the Assets/Editor folder.
/// To adjust your personal settings in Editor go to Edit -> Project Settings -> Scripts Template Settings
/// </summary>
public class TemplateProcessor : AssetModificationProcessor
{
#region Core
    private const string ProjectPlaceholder = "[Project Name]";
    private const string AuthorPlaceholder  = "[Author]";
    private const string EmailPlaceholder   = "[Email]";
    private const string DatePlaceholder    = "#DATE#";

    private const string PrefsProjectKey = "TemplateProcessor_ProjectName";
    private const string PrefsAuthorKey  = "TemplateProcessor_AuthorName"; 
    private const string PrefsEmailKey   = "TemplateProcessor_AuthorEmail";

    /// <summary>
    /// Unity calls this method whenever a new asset is created.
    /// </summary>
    public static void OnWillCreateAsset(string assetPath)
    {
        assetPath = assetPath.Replace(".meta", "");
        if (!assetPath.EndsWith(".cs")) return;
        string fullPath = Path.GetFullPath(assetPath);
        if (!File.Exists(fullPath)) return;

        try
        {
            string content = File.ReadAllText(fullPath);

            //Add Settings
            string orgProjectName = EditorPrefs.GetString(PrefsProjectKey, "MyProjectName");
            string orgAuthorName = EditorPrefs.GetString(PrefsAuthorKey, "DeveloperName");
            string orgAuthorEmail = EditorPrefs.GetString(PrefsEmailKey, "developer@domain.com");

            content = content.Replace(ProjectPlaceholder, orgProjectName);
            content = content.Replace(AuthorPlaceholder, orgAuthorName);
            content = content.Replace(EmailPlaceholder, orgAuthorEmail);
            content = content.Replace(DatePlaceholder, DateTime.Now.ToString("yyyy-MM-dd"));

            File.WriteAllText(fullPath, content);
            
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Debug.LogError($"[Wasted Resources] Automation failed for {assetPath}: {e.Message}");
        }
    }
#endregion


#region Settings
    [SettingsProvider]
    public static SettingsProvider CreateTemplateSettingsProvider()
    {
        var provider = new SettingsProvider("Project/Scripts Template Settings", SettingsScope.Project) ;
        {
            provider.guiHandler = (searchContext) =>
            {
                string currentProject = EditorPrefs.GetString(PrefsProjectKey, "MyProjectName");
                string currentAuthor = EditorPrefs.GetString(PrefsAuthorKey, "DeveloperName");
                string currentEmail = EditorPrefs.GetString(PrefsEmailKey, "developer@domain.com");

                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Configure specifications for newly generated Script templates", MessageType.Info);
                GUILayout.Space(5);

                EditorGUI.BeginChangeCheck();

                string newProject = EditorGUILayout.TextField("Project Name", currentProject);
                string newAuthor = EditorGUILayout.TextField("Developer Name", currentAuthor);
                string newEmail = EditorGUILayout.TextField("developer@domain.com", currentEmail);

                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetString(PrefsProjectKey, newProject);
                    EditorPrefs.SetString(PrefsAuthorKey, newAuthor);
                    EditorPrefs. SetString(PrefsEmailKey, newEmail);
                }
            };
            return provider ;
        }

    }
#endregion


#region Templates
    [MenuItem("Assets/Create/New Mono Script", false, 1)]
    public static void CreateScript()
    {
        string templatePath = "Assets/Editor/Templates/MonoBehaviourScript.cs.txt" ;
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewMonoScript.cs");
    }
#endregion
}