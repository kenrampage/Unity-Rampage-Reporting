using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace KenRampage.Reporting
{
    public class CameraAnalyzer : BaseSceneAnalyzer
    {
        public override string Category => "Cameras";
        
        public override bool ShouldAnalyze(PerformanceAnalyzerSettings settings)
        {
            return settings.AnalyzeCameras;
        }
        
        public override Dictionary<string, object> Analyze(List<GameObject> allObjects, Scene scene, PerformanceAnalyzerSettings settings)
        {
            Dictionary<string, object> metrics = new Dictionary<string, object>();
            Dictionary<string, int> cameraRenderPaths = new Dictionary<string, int>();
            int totalCameras = 0;
            int activeCameras = 0;
            
            foreach (GameObject obj in allObjects)
            {
                if (obj == null) continue;
                
                foreach (Camera camera in GetComponentsFromGameObject<Camera>(obj))
                {
                    totalCameras++;
                    if (obj.activeInHierarchy && camera.enabled)
                        activeCameras++;
                    
                    string renderPath = camera.renderingPath.ToString();
                    if (!cameraRenderPaths.ContainsKey(renderPath))
                        cameraRenderPaths[renderPath] = 0;
                    cameraRenderPaths[renderPath]++;
                }
            }
            
            metrics["Total Cameras"] = totalCameras;
            metrics["Active Cameras"] = activeCameras;
            
            foreach (var kvp in cameraRenderPaths)
            {
                metrics[$"Render Path: {kvp.Key}"] = kvp.Value;
            }
            
            return metrics;
        }
    }
}