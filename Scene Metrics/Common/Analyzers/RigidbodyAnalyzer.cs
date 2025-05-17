using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace KenRampage.Reporting
{
    public class RigidbodyAnalyzer : BaseSceneAnalyzer
    {
        public override string Category => "Rigidbodies";
        
        public override bool ShouldAnalyze(PerformanceAnalyzerSettings settings)
        {
            return settings.AnalyzeRigidbodies;
        }
        
        public override Dictionary<string, object> Analyze(List<GameObject> allObjects, Scene scene, PerformanceAnalyzerSettings settings)
        {
            Dictionary<string, object> metrics = new Dictionary<string, object>();
            int totalRigidbodies = 0;
            int kinematicRigidbodies = 0;
            int dynamicRigidbodies = 0;
            
            foreach (GameObject obj in allObjects)
            {
                if (obj == null) continue;
                
                foreach (Rigidbody rb in GetComponentsFromGameObject<Rigidbody>(obj))
                {
                    totalRigidbodies++;
                    
                    if (rb.isKinematic)
                        kinematicRigidbodies++;
                    else
                        dynamicRigidbodies++;
                }
            }
            
            metrics["Total Rigidbodies"] = totalRigidbodies;
            metrics["Kinematic Rigidbodies"] = kinematicRigidbodies;
            metrics["Dynamic Rigidbodies"] = dynamicRigidbodies;
            
            return metrics;
        }
    }
}