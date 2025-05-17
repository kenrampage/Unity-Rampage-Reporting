using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace KenRampage.Reporting
{
    public class ColliderAnalyzer : BaseSceneAnalyzer
    {
        public override string Category => "Colliders";
        
        public override bool ShouldAnalyze(PerformanceAnalyzerSettings settings)
        {
            return settings.AnalyzeColliders;
        }
        
        public override Dictionary<string, object> Analyze(List<GameObject> allObjects, Scene scene, PerformanceAnalyzerSettings settings)
        {
            Dictionary<string, object> metrics = new Dictionary<string, object>();
            Dictionary<string, int> colliderTypes = new Dictionary<string, int>();
            int totalColliders = 0;
            
            foreach (GameObject obj in allObjects)
            {
                if (obj == null) continue;
                
                foreach (Collider collider in GetComponentsFromGameObject<Collider>(obj))
                {
                    totalColliders++;
                    
                    string colliderType = collider.GetType().Name;
                    if (!colliderTypes.ContainsKey(colliderType))
                        colliderTypes[colliderType] = 0;
                    colliderTypes[colliderType]++;
                }
            }
            
            metrics["Total Colliders"] = totalColliders;
            
            foreach (var kvp in colliderTypes)
            {
                metrics[$"{kvp.Key} Count"] = kvp.Value;
            }
            
            return metrics;
        }
    }
}