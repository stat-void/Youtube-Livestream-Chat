using UnityEngine;


public class MainPresenter : MonoBehaviour
{
    [SerializeField] protected StartupDisplayPresenter StartPresenter;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        PreloadViews();
    }

    private void Start()
    {
        StartPresenter.Open();
    }

    private void PreloadViews()
    {
        ViewSystem.Open("ModeSelector/ModeSelector");                       // Scene that displays all possible "modes" in a list that the user can pick from.
        ViewSystem.Open("ModeScenes/ChatDisplay/ChatDisplay");              // Generic chat displayer.
        ViewSystem.Open("ModeScenes/UserListenerMode/UserListenerMode");    // Chat display where you can listen to specific users.
        ViewSystem.Open("ModeScenes/VoteMode/VoteMode");                    // Recreation of Youtube voting.
        ViewSystem.Open("ModeScenes/PollMode/PollMode");                    // Custom poller, to see general sentiment on what users are writing.
        ViewSystem.Open("ModeScenes/Settings/Settings");                    // Modify settings that affect all other views.
        ViewSystem.Open("ModeScenes/Personalisation/Personalisation");      // Modify visual settings.
    }
}
