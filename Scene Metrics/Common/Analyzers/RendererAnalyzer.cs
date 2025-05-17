using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace KenRampage.Reporting
{
    public class RendererAnalyzer : BaseSceneAnalyzer
    {
        public override string Category => "Renderers";
        
        public override bool ShouldAnalyze(PerformanceAnalyzerSettings settings)
        {
            return settings.AnalyzeRenderers;
        }
        
        public override Dictionary<string, object> Analyze(List<GameObject> allObjects, Scene scene, PerformanceAnalyzerSettings settings)
        {
            Dictionary<string, object> metrics = new Dictionary<string, object>();
            Dictionary<string, int> rendererTypes = new Dictionary<string, int>();
            int totalTriangleCount = 0;
            int totalVertexCount = 0;
            int highPolyObjectsCount = 0;
            
            foreach (GameObject obj in allObjects)
            {
                if (obj == null) continue;
                
                // Count different renderer types
                foreach (Renderer renderer in GetComponentsFromGameObject<Renderer>(obj))
                {
                    string rendererType = renderer.GetType().Name;
                    if (!rendererTypes.ContainsKey(rendererType))
                        rendererTypes[rendererType] = 0;
                    rendererTypes[rendererType]++;
                    
                    // Calculate approximate triangle/vertex counts
                    if (renderer is MeshRenderer)
                    {
                        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                        if (meshFilter != null && meshFilter.sharedMesh != null)
                        {
                            int triangles = meshFilter.sharedMesh.triangles.Length / 3;
                            int vertices = meshFilter.sharedMesh.vertexCount;
                            totalTriangleCount += triangles;
                            totalVertexCount += vertices;
                            
                            // Track high-poly objects
                            if (triangles > 5000)
                            {
                                highPolyObjectsCount++;
                            }
                        }
                    }
                    else if (renderer is SkinnedMeshRenderer skinnedMesh)
                    {
                        if (skinnedMesh.sharedMesh != null)
                        {
                            int triangles = skinnedMesh.sharedMesh.triangles.Length / 3;
                            int vertices = skinnedMesh.sharedMesh.vertexCount;
                            totalTriangleCount += triangles;
                            totalVertexCount += vertices;
                            
                            // Track high-poly objects
                            if (triangles > 5000)
                            {
                                highPolyObjectsCount++;
                            }
                        }
                    }
                }
            }
            
            // Add metrics
            foreach (var kvp in rendererTypes)
            {
                metrics[$"{kvp.Key} Count"] = kvp.Value;
            }
            
            metrics["Total Triangle Count"] = totalTriangleCount;
            metrics["Total Vertex Count"] = totalVertexCount;
            metrics["High-Poly Object Count"] = highPolyObjectsCount;
            
            // Removed detailed high-poly object path listings
            
            return metrics;
        }
    }
}