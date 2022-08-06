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

    private void OnScreenResize(Vector2 arg1, Vector2 arg2)
    {
        foreach (TextItem item in TextFields)
            item.UpdateFit();
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


}
