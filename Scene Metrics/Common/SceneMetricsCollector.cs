using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System; // For StringComparison
using System.Collections.Generic;
using System.Linq;
using KenRampage.Reporting;

namespace KenRampage.Reporting
{
    /// <summary>
    /// Handles the collection of performance metrics in Unity scenes using modular analyzers.
    /// </summary>
    public static class SceneMetricsCollector
    {
        /// <summary>
        /// Generates a descriptive name for a scene from its full path.
        /// Example: "Assets/Levels/MyScene.unity" becomes "Levels/MyScene".
        /// </summary>
        /// <param name="fullPath">The full asset path to the scene file.</param>
        /// <returns>A descriptive scene name.</returns>
        public static string GetDescriptiveSceneNameFromPath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath)) return "UnknownScene";
            string relativePath = fullPath.Replace("\\", "/"); // Normalize to forward slashes

            const string assetsPrefix = "Assets/";
            if (relativePath.StartsWith(assetsPrefix, StringComparison.OrdinalIgnoreCase))
            {
                relativePath = relativePath.Substring(assetsPrefix.Length);
            }

            const string unitySuffix = ".unity";
            if (relativePath.EndsWith(unitySuffix, StringComparison.OrdinalIgnoreCase))
            {
                relativePath = relativePath.Substring(0, relativePath.Length - unitySuffix.Length);
            }
            return relativePath;
        }

        /// <summary>
        /// Analyzes all performance metrics in a scene based on the provided settings.
        /// </summary>
        /// <param name="scene">The scene to analyze</param>
        /// <param name="settings">Settings controlling which metrics to analyze</param>
        /// <returns>Dictionary of metric categories and their collected values</returns>
        public static Dictionary<string, Dictionary<string, object>> AnalyzeScenePerformance(Scene scene, PerformanceAnalyzerSettings settings)
        {
            Dictionary<string, Dictionary<string, object>> metrics = new Dictionary<string, Dictionary<string, object>>();
            
            if (!scene.IsValid())
            {
                Debug.LogError("Invalid scene provided for analysis");
                return metrics;
            }
            
            GameObject[] rootObjects = scene.GetRootGameObjects();
            List<GameObject> allObjects = new List<GameObject>();
            
            // Collect all GameObjects in the scene including inactive ones
            foreach (GameObject rootObject in rootObjects)
            {
                if (rootObject == null) continue;
                
                allObjects.Add(rootObject);
                var childTransforms = rootObject.GetComponentsInChildren<Transform>(true);
                if (childTransforms != null)
                {
                    allObjects.AddRange(childTransforms
                        .Where(t => t != null)
                        .Select(t => t.gameObject)
                        .Where(go => go != null));
                }
            }
            
            // Remove duplicates (if any)
            allObjects = allObjects.Where(obj => obj != null).Distinct().ToList();
            
            // Use the modular analyzers from the registry
            foreach (var analyzer in AnalyzerRegistry.Analyzers)
            {
                try
                {
                    if (analyzer.ShouldAnalyze(settings))
                    {
                        var categoryMetrics = analyzer.Analyze(allObjects, scene, settings);
                        if (categoryMetrics != null && categoryMetrics.Count > 0)
                        {
                            metrics[analyzer.Category] = categoryMetrics;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error in {analyzer.Category} analyzer: {ex.Message}\n{ex.StackTrace}");
                }
            }
            
            return metrics;
        }
        
        /// <summary>
        /// Gets the full path of a GameObject in the hierarchy.
        /// Maintained for backward compatibility.
        /// </summary>
        public static string GetGameObjectPath(GameObject obj)
        {
            string path = obj.name;
            Transform parent = obj.transform.parent;
            
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            
            return path;
        }
    }
}