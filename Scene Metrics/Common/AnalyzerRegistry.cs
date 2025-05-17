using System.Collections.Generic;
using KenRampage.Reporting;

namespace KenRampage.Reporting
{
    /// <summary>
    /// Registry of all scene analyzers
    /// </summary>
    public static class AnalyzerRegistry
    {
        private static List<ISceneAnalyzer> _analyzers;
        
        public static IReadOnlyList<ISceneAnalyzer> Analyzers
        {
            get
            {
                if (_analyzers == null)
                {
                    InitializeAnalyzers();
                }
                return _analyzers;
            }
        }
        
        private static void InitializeAnalyzers()
        {
            _analyzers = new List<ISceneAnalyzer>
            {
                new GameObjectAnalyzer(),
                new HierarchyDepthAnalyzer(),
                new EmptyGameObjectAnalyzer(),
                new RendererAnalyzer(),
                new MaterialAnalyzer(),
                new ShaderAnalyzer(),
                new TextureAnalyzer(),
                new CameraAnalyzer(),
                new ColliderAnalyzer(),
                new RigidbodyAnalyzer(),
                new PrefabAnalyzer(),
                new ParticleSystemAnalyzer(),
                new LightAnalyzer(),
                new ReflectionProbeAnalyzer(),
                new AudioSourceAnalyzer(),
                new RenderingPerformanceAnalyzer(),
                new ComponentCountAnalyzer(),
                // Add additional analyzers as needed
            };
        }
    }
}