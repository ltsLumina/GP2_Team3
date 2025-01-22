using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NoSlimes
{
    public class SceneLoader : Loggers.LoggerMonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        private LoadingScreen loadingScreen;
        private CancellationToken cancellationToken;

        private bool isLoading = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                Logger.LogWarning("Another instance of SceneLoader already exists!", this);
            }
        }

        private void Start()
        {
            loadingScreen = GetComponentInChildren<LoadingScreen>();
            cancellationToken = destroyCancellationToken;
        }

        public async void LoadScene(int sceneBuildIndex, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            await LoadSceneAsync(sceneBuildIndex, loadSceneMode);
        }

        private async Task LoadSceneAsync(int sceneBuildIndex, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if(isLoading)
                return;

            isLoading = true;

            loadingScreen.Show();
            loadingScreen.SetProgress(0);

            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, loadSceneMode);
            if (loadOperation != null)
            {
                loadOperation.allowSceneActivation = false;

                while (!loadOperation.isDone && !cancellationToken.IsCancellationRequested)
                {
                    float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
                    loadingScreen.SetProgress(progress);

                    if (loadOperation.progress >= 0.9f)
                    {
                        loadOperation.allowSceneActivation = true;
                    }

                    await Task.Yield();
                }
            }

            loadingScreen.Hide();

            isLoading = false;
        }
    }
}
