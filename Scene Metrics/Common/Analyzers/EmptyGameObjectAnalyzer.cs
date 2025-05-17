using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace KenRampage.Reporting
{
    public class EmptyGameObjectAnalyzer : BaseSceneAnalyzer
    {
        public override string Category => "Empty GameObjects";
        
        public override bool ShouldAnalyze(PerformanceAnalyzerSettings settings)
        {
            return settings.AnalyzeEmptyGameObjects;
        }
        
        public override Dictionary<string, object> Analyze(List<GameObject> allObjects, Scene scene, PerformanceAnalyzerSettings settings)
        {
            Dictionary<string, object> metrics = new Dictionary<string, object>();
            int emptyObjectCount = 0;
            
            // Find objects with only a Transform component
            foreach (GameObject obj in allObjects)
            {
                if (obj == null) continue;
                
                if (obj.GetComponents<Component>().Length == 1) // Only has Transform
                {
                    emptyObjectCount++;
                }
            }
            
            metrics["Total Empty GameObjects"] = emptyObjectCount;
            
            // Removed detailed path listings
            
            return metrics;
        }
    }
}