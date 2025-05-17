using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace KenRampage.Reporting
{
    public class HierarchyDepthAnalyzer : BaseSceneAnalyzer
    {
        public override string Category => "Hierarchy Depth";
        
        public override bool ShouldAnalyze(PerformanceAnalyzerSettings settings)
        {
            return settings.AnalyzeHierarchyDepth;
        }
        
        public override Dictionary<string, object> Analyze(List<GameObject> allObjects, Scene scene, PerformanceAnalyzerSettings settings)
        {
            Dictionary<string, object> metrics = new Dictionary<string, object>();
            int maxDepth = 0;
            int objectsExceedingRecommendedDepth = 0;
            int[] depthCounts = new int[100]; // Array to store counts at each depth level
            
            // Get root objects from the scene
            GameObject[] rootObjects = scene.GetRootGameObjects();
            
            foreach (GameObject rootObject in rootObjects)
            {
                AnalyzeObjectDepth(rootObject.transform, 0);
            }
            
            metrics["Maximum Hierarchy Depth"] = maxDepth;
            metrics["Objects Exceeding Recommended Depth"] = objectsExceedingRecommendedDepth;
            
            // Instead of individual metrics for each depth, add a summary
            metrics["Total Depth Distribution Count"] = depthCounts.Sum();
            metrics["Average Object Depth"] = CalculateAverageDepth(depthCounts);
            
            // Helper function to analyze depth
            void AnalyzeObjectDepth(Transform t, int depth)
            {
                // Record depth distribution
                if (depth < depthCounts.Length)
                    depthCounts[depth]++;
                
                // Track maximum depth
                if (depth > maxDepth)
                    maxDepth = depth;
                
                // Count objects exceeding recommended depth
                if (depth > settings.MaxRecommendedHierarchyDepth)
                    objectsExceedingRecommendedDepth++;
                
                // Process children
                foreach (Transform child in t)
                {
                    AnalyzeObjectDepth(child, depth + 1);
                }
            }
            
            return metrics;
        }
        
        private float CalculateAverageDepth(int[] depthCounts)
        {
            float sum = 0;
            int count = 0;
            
            for (int i = 0; i < depthCounts.Length; i++)
            {
                sum += i * depthCounts[i];
                count += depthCounts[i];
            }
            
            return count > 0 ? sum / count : 0;
        }
    }
}