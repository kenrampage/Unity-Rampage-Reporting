using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace KenRampage.Reporting
{
    public class MaterialAnalyzer : BaseSceneAnalyzer
    {
        public override string Category => "Materials";
        
        public override bool ShouldAnalyze(PerformanceAnalyzerSettings settings)
        {
            return settings.AnalyzeMaterials;
        }
        
        public override Dictionary<string, object> Analyze(List<GameObject> allObjects, Scene scene, PerformanceAnalyzerSettings settings)
        {
            Dictionary<string, object> metrics = new Dictionary<string, object>();
            HashSet<Material> uniqueMaterials = new HashSet<Material>();
            Dictionary<Material, int> materialInstances = new Dictionary<Material, int>();
            int totalMaterialSlots = 0;
            int mostUsedMaterialCount = 0;
            
            foreach (GameObject obj in allObjects)
            {
                if (obj == null) continue;
                
                foreach (Renderer renderer in GetComponentsFromGameObject<Renderer>(obj))
                {
                    if (renderer.sharedMaterials != null)
                    {
                        totalMaterialSlots += renderer.sharedMaterials.Length;
                        
                        foreach (Material mat in renderer.sharedMaterials)
                        {
                            if (mat == null) continue;
                            
                            uniqueMaterials.Add(mat);
                            
                            if (!materialInstances.ContainsKey(mat))
                                materialInstances[mat] = 0;
                            materialInstances[mat]++;
                            
                            if (materialInstances[mat] > mostUsedMaterialCount)
                                mostUsedMaterialCount = materialInstances[mat];
                        }
                    }
                }
            }
            
            metrics["Unique Material Count"] = uniqueMaterials.Count;
            metrics["Total Material Slots"] = totalMaterialSlots;
            metrics["Most Used Material Count"] = mostUsedMaterialCount;
            
            // Removed detailed material name listings
            
            return metrics;
        }
    }
}