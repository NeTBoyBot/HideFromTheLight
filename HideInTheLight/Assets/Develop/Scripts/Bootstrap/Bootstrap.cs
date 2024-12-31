using Develop.Scripts.Services.Abstractions;
using DI;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Develop.Scripts.Bootstrap
{
    public class Bootstrap : MonoBehaviour
    {
        [Dropdown("GetSceneNames")]
        [SerializeField] private string _sceneToLoad;

        [Inject] private readonly ISceneLoader _sceneLoader;

        private async void Awake() => await _sceneLoader.LoadScene(_sceneToLoad);

        private string[] GetSceneNames()
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            string[] sceneNames = new string[sceneCount - 1];

            for (int i = 1; i < sceneCount; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);

                sceneNames[i - 1] = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            }

            return sceneNames;
        }
    }
}

