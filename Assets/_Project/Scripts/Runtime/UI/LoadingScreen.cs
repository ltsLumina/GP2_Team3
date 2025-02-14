using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NoSlimes
{
    public class LoadingScreen : MonoBehaviour
    {
        [Header("Loading Screen")]
        [SerializeField] private GameObject screen;
        [SerializeField] private Image loadingBar;
        [SerializeField] private Image loadingIcon;
        [SerializeField] private float iconSpinSpeed = 2.3f;

        [Header("Tips")]
        [SerializeField] private TMP_Text tipText;
        [SerializeField] private TipsSO tips;

        private void Awake()
        {
            //screen = transform.GetChild(0).gameObject;
        }

        public void Show()
        {
            ShowTip();
            loadingIcon.transform.rotation = Quaternion.identity;

            screen.SetActive(true);
        }

        public void Hide() => screen.SetActive(false);

        public void SetProgress(float progress)
        {
            progress = Mathf.Clamp01(progress);

            loadingBar.fillAmount = progress;
            loadingIcon.transform.Rotate(Vector3.back, iconSpinSpeed * progress * 8);
        }

        private void ShowTip()
        {
            string tip = tips.GetRandomTip();
            tipText.text = tip;
        }
    }
}
