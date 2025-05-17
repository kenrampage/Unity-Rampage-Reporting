using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using KenRampage.Reporting;

namespace KenRampage.Reporting.Editor
{
    /// <summary>
    /// Main manager class for the Performance Analyzer system that orchestrates
    /// the analysis process and handles exporting results.
    /// </summary>
    public static class PerformanceAnalyzerManager
    {
        #region Menu Items

        /// <summary>
        /// Menu item to analyze performance metrics across scenes.
        /// </summary>
        [MenuItem("Tools/Ken Rampage/Scene Metrics/Performance Metrics", false, 200)]
        private static void AnalyzePerformanceMetrics()
        {
            // Get all scene asset paths in the project
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
            
            if (sceneGuids.Length == 0)
            {
                Debug.LogError("No scenes found in the project.");
                return;
            }
            
            // Convert GUIDs to file paths and display names
            List<string> scenePaths = new List<string>();
            List<string> sceneNames = new List<string>();
            
            foreach (string guid in sceneGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                scenePaths.Add(path);
                sceneNames.Add(SceneMetricsCollector.GetDescriptiveSceneNameFromPath(path));
            }
            
            // First show settings window
            PerformanceAnalyzerSettingsWindow settingsWindow = ScriptableObject.CreateInstance<PerformanceAnalyzerSettingsWindow>();
            settingsWindow.Initialize("Performance Analysis Settings", OnAnalysisSettingsConfirmed);
            settingsWindow.ShowUtility();
            
            void OnAnalysisSettingsConfirmed(PerformanceAnalyzerSettings settings)
            {
                // Then show scene selection window
                MultiSelectionWindow window = ScriptableObject.CreateInstance<MultiSelectionWindow>();
                window.Initialize("Select Scenes to Analyze", sceneNames.ToArray(), scenePaths.ToArray());
                window.OnSelectionConfirmed += (selectedScenePaths) => {
                    EditorApplication.delayCall += () => ProcessMultipleScenes(selectedScenePaths, settings);
                };
                window.ShowUtility();
            }
        }

        /// <summary>
        /// Menu item to analyze component counts only
        /// </summary>
        [MenuItem("Tools/Ken Rampage/Scene Metrics/Component Counts", false, 100)]
        private static void CountComponents()
        {
            // Create settings that only enable component analysis
            PerformanceAnalyzerSettings settings = ScriptableObject.CreateInstance<PerformanceAnalyzerSettings>();
            
            // Disable everything except component counting
            SetAllSettings(settings, false);
            settings.AnalyzeComponentDistribution = true;
            
            // Launch the scene selection
            SelectScenesAndAnalyze(settings, "ComponentCounts");
        }

        /// <summary>
        /// Helper to enable/disable all settings
        /// </summary>
        private static void SetAllSettings(PerformanceAnalyzerSettings settings, bool enabled)
        {
            // This would set all boolean properties to the specified value
            // Corrected to use GetFields for public fields
            foreach (var field in typeof(PerformanceAnalyzerSettings).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                if (field.FieldType == typeof(bool))
                {
                    field.SetValue(settings, enabled);
                }
            }
        }

        /// <summary>
        /// Common method to select scenes and start analysis
        /// </summary>
        private static void SelectScenesAndAnalyze(PerformanceAnalyzerSettings settings, string filePrefix)
        {
            // Get all scene asset paths in the project
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
            
            if (sceneGuids.Length == 0)
            {
                Debug.LogError("No scenes found in the project.");
                return;
            }
            
            // Convert GUIDs to file paths and display names
            List<string> scenePaths = new List<string>();
            List<string> sceneNames = new List<string>();
            
            foreach (string guid in sceneGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                scenePaths.Add(path);
                sceneNames.Add(SceneMetricsCollector.GetDescriptiveSceneNameFromPath(path));
            }
            
            // Show scene selection window
            MultiSelectionWindow window = ScriptableObject.CreateInstance<MultiSelectionWindow>();
            window.Initialize("Select Scenes to Analyze", sceneNames.ToArray(), scenePaths.ToArray());
            window.OnSelectionConfirmed += (selectedScenePaths) => {
                EditorApplication.delayCall += () => ProcessMultipleScenes(selectedScenePaths, settings, filePrefix);
            };
            window.ShowUtility();
        }

        #endregion

        #region Core Analysis Functions

