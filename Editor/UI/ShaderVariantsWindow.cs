using System;
using System.IO;
using System.Linq;
using Unity.ProjectAuditor.Editor.Auditors;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unity.ProjectAuditor.Editor.UI
{
    class ShaderVariantsWindow : AnalysisWindow, IProjectIssueFilter
    {
        const string k_BuildRequiredInfo = @"
Build the project to view the Shader Variants
";

        const string k_PlayerLogInfo = @"
To find which shader variants are compiled at runtime, follow these steps:
- Enable the Log Shader Compilation option (Project Settings => Graphics => Shader Loading)
- Make a Development build
- Run the build on the target platform
- Drag & Drop the Player.log file on this window
";
        const string k_NotLogFile = "Player log file not recognized.";
        const string k_NoCompiledVariantWarning = "No compiled shader variants found in player log. Perhaps, Log Shader Compilation was not enabled when the project was built.";
        const string k_NoCompiledVariantWarningLogDisabled = "No compiled shader variants found in player log. Shader compilation logging is disabled. Would you like to enable it? (Shader compilation will not appear in the log until the project is rebuilt)";
        const string k_PlayerLogProcessed = "Player log file successfully processed.";

        bool m_FlatView;
        bool m_HideCompiledVariants;
        IProjectIssueFilter m_MainFilter;
        ShadersAuditor m_ShadersAuditor;

        public void SetShadersAuditor(ShadersAuditor shadersAuditor)
        {
            m_ShadersAuditor = shadersAuditor;
        }

        void ParsePlayerLog(string logFilename)
        {
            if (string.IsNullOrEmpty(logFilename))
                return;

            var variants = m_Issues.Where(i => i.category == IssueCategory.ShaderVariants).ToArray();

            if (m_ShadersAuditor.ParsePlayerLog(logFilename, variants, new ProgressBarDisplay()))
            {
                EditorUtility.DisplayDialog("Shader Variants", k_PlayerLogProcessed, "Ok");
                m_AnalysisView.Refresh();
            }
            else
            {
                if (GraphicsSettings.logWhenShaderIsCompiled)
                {
                    EditorUtility.DisplayDialog("Shader Variants", k_NoCompiledVariantWarning, "Ok");
                }
                else
                {
                    GraphicsSettings.logWhenShaderIsCompiled = EditorUtility.DisplayDialog("Shader Variants", k_NoCompiledVariantWarningLogDisabled, "Yes", "No");
                }
            }
        }

        public override void CreateTable(AnalysisViewDescriptor desc, ProjectAuditorConfig config, Preferences prefs, IProjectIssueFilter filter)
        {
            m_MainFilter = filter;
            base.CreateTable(desc, config, prefs, this);
            m_AnalysisView.SetFlatView(m_FlatView);
        }

        public override void OnGUI()
        {
            var buildAvailable = ShadersAuditor.BuildDataAvailable();

            EditorGUILayout.BeginVertical(GUI.skin.box);

            var helpStyle = new GUIStyle(EditorStyles.textField);
            helpStyle.wordWrap = true;
            EditorGUILayout.LabelField(buildAvailable ? k_PlayerLogInfo : k_BuildRequiredInfo, helpStyle);

            EditorGUI.BeginChangeCheck();

            var lastEnabled = GUI.enabled;
            GUI.enabled = buildAvailable;
            m_FlatView = EditorGUILayout.ToggleLeft("Flat View", m_FlatView, GUILayout.Width(160));
            m_HideCompiledVariants = EditorGUILayout.ToggleLeft("Hide Compiled Variants", m_HideCompiledVariants, GUILayout.Width(160));
            GUI.enabled = lastEnabled;

            if (EditorGUI.EndChangeCheck())
            {
                m_AnalysisView.SetFlatView(m_FlatView);
                m_AnalysisView.Refresh();
            }

            EditorGUILayout.EndVertical();

            if (buildAvailable)
            {
                var evt = Event.current;

                switch (evt.type)
                {
                    case EventType.DragExited:
                        break;
                    case EventType.DragUpdated:
                        var valid = 1 == DragAndDrop.paths.Count(path => Path.HasExtension(path) && Path.GetExtension(path).Equals(".log"));
                        DragAndDrop.visualMode = valid ? DragAndDropVisualMode.Generic : DragAndDropVisualMode.Rejected;
                        evt.Use();
                        break;
                    case EventType.DragPerform:
                        DragAndDrop.AcceptDrag();
                        HandleDragAndDrop();
                        evt.Use();
                        break;
                }
                base.OnGUI();
            }
        }

        void HandleDragAndDrop()
        {
            var paths = DragAndDrop.paths;
            foreach (var path in paths)
            {
                ParsePlayerLog(path);
            }
        }

        public bool Match(ProjectIssue issue)
        {
            if (!m_MainFilter.Match(issue))
                return false;
            if (m_HideCompiledVariants)
                return !issue.GetCustomPropertyAsBool((int)ShaderVariantProperty.Compiled);
            return true;
        }
    }
}
