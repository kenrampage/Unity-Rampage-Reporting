using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace KenRampage.Reporting
{
    /// <summary>
    /// Interface for all scene metric analyzers.
    /// </summary>
    public interface ISceneAnalyzer
    {
        /// <summary>
        /// Name of the analyzer category
        /// </summary>
        string Category { get; }
        
        /// <summary>
        /// Determines if this analyzer should run based on the settings
        /// </summary>
        bool ShouldAnalyze(PerformanceAnalyzerSettings settings);
        
        /// <summary>
        /// Performs analysis and returns metrics
        /// </summary>
        Dictionary<string, object> Analyze(List<GameObject> allObjects, Scene scene, PerformanceAnalyzerSettings settings);
    }
}