using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using KenRampage.Reporting;

namespace KenRampage.Reporting.Editor
{
    /// <summary>
    /// Window for configuring performance analysis settings.
    /// </summary>
    public class PerformanceAnalyzerSettingsWindow : EditorWindow
    {
        private PerformanceAnalyzerSettings _settings;
        private Vector2 _scrollPosition;
        private bool _showHierarchySettings = true;
        private bool _showRenderingSettings = true;
        private bool _showPhysicsSettings = true;
        private bool _showAssetSettings = true;
        private bool _showComponentSettings = true;
        private bool _showThresholdSettings = true;

        public delegate void SettingsConfirmedHandler(PerformanceAnalyzerSettings settings);
        public event SettingsConfirmedHandler OnSettingsConfirmed;

        /// <summary>
        /// Initializes the window with title and callback.
        /// </summary>
        /// <param name="title">The window title.</param>
        /// <param name="callback">Callback to invoke when settings are confirmed.</param>
        public void Initialize(string title, SettingsConfirmedHandler callback)
        {
            this.titleContent = new GUIContent(title);
            OnSettingsConfirmed = callback;
            _settings = PerformanceAnalyzerSettings.GetOrCreateSettings();
            this.minSize = new Vector2(400, 500);
        }

        private void OnGUI()
        {
            GUILayout.Label("Performance Analysis Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // Quick buttons for enabling/disabling all
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Enable All", GUILayout.Width(120)))
            {
                SetAllSettings(true);
            }
            if (GUILayout.Button("Disable All", GUILayout.Width(120)))
            {
                SetAllSettings(false);
            }
            if (GUILayout.Button("Reset to Defaults", GUILayout.Width(120)))
            {
                _settings = ScriptableObject.CreateInstance<PerformanceAnalyzerSettings>();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();

            // Hierarchy metrics
            _showHierarchySettings = EditorGUILayout.Foldout(_showHierarchySettings, "GameObject Hierarchy Analysis", true, EditorStyles.foldoutHeader);
            if (_showHierarchySettings)
            {
                EditorGUI.indentLevel++;
                _settings.AnalyzeGameObjectCounts = EditorGUILayout.ToggleLeft("Count GameObjects", _settings.AnalyzeGameObjectCounts);
                _settings.AnalyzeHierarchyDepth = EditorGUILayout.ToggleLeft("Analyze Hierarchy Depth", _settings.AnalyzeHierarchyDepth);
                _settings.AnalyzeEmptyGameObjects = EditorGUILayout.ToggleLeft("Find Empty GameObjects", _settings.AnalyzeEmptyGameObjects);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();

            // Rendering metrics
            _showRenderingSettings = EditorGUILayout.Foldout(_showRenderingSettings, "Rendering Analysis", true, EditorStyles.foldoutHeader);
            if (_showRenderingSettings)
            {
                EditorGUI.indentLevel++;
                _settings.AnalyzeRenderers = EditorGUILayout.ToggleLeft("Analyze Renderers", _settings.AnalyzeRenderers);
                _settings.AnalyzeMaterials = EditorGUILayout.ToggleLeft("Analyze Materials", _settings.AnalyzeMaterials);
                _settings.AnalyzeShaders = EditorGUILayout.ToggleLeft("Analyze Shaders", _settings.AnalyzeShaders);
                _settings.AnalyzeTextures = EditorGUILayout.ToggleLeft("Analyze Textures", _settings.AnalyzeTextures);
                _settings.AnalyzeCameras = EditorGUILayout.ToggleLeft("Analyze Cameras", _settings.AnalyzeCameras);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();

            // Physics metrics
            _showPhysicsSettings = EditorGUILayout.Foldout(_showPhysicsSettings, "Physics Analysis", true, EditorStyles.foldoutHeader);
            if (_showPhysicsSettings)
            {
                EditorGUI.indentLevel++;
                _settings.AnalyzeColliders = EditorGUILayout.ToggleLeft("Analyze Colliders", _settings.AnalyzeColliders);
                _settings.AnalyzeRigidbodies = EditorGUILayout.ToggleLeft("Analyze Rigidbodies", _settings.AnalyzeRigidbodies);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();

            // Asset references
            _showAssetSettings = EditorGUILayout.Foldout(_showAssetSettings, "Asset References Analysis", true, EditorStyles.foldoutHeader);
            if (_showAssetSettings)
            {
                EditorGUI.indentLevel++;
                _settings.AnalyzePrefabInstances = EditorGUILayout.ToggleLeft("Analyze Prefab Instances", _settings.AnalyzePrefabInstances);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();

            // Performance-critical components
            _showComponentSettings = EditorGUILayout.Foldout(_showComponentSettings, "Performance-Critical Components", true, EditorStyles.foldoutHeader);
            if (_showComponentSettings)
            {
                EditorGUI.indentLevel++;
                _settings.AnalyzeParticleSystems = EditorGUILayout.ToggleLeft("Analyze Particle Systems", _settings.AnalyzeParticleSystems);
                _settings.AnalyzeLights = EditorGUILayout.ToggleLeft("Analyze Lights", _settings.AnalyzeLights);
                _settings.AnalyzeReflectionProbes = EditorGUILayout.ToggleLeft("Analyze Reflection Probes", _settings.AnalyzeReflectionProbes);
                _settings.AnalyzeAudioSources = EditorGUILayout.ToggleLeft("Analyze Audio Sources", _settings.AnalyzeAudioSources);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();

            // Threshold settings
            _showThresholdSettings = EditorGUILayout.Foldout(_showThresholdSettings, "Analysis Thresholds", true, EditorStyles.foldoutHeader);
            if (_showThresholdSettings)
            {
                EditorGUI.indentLevel++;
                _settings.MaxRecommendedHierarchyDepth = EditorGUILayout.IntSlider("Max Hierarchy Depth", _settings.MaxRecommendedHierarchyDepth, 3, 20);
                _settings.MaxRecommendedTextureSize = EditorGUILayout.IntField("Max Texture Size", _settings.MaxRecommendedTextureSize);
                _settings.MaxRecommendedPolygonCount = EditorGUILayout.IntField("Max Polygon Count", _settings.MaxRecommendedPolygonCount);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.Space();

            // Confirm and cancel buttons
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel", GUILayout.Width(100)))
            {
                this.Close();
            }
            if (GUILayout.Button("Confirm", GUILayout.Width(100)))
            {
                OnSettingsConfirmed?.Invoke(_settings);
                this.Close();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void SetAllSettings(bool enabled)
        {
            _settings.AnalyzeGameObjectCounts = enabled;
            _settings.AnalyzeHierarchyDepth = enabled;
            _settings.AnalyzeEmptyGameObjects = enabled;
            _settings.AnalyzeRenderers = enabled;
            _settings.AnalyzeMaterials = enabled;
            _settings.AnalyzeShaders = enabled;
            _settings.AnalyzeTextures = enabled;
            _settings.AnalyzeCameras = enabled;
            _settings.AnalyzeColliders = enabled;
            _settings.AnalyzeRigidbodies = enabled;
            _settings.AnalyzePrefabInstances = enabled;
            _settings.AnalyzeParticleSystems = enabled;
            _settings.AnalyzeLights = enabled;
            _settings.AnalyzeReflectionProbes = enabled;
            _settings.AnalyzeAudioSources = enabled;
            _settings.AnalyzeComponentDistribution = enabled;
        }

        public static void ShowWindow()
        {
            var window = GetWindow<PerformanceAnalyzerSettingsWindow>(false, "Performance Analysis Settings");
            window.Initialize("Performance Analysis Settings", null);
            window.Show();
        }
    }

    /// <summary>
    /// Window for selecting multiple scenes to analyze.
    /// </summary>
    public class MultiSelectionWindow : EditorWindow
    {
        private string[] _displayNames;
        private string[] _values;
        private bool[] _selections;
        private string _windowTitle;
        private Vector2 _scrollPosition;
        private string _searchFilter = "";
        private bool _anyMatching = true;

        public delegate void SelectionConfirmedHandler(string[] selectedValues);
        public event SelectionConfirmedHandler OnSelectionConfirmed;

        /// <summary>
        /// Initializes the window with display names and values.
        /// </summary>
        /// <param name="title">The window title.</param>
        /// <param name="displayNames">Names to display in the UI.</param>
        /// <param name="values">Actual values to return when selected.</param>
        public void Initialize(string title, string[] displayNames, string[] values)
        {
            _windowTitle = title;
            _displayNames = displayNames;
            _values = values;
            _selections = new bool[displayNames.Length];
            this.titleContent = new GUIContent(_windowTitle);
            this.minSize = new Vector2(400, 500);
        }

        private void OnGUI()
        {
            GUILayout.Label(_windowTitle, EditorStyles.boldLabel);
            
            // Search bar
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Filter:", GUILayout.Width(40));
            string newFilter = EditorGUILayout.TextField(_searchFilter);
            if (newFilter != _searchFilter)
            {
                _searchFilter = newFilter;
                _anyMatching = false;
                for (int i = 0; i < _displayNames.Length; i++)
                {
                    if (string.IsNullOrEmpty(_searchFilter) || 
                        _displayNames[i].IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        _anyMatching = true;
                        break;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            
            // Quick selection buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select All"))
            {
                for (int i = 0; i < _selections.Length; i++)
                {
                    if (string.IsNullOrEmpty(_searchFilter) || 
                        _displayNames[i].IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        _selections[i] = true;
                    }
                }
            }
            if (GUILayout.Button("Select None"))
            {
                for (int i = 0; i < _selections.Length; i++)
                    _selections[i] = false;
            }
            if (GUILayout.Button("Invert Selection"))
            {
                for (int i = 0; i < _selections.Length; i++)
                {
                    if (string.IsNullOrEmpty(_searchFilter) || 
                        _displayNames[i].IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        _selections[i] = !_selections[i];
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            
            // Show warning if no items match the filter
            if (!string.IsNullOrEmpty(_searchFilter) && !_anyMatching)
            {
                EditorGUILayout.HelpBox("No items match the current filter.", MessageType.Info);
            }
            
            // Checkboxes for each scene
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            for (int i = 0; i < _displayNames.Length; i++)
            {
                // Apply filter
                if (!string.IsNullOrEmpty(_searchFilter) && 
                    _displayNames[i].IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }
                
                _selections[i] = EditorGUILayout.ToggleLeft(_displayNames[i], _selections[i]);
            }
            EditorGUILayout.EndScrollView();
            
            // Show selection count
            int selectedCount = 0;
            for (int i = 0; i < _selections.Length; i++)
                if (_selections[i]) selectedCount++;
            
            EditorGUILayout.LabelField($"Selected: {selectedCount} / {_displayNames.Length}");
            
            // Confirm and cancel buttons
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel", GUILayout.Width(100)))
            {
                this.Close();
            }
            
            EditorGUI.BeginDisabledGroup(selectedCount == 0);
            if (GUILayout.Button("Confirm", GUILayout.Width(100)))
            {
                List<string> selectedValues = new List<string>();
                for (int i = 0; i < _selections.Length; i++)
                {
                    if (_selections[i])
                        selectedValues.Add(_values[i]);
                }
                
                OnSelectionConfirmed?.Invoke(selectedValues.ToArray());
                this.Close();
            }
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.EndHorizontal();
        }
    }
}