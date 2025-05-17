using UnityEngine;
using System;

namespace KenRampage.Reporting
{
    /// <summary>
    /// Settings for controlling which metrics to analyze in the Performance Analyzer.
    /// </summary>
    [Serializable]
    public class PerformanceAnalyzerSettings : ScriptableObject
    {
        // GameObject hierarchy metrics
        public bool AnalyzeGameObjectCounts = true;
        public bool AnalyzeHierarchyDepth = true;
        public bool AnalyzeEmptyGameObjects = true;
        
        // Rendering metrics
        public bool AnalyzeMaterials = true;
        public bool AnalyzeShaders = true;
        public bool AnalyzeTextures = true;
        public bool AnalyzeRenderers = true;
        public bool AnalyzeCameras = true;
        
        // Rendering Performance
        public bool AnalyzeDrawCalls = true;
        public bool AnalyzeBatching = true;
        public bool AnalyzeGPUInstancing = true;
        
        // Physics metrics
        public bool AnalyzeColliders = true;
        public bool AnalyzeRigidbodies = true;
        
        // Asset references
        public bool AnalyzePrefabInstances = true;
        
        // Performance-critical components
        public bool AnalyzeParticleSystems = true;
        public bool AnalyzeLights = true;
        public bool AnalyzeReflectionProbes = true;
        public bool AnalyzeAudioSources = true;
        
        // Component Analysis
        public bool AnalyzeComponentDistribution = true;
        
        // Analysis thresholds
        public int MaxRecommendedHierarchyDepth = 8;
        public int MaxRecommendedTextureSize = 2048;
        public int MaxRecommendedPolygonCount = 100000;

        private static PerformanceAnalyzerSettings _instance;
        
        /// <summary>
        /// Gets or creates a settings instance.
        /// </summary>
        /// <returns>The settings instance.</returns>
        public static PerformanceAnalyzerSettings GetOrCreateSettings()
        {
            if (_instance != null)
                return _instance;
            
            _instance = ScriptableObject.CreateInstance<PerformanceAnalyzerSettings>();
            return _instance;
        }
    }
}