using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace KenRampage.Reporting
{
    public class AudioSourceAnalyzer : BaseSceneAnalyzer
    {
        public override string Category => "Audio Sources";
        
        public override bool ShouldAnalyze(PerformanceAnalyzerSettings settings)
        {
            return settings.AnalyzeAudioSources;
        }
        
        public override Dictionary<string, object> Analyze(List<GameObject> allObjects, Scene scene, PerformanceAnalyzerSettings settings)
        {
            Dictionary<string, object> metrics = new Dictionary<string, object>();
            int totalAudioSources = 0;
            int playOnAwakeSources = 0;
            int loopSources = 0;
            
            foreach (GameObject obj in allObjects)
            {
                if (obj == null) continue;
                
                foreach (AudioSource source in GetComponentsFromGameObject<AudioSource>(obj))
                {
                    totalAudioSources++;
                    
                    if (source.playOnAwake)
                        playOnAwakeSources++;
                    
                    if (source.loop)
                        loopSources++;
                }
            }
            
            metrics["Total Audio Sources"] = totalAudioSources;
            metrics["Play On Awake Sources"] = playOnAwakeSources;
            metrics["Looping Audio Sources"] = loopSources;
            
            return metrics;
        }
    }
}