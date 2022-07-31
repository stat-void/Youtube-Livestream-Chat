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
        ViewSystem.Open("ModeSelector/ModeSelector");           // Scene that displays all possible "modes" in a list that the user can pick from.
        ViewSystem.Open("ModeScenes/ChatDisplay/ChatDisplay");  // Generic chat displayer
        ViewSystem.Open("ModeScenes/FocusMode/FocusMode");      // Chat display where you can listen to specific users
        ViewSystem.Open("ModeScenes/Settings/Settings");        // Modify settings that affect all other views.
    }


    /*
    TODO List

    Manual Timer activation/deactivation
        Will require slight modification for the query panel positioning.

    Do I need a disclaimer for API Key usage guidelines?
        Youtube mentioned it somewhere, but where should I?

    In-build guide on how to get an API Key.
        So people wouldn't have to read the GitLab/Hub page.

    Text
        More language support? Any way to reduce memory on it?

    Long term - Rare cases of chat display messing up text display. Might take awhile to catch and debug it.

    Long term - Text cleanup?
        Some people can send messages later than others, but due to a better connection, their message will come up first, causing
            the resulting text to pop up multiple times. Should I add some minor checks for the previous message bulk to ignore repeating ID / message content?
            Occurrence rate relatively rare so I'll not focus on this for now.

    Long term - Fix current ARecyclerList visual bug (More info in said class).
        When scrolling too aggressively, some of the start/end elements can disappear. Refreshes will make them reappear though.

    Long term - RecyclerView lists with resizable elements.
        So ChatMessageListener can increase the amount of messages to 500/1000 remembered with trivial performance hits.
        This could also make the movement of the chat display smoother?
        What about a RecyclerGrid for some other areas?

    Long term - Android Support?
        I'd have to make sure that touch inputs work okay, but other than that, Would I benefit from having this as a portable solution?
        What about frame rates? Would having a selectable 30/60 fps mode help?
    */

}
