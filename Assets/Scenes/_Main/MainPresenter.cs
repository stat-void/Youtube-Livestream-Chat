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
    Version 2 - Manual Timer activation/deactivation
        Query items were moved to have a good spot for the play/pause button.
        And you need to make play/pause icons.
        Maybe change text from "wait" to "delay" too.

    Version 2 - Rare cases of chat display messing up text display.
        Determined 2 separate situations as the cause:
            1. Too many messages in a row, easier to debug with manual timer.
            2. Some Superchat events having an unhandled tier value.

    Version 2 - First voter system.
        You know what this is.

    Version 2 - Second voter system.
        You also know what this is.

    Version 2 - Even more language support
        There are always unpredictable symbols that may be used in a chat.

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
