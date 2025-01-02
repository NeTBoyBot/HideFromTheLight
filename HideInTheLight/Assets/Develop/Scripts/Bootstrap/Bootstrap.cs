using Develop.Scripts.Services.Abstractions;
using DI;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Develop.Scripts.Bootstrap
{
    public class Bootstrap : MonoBehaviour
    {
        [Scene]
        [SerializeField] private string _sceneToLoad;

        [Inject] private readonly ISceneLoader _sceneLoader;

        private async void Awake() => await _sceneLoader.LoadScene(_sceneToLoad);
    }
}

