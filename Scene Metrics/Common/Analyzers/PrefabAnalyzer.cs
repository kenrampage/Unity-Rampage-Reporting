using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace KenRampage.Reporting
{
    public class PrefabAnalyzer : BaseSceneAnalyzer
    {
        public override string Category => "Prefab Instances";
        
        public override bool ShouldAnalyze(PerformanceAnalyzerSettings settings)
        {
            return settings.AnalyzePrefabInstances;
        }
        
        public override Dictionary<string, object> Analyze(List<GameObject> allObjects, Scene scene, PerformanceAnalyzerSettings settings)
        {
            Dictionary<string, object> metrics = new Dictionary<string, object>();
            Dictionary<string, int> prefabInstances = new Dictionary<string, int>();
            int totalPrefabInstances = 0;
            int maxInstanceCount = 0;
            
            foreach (GameObject obj in allObjects)
            {
                if (obj == null) continue;
                
                if (PrefabUtility.IsPartOfPrefabInstance(obj) && PrefabUtility.IsOutermostPrefabInstanceRoot(obj))
                {
                    totalPrefabInstances++;
                    
                    GameObject prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(obj);
                    if (prefabAsset != null)
                    {
                        string prefabName = prefabAsset.name;
                        if (!prefabInstances.ContainsKey(prefabName))
                            prefabInstances[prefabName] = 0;
                        prefabInstances[prefabName]++;
                        
                        if (prefabInstances[prefabName] > maxInstanceCount)
                            maxInstanceCount = prefabInstances[prefabName];
                    }
                }
            }
            
            metrics["Total Prefab Instances"] = totalPrefabInstances;
            metrics["Unique Prefab Types"] = prefabInstances.Count;
            metrics["Most Common Prefab Instance Count"] = maxInstanceCount;
            
            return metrics;
        }
    }
}