using System.Threading;
using System.Threading.Tasks;
using NoSlimes.Loggers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NoSlimes
{
    public class SceneLoader : LoggerMonoBehaviour
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

            if(SceneManager.GetActiveScene().buildIndex == (int)SceneIndexes.MANAGER)
            {
                LoadScene((int)SceneIndexes.MAIN_MENU);
            }
        }

        public async void LoadScene(int sceneBuildIndex, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            await LoadSceneAsync(sceneBuildIndex, loadSceneMode);
        }

        private async Awaitable LoadSceneAsync(int sceneBuildIndex, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if(isLoading)
                return;

            isLoading = true;

            loadingScreen.Show();
            loadingScreen.SetProgress(0);

            await Awaitable.NextFrameAsync(); // Note: has overload with cancellation support

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

            await Awaitable.NextFrameAsync(); // Note: has overload with cancellation support

            loadingScreen.Hide();
            isLoading = false;
        }
    }
}
