using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace KenRampage.Reporting
{
    public class LightAnalyzer : BaseSceneAnalyzer
    {
        public override string Category => "Lights";
        
        public override bool ShouldAnalyze(PerformanceAnalyzerSettings settings)
        {
            return settings.AnalyzeLights;
        }
        
        public override Dictionary<string, object> Analyze(List<GameObject> allObjects, Scene scene, PerformanceAnalyzerSettings settings)
        {
            Dictionary<string, object> metrics = new Dictionary<string, object>();
            Dictionary<LightType, int> lightTypes = new Dictionary<LightType, int>();
            Dictionary<LightRenderMode, int> lightRenderModes = new Dictionary<LightRenderMode, int>();
            int totalLights = 0;
            int realtimeLights = 0;
            
            foreach (GameObject obj in allObjects)
            {
                if (obj == null) continue;
                
                foreach (Light light in GetComponentsFromGameObject<Light>(obj))
                {
                    totalLights++;
                    
                    if (!lightTypes.ContainsKey(light.type))
                        lightTypes[light.type] = 0;
                    lightTypes[light.type]++;
                    
                    if (!lightRenderModes.ContainsKey(light.renderMode))
                        lightRenderModes[light.renderMode] = 0;
                    lightRenderModes[light.renderMode]++;
                    
                    if (light.renderMode == LightRenderMode.ForcePixel || 
                        light.renderMode == LightRenderMode.Auto)
                        realtimeLights++;
                }
            }
            
            metrics["Total Lights"] = totalLights;
            metrics["Realtime Lights"] = realtimeLights;
            
            foreach (var kvp in lightTypes)
            {
                metrics[$"{kvp.Key} Light Count"] = kvp.Value;
            }
            
            foreach (var kvp in lightRenderModes)
            {
                metrics[$"{kvp.Key} Render Mode Count"] = kvp.Value;
            }
            
            return metrics;
        }
    }
}