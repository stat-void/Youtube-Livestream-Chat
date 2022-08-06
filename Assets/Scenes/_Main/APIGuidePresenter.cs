using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class APIGuidePresenter : MonoBehaviour
{
    [SerializeField] protected GameObject BaseGO;

    [SerializeField] protected List<TextItem> TextFields;

    [SerializeField] protected Button OpenButton;
    [SerializeField] protected Button CloseButton;

    [SerializeField] protected Button TOSButton;
    [SerializeField] protected Button ConsoleButton;
    [SerializeField] protected Button ProjectButton;
    [SerializeField] protected Button LibraryButton;
    [SerializeField] protected Button CredentialsButton;
    [SerializeField] protected Button QuotaButton;

    bool _quitting = false;
    private IEnumerator _currentResize = null;
    private float _resizeWaitSeconds = 0f;

    private void Awake()
    {
        OpenButton.onClick          .AddListener(OnOpenClicked);
        CloseButton.onClick         .AddListener(OnCloseClicked);
        TOSButton.onClick           .AddListener(OnTOSClicked);
        ConsoleButton.onClick       .AddListener(OnConsoleClicked);
        ProjectButton.onClick       .AddListener(OnProjectClicked);
        LibraryButton.onClick       .AddListener(OnLibraryClicked);
        CredentialsButton.onClick   .AddListener(OnCredentialsClicked);
        QuotaButton.onClick         .AddListener(OnQuotaClicked);

        ScreenResizeListener.OnResize += OnScreenResize;

        BaseGO.SetActive(false);
    }

    public void Finish()
    {
        OpenButton.onClick          .RemoveListener(OnOpenClicked);
        CloseButton.onClick         .RemoveListener(OnCloseClicked);
        TOSButton.onClick           .RemoveListener(OnTOSClicked);
        ConsoleButton.onClick       .RemoveListener(OnConsoleClicked);
        ProjectButton.onClick       .RemoveListener(OnProjectClicked);
        LibraryButton.onClick       .RemoveListener(OnLibraryClicked);
        CredentialsButton.onClick   .RemoveListener(OnCredentialsClicked);
        QuotaButton.onClick         .RemoveListener(OnQuotaClicked);

        ScreenResizeListener.OnResize -= OnScreenResize;

        BaseGO.SetActive(false);
    }

    private void OnScreenResize()
    {
        if (_quitting)
            return;

        _resizeWaitSeconds = 0.1f;

        if (_currentResize == null)
        {
            _currentResize = RefreshFit();
            StartCoroutine(_currentResize);
        }
    }

    private IEnumerator RefreshFit()
    {
        while (_resizeWaitSeconds > 0)
        {
            yield return null;
            _resizeWaitSeconds -= Time.unscaledDeltaTime;
        }

        _resizeWaitSeconds = 0;
        foreach (TextItem item in TextFields)
            item.UpdateFit();

        _currentResize = null;
        yield break;
    }



    private async void OnOpenClicked()
    {
        BaseGO.SetActive(true);
        await Task.Delay(20);

        foreach (TextItem item in TextFields)
            item.UpdateFit();
    } 

    private void OnCloseClicked() =>
        BaseGO.SetActive(false);

    private void OnTOSClicked() =>
        Application.OpenURL("https://developers.google.com/youtube/terms/api-services-terms-of-service");

    private void OnConsoleClicked() =>
        Application.OpenURL("https://console.cloud.google.com");

    private void OnProjectClicked() =>
        Application.OpenURL("https://console.cloud.google.com/projectcreate");

    private void OnLibraryClicked() =>
        Application.OpenURL("https://console.cloud.google.com/apis/library/youtube.googleapis.com");

    private void OnCredentialsClicked() =>
        Application.OpenURL("https://console.cloud.google.com/apis/credentials");

    private void OnQuotaClicked() =>
        Application.OpenURL("https://cloud.google.com/docs/quota");

    private void OnApplicationQuit()
    {
        _quitting = true;
    }
}