        /// <summary>
        /// Processes multiple scenes and generates combined reports.
        /// </summary>
        /// <param name="selectedScenePaths">Array of scene paths to analyze</param>
        /// <param name="settings">Analysis settings controlling which metrics to collect</param>
        private static void ProcessMultipleScenes(string[] selectedScenePaths, PerformanceAnalyzerSettings settings, string filePrefix = "PerformanceMetrics")
        {
            if (selectedScenePaths == null || selectedScenePaths.Length == 0)
            {
                Debug.LogError("No scenes selected for processing.");
                return;
            }

            // Store the currently open scene to restore it later
            Scene currentScene = SceneManager.GetActiveScene();
            string currentScenePath = currentScene.path;
            
            Dictionary<string, Dictionary<string, Dictionary<string, object>>> allSceneMetrics = 
                new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

            try
            {
                // Process each selected scene
                for (int i = 0; i < selectedScenePaths.Length; i++)
                {
                    string scenePath = selectedScenePaths[i];
                    string sceneName = SceneMetricsCollector.GetDescriptiveSceneNameFromPath(scenePath);
                    
                    EditorUtility.DisplayProgressBar("Processing Scenes", 
                        $"Analyzing scene {i+1}/{selectedScenePaths.Length}: {sceneName}", 
                        (float)i / selectedScenePaths.Length);
                    
                    Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                    
                    Debug.Log($"Analyzing performance metrics in scene: {sceneName}");
                    Dictionary<string, Dictionary<string, object>> sceneMetrics = 
                        SceneMetricsCollector.AnalyzeScenePerformance(scene, settings);
                    allSceneMetrics.Add(sceneName, sceneMetrics);
                }
                
                // Export the combined data
                ExportMetricsToCsv(allSceneMetrics, filePrefix);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error processing scenes: {ex.Message}");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                
                // Reopen the original scene
                if (!string.IsNullOrEmpty(currentScenePath))
                {
                    EditorSceneManager.OpenScene(currentScenePath);
                }
            }
        }

        /// <summary>
        /// Exports metrics data to CSV format for review and comparison.
        /// </summary>
        /// <param name="allSceneMetrics">Dictionary of scene metrics to export</param>
        /// <param name="fileNamePrefix">Prefix for the export file name</param>
        private static void ExportMetricsToCsv(
            Dictionary<string, Dictionary<string, Dictionary<string, object>>> allSceneMetrics, 
            string fileNamePrefix)
        {
            if (allSceneMetrics == null || allSceneMetrics.Count == 0)
            {
                Debug.LogError("No metrics data to export.");
                return;
            }

            // Generate a suggested filename with date
            string dateSuffix = System.DateTime.Now.ToString("yyyyMMdd");
            string suggestedFileName = $"{fileNamePrefix}_{dateSuffix}.csv";

            // Show "Save As" dialog
            string filePath = EditorUtility.SaveFilePanel(
                "Save Performance Metrics CSV", // Dialog title
                "",                             // Default directory (user will choose)
                suggestedFileName,              // Suggested filename
                "csv"                           // File extension
            );

            // Check if the user cancelled the dialog
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.Log("CSV export cancelled by user.");
                return;
            }

            // Create CSV Content
            StringBuilder sb = new StringBuilder();
            
            // Get all scene names (for columns)
            List<string> sceneNames = allSceneMetrics.Keys.ToList();
            
            // Get all unique metric categories and names across all scenes
            Dictionary<string, HashSet<string>> allMetricsByCategory = new Dictionary<string, HashSet<string>>();
            
            foreach (var sceneData in allSceneMetrics.Values)
            {
                foreach (var categoryKvp in sceneData)
                {
                    string category = categoryKvp.Key;
                    
                    if (!allMetricsByCategory.ContainsKey(category))
                        allMetricsByCategory[category] = new HashSet<string>();
                    
                    foreach (var metricKvp in categoryKvp.Value)
                    {
                        allMetricsByCategory[category].Add(metricKvp.Key);
                    }
                }
            }
            
            // Create the header row with scene names
            sb.Append("Category,Metric");
            foreach (string sceneName in sceneNames)
            {
                sb.Append($",{sceneName}");
            }
            sb.AppendLine();
            
            // Sort categories for consistent output
            List<string> sortedCategories = allMetricsByCategory.Keys.OrderBy(c => c).ToList();
            
            // Add rows for each metric
            foreach (string category in sortedCategories)
            {
                // Sort metric names within category
                List<string> sortedMetricNames = allMetricsByCategory[category].OrderBy(m => m).ToList();
                
                foreach (string metricName in sortedMetricNames)
                {
                    sb.Append($"\"{category}\",\"{metricName}\"");
                    
                    // Add the metric value for each scene
                    foreach (string sceneName in sceneNames)
                    {
                        string value = "N/A";
                        
                        if (allSceneMetrics[sceneName].ContainsKey(category) && 
                            allSceneMetrics[sceneName][category].ContainsKey(metricName))
                        {
                            object rawValue = allSceneMetrics[sceneName][category][metricName];
                            if (rawValue != null)
                            {
                                value = rawValue.ToString();
                            }
                        }
                        
                        sb.Append($",\"{value}\"");
                    }
                    
                    sb.AppendLine();
                }
            }

            // Write to File
            try
            {
                File.WriteAllText(filePath, sb.ToString());
                Debug.Log($"Successfully exported performance metrics to: {filePath}");
                EditorUtility.RevealInFinder(filePath);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to write CSV file at {filePath}. Error: {ex.Message}");
            }
        }

        #endregion
    }
}