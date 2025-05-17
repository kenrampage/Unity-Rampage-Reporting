using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace KenRampage.Reporting
{
    public class ShaderAnalyzer : BaseSceneAnalyzer
    {
        public override string Category => "Shaders";
        
        public override bool ShouldAnalyze(PerformanceAnalyzerSettings settings)
        {
            return settings.AnalyzeShaders;
        }
        
        public override Dictionary<string, object> Analyze(List<GameObject> allObjects, Scene scene, PerformanceAnalyzerSettings settings)
        {
            Dictionary<string, object> metrics = new Dictionary<string, object>();
            Dictionary<string, int> shaderUsage = new Dictionary<string, int>();
            int maxShaderUsageCount = 0;
            
            foreach (GameObject obj in allObjects)
            {
                if (obj == null) continue;
                
                foreach (Renderer renderer in GetComponentsFromGameObject<Renderer>(obj))
                {
                    if (renderer.sharedMaterials == null) continue;
                    
                    foreach (Material mat in renderer.sharedMaterials)
                    {
                        if (mat == null || mat.shader == null) continue;
                        
                        string shaderName = mat.shader.name;
                        if (!shaderUsage.ContainsKey(shaderName))
                            shaderUsage[shaderName] = 0;
                        shaderUsage[shaderName]++;
                        
                        if (shaderUsage[shaderName] > maxShaderUsageCount)
                            maxShaderUsageCount = shaderUsage[shaderName];
                    }
                }
            }
            
            metrics["Unique Shader Count"] = shaderUsage.Count;
            metrics["Most Used Shader Count"] = maxShaderUsageCount;
            
            return metrics;
        }
    }
}