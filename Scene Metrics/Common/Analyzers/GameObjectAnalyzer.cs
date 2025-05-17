using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace KenRampage.Reporting
{
    public class GameObjectAnalyzer : BaseSceneAnalyzer
    {
        public override string Category => "GameObject Counts";
        
        public override bool ShouldAnalyze(PerformanceAnalyzerSettings settings)
        {
            return settings.AnalyzeGameObjectCounts;
        }
        
        public override Dictionary<string, object> Analyze(List<GameObject> allObjects, Scene scene, PerformanceAnalyzerSettings settings)
        {
            Dictionary<string, object> metrics = new Dictionary<string, object>();
            
            // Filter out nulls for safety
            var validObjects = allObjects.Where(obj => obj != null).ToList();
            
            int totalGameObjects = validObjects.Count;
            int activeGameObjects = validObjects.Count(go => go.activeInHierarchy);
            int inactiveGameObjects = totalGameObjects - activeGameObjects;
            
            metrics["Total GameObjects"] = totalGameObjects;
            metrics["Active GameObjects"] = activeGameObjects;
            metrics["Inactive GameObjects"] = inactiveGameObjects;
            
            return metrics;
        }
    }
}