using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace KenRampage.Reporting
{
    public class ReflectionProbeAnalyzer : BaseSceneAnalyzer
    {
        public override string Category => "Reflection Probes";
        
        public override bool ShouldAnalyze(PerformanceAnalyzerSettings settings)
        {
            return settings.AnalyzeReflectionProbes;
        }
        
        public override Dictionary<string, object> Analyze(List<GameObject> allObjects, Scene scene, PerformanceAnalyzerSettings settings)
        {
            Dictionary<string, object> metrics = new Dictionary<string, object >();
            int totalReflectionProbes = 0;
            int realtimeReflectionProbes = 0;
            
            foreach (GameObject obj in allObjects)
            {
                if (obj == null) continue;
                
                foreach (ReflectionProbe probe in GetComponentsFromGameObject<ReflectionProbe>(obj))
                {
                    totalReflectionProbes++;
                    
                    if (probe.mode == ReflectionProbeMode.Realtime)
                        realtimeReflectionProbes++;
                }
            }
            
            metrics["Total Reflection Probes"] = totalReflectionProbes;
            metrics["Realtime Reflection Probes"] = realtimeReflectionProbes;
            
            return metrics;
        }
    }
}