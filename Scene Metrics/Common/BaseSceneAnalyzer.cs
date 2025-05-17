using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace KenRampage.Reporting
{
    /// <summary>
    /// Base class for scene analyzers providing common functionality
    /// </summary>
    public abstract class BaseSceneAnalyzer : ISceneAnalyzer
    {
        public abstract string Category { get; }
        
        public abstract bool ShouldAnalyze(PerformanceAnalyzerSettings settings);
        
        public abstract Dictionary<string, object> Analyze(List<GameObject> allObjects, Scene scene, PerformanceAnalyzerSettings settings);
        
        /// <summary>
        /// Gets the full path of a GameObject in the hierarchy.
        /// </summary>
        protected string GetGameObjectPath(GameObject obj)
        {
            if (obj == null) return "null";
            
            string path = obj.name;
            Transform parent = obj.transform.parent;
            
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            
            return path;
        }
        
        /// <summary>
        /// Safe method to get components, handling null objects
        /// </summary>
        protected T[] GetComponentsFromGameObject<T>(GameObject obj) where T : Component
        {
            if (obj == null) return new T[0];
            return obj.GetComponents<T>();
        }
    }
}