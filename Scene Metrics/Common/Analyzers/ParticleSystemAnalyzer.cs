using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace KenRampage.Reporting
{
    public class ParticleSystemAnalyzer : BaseSceneAnalyzer
    {
        public override string Category => "Particle Systems";
        
        public override bool ShouldAnalyze(PerformanceAnalyzerSettings settings)
        {
            return settings.AnalyzeParticleSystems;
        }
        
        public override Dictionary<string, object> Analyze(List<GameObject> allObjects, Scene scene, PerformanceAnalyzerSettings settings)
        {
            Dictionary<string, object> metrics = new Dictionary<string, object>();
            int totalParticleSystems = 0;
            int activeParticleSystems = 0;
            int maxParticles = 0;
            int highParticleCountSystemsCount = 0;
            
            foreach (GameObject obj in allObjects)
            {
                if (obj == null) continue;
                
                foreach (ParticleSystem ps in GetComponentsFromGameObject<ParticleSystem>(obj))
                {
                    totalParticleSystems++;
                    
                    if (obj.activeInHierarchy && ps.isPlaying)
                        activeParticleSystems++;
                    
                    int particleCount = ps.main.maxParticles;
                    maxParticles += particleCount;
                    
                    if (particleCount > 1000)
                    {
                        highParticleCountSystemsCount++;
                    }
                }
            }
            
            metrics["Total Particle Systems"] = totalParticleSystems;
            metrics["Active Particle Systems"] = activeParticleSystems;
            metrics["Total Max Particle Count"] = maxParticles;
            metrics["High Particle Count Systems"] = highParticleCountSystemsCount;
            
            // Removed detailed particle system path listings
            
            return metrics;
        }
    }
}