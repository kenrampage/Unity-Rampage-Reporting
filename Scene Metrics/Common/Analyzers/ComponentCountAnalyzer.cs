using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace KenRampage.Reporting
{
    /// <summary>
    /// Analyzer that counts all components in a scene by type
    /// </summary>
    public class ComponentCountAnalyzer : BaseSceneAnalyzer
    {
        public override string Category => "Components";
        
        public override bool ShouldAnalyze(PerformanceAnalyzerSettings settings)
        {
            return settings.AnalyzeComponentDistribution;
        }
        
        public override Dictionary<string, object> Analyze(List<GameObject> allObjects, Scene scene, PerformanceAnalyzerSettings settings)
        {
            Dictionary<string, object> metrics = new Dictionary<string, object>();
            Dictionary<System.Type, int> componentCounts = new Dictionary<System.Type, int>();
            
            foreach (GameObject obj in allObjects)
            {
                if (obj == null) continue;
                
                Component[] components = obj.GetComponents<Component>();
                
                foreach (Component component in components)
                {
                    // Handle potential Missing Script components
                    if (component == null)
                    {
                        System.Type missingType = typeof(UnityEngine.Object); // Use a generic placeholder
                        if (componentCounts.ContainsKey(missingType))
                            componentCounts[missingType]++;
                        else
                            componentCounts.Add(missingType, 1);
                        continue;
                    }

                    System.Type componentType = component.GetType();

                    // Increment count for this component type
                    if (componentCounts.ContainsKey(componentType))
                        componentCounts[componentType]++;
                    else
                        componentCounts.Add(componentType, 1);
                }
            }
            
            // Convert the component counts to the metrics format
            // We'll use the full type name as the metric name
            foreach (var kvp in componentCounts.OrderBy(kvp => kvp.Key.FullName))
            {
                string componentName = (kvp.Key == typeof(UnityEngine.Object)) 
                    ? "Missing Script" 
                    : kvp.Key.FullName;
                
                metrics[componentName] = kvp.Value;
            }
            
            return metrics;
        }
    }
}