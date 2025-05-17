using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace KenRampage.Reporting
{
    public class RenderingPerformanceAnalyzer : BaseSceneAnalyzer
    {
        public override string Category => "Rendering Performance";
        
        public override bool ShouldAnalyze(PerformanceAnalyzerSettings settings)
        {
            return settings.AnalyzeDrawCalls || 
                   settings.AnalyzeBatching || 
                   settings.AnalyzeGPUInstancing;
        }
        
        public override Dictionary<string, object> Analyze(List<GameObject> allObjects, Scene scene, PerformanceAnalyzerSettings settings)
        {
            Dictionary<string, object> metrics = new Dictionary<string, object>();
            
            int potentialDrawCalls = 0;
            int staticBatchingObjects = 0;
            int dynamicBatchingObjects = 0;
            int gpuInstancingObjects = 0;
            
            foreach (GameObject obj in allObjects)
            {
                if (obj == null) continue;
                
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer == null || !renderer.enabled) continue;
                
                // Count potential draw calls (each renderer potentially causes one or more)
                potentialDrawCalls++;
                
                // Check for batching flags
                if (obj.isStatic && (GameObjectUtility.GetStaticEditorFlags(obj) & StaticEditorFlags.BatchingStatic) != 0)
                {
                    staticBatchingObjects++;
                }
                else
                {
                    // Check if it could be dynamically batched (small meshes, etc.)
                    MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null && 
                        meshFilter.sharedMesh.vertexCount < 900) // Unity's approximate limit
                    {
                        dynamicBatchingObjects++;
                    }
                }
                
                // Check for GPU instancing
                if (renderer.sharedMaterials != null)
                {
                    foreach (Material mat in renderer.sharedMaterials)
                    {
                        if (mat != null && mat.enableInstancing)
                        {
                            gpuInstancingObjects++;
                            break; // Count the object only once even if multiple materials use instancing
                        }
                    }
                }
            }
            
            metrics["Potential Draw Calls"] = potentialDrawCalls;
            metrics["Static Batching Objects"] = staticBatchingObjects;
            metrics["Dynamic Batching Candidates"] = dynamicBatchingObjects;
            metrics["GPU Instancing Objects"] = gpuInstancingObjects;
            
            return metrics;
        }
    }
}