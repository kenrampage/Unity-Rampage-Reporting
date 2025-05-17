using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace KenRampage.Reporting
{
    public class TextureAnalyzer : BaseSceneAnalyzer
    {
        public override string Category => "Textures";
        
        public override bool ShouldAnalyze(PerformanceAnalyzerSettings settings)
        {
            return settings.AnalyzeTextures;
        }
        
        public override Dictionary<string, object> Analyze(List<GameObject> allObjects, Scene scene, PerformanceAnalyzerSettings settings)
        {
            Dictionary<string, object> metrics = new Dictionary<string, object>();
            HashSet<Texture> uniqueTextures = new HashSet<Texture>();
            int largeTexturesCount = 0;
            long estimatedTextureMemory = 0;
            int maxTextureSize = 0;
            
            foreach (GameObject obj in allObjects)
            {
                if (obj == null) continue;
                
                foreach (Renderer renderer in GetComponentsFromGameObject<Renderer>(obj))
                {
                    if (renderer.sharedMaterials == null) continue;
                    
                    foreach (Material mat in renderer.sharedMaterials)
                    {
                        if (mat == null) continue;
                        
                        // Get all texture properties in the material
                        foreach (string propertyName in mat.GetTexturePropertyNames())
                        {
                            Texture texture = mat.GetTexture(propertyName);
                            if (texture == null) continue;
                            
                            uniqueTextures.Add(texture);
                            
                            // Check if it's a large texture
                            if (texture is Texture2D tex2D)
                            {
                                if (tex2D.width > settings.MaxRecommendedTextureSize || tex2D.height > settings.MaxRecommendedTextureSize)
                                {
                                    largeTexturesCount++;
                                }
                                
                                // Track maximum texture size
                                int size = Mathf.Max(tex2D.width, tex2D.height);
                                if (size > maxTextureSize)
                                    maxTextureSize = size;
                                
                                // Estimate memory usage (very approximate)
                                int bytesPerPixel = GetBytesPerPixel(tex2D.format);
                                estimatedTextureMemory += (long)tex2D.width * tex2D.height * bytesPerPixel;
                            }
                        }
                    }
                }
            }
            
            metrics["Unique Texture Count"] = uniqueTextures.Count;
            metrics["Estimated Texture Memory (MB)"] = estimatedTextureMemory / (1024f * 1024f);
            metrics["Large Textures Count"] = largeTexturesCount;
            metrics["Maximum Texture Size"] = maxTextureSize;
            
            // Removed detailed texture path listings
            
            return metrics;
        }
        
        private int GetBytesPerPixel(TextureFormat format)
        {
            switch (format)
            {
                case TextureFormat.Alpha8:
                    return 1;
                case TextureFormat.RGB24:
                    return 3;
                case TextureFormat.RGBA32:
                    return 4;
                case TextureFormat.RGB565:
                case TextureFormat.R16:
                    return 2;
                case TextureFormat.DXT1:
                    return 1; // Approximation (it's actually compressed)
                case TextureFormat.DXT5:
                    return 1; // Approximation (it's actually compressed but higher quality than DXT1)
                default:
                    return 4; // Default assumption
            }
        }
    }
}