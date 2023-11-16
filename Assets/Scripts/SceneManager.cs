using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public class SceneManager
    {
        [SerializeField] private List<SceneItem> scenes;

        private readonly Dictionary<Enums.Scene, string> scenesLookup = new Dictionary<Enums.Scene, string>();
        private readonly Dictionary<Enums.Scene, GameObject> loadedScenes = new Dictionary<Enums.Scene, GameObject>();

        private int playersCountFinishedOperation;

        public void Initialize()
        {
            BuildLookup();
        }

        public void LoadScene(Enums.Scene sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(scenesLookup[sceneName]);
        }
        
        private void BuildLookup()
        {
            foreach (SceneItem sceneItem in scenes)
            {
                scenesLookup.Add(sceneItem.SceneName, sceneItem.SceneObject);
            }
        }
        
        [Serializable]
        private struct SceneItem
        {
            [field: SerializeField] public Enums.Scene SceneName { get; set; }
            [field: SerializeField, Scene] public string SceneObject { get; set; }
        }
    }
}