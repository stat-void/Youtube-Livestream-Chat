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
        ViewSystem.Open("ModeScenes/Settings/Settings");        // Modify settings that affect all other views.
    }


    /*
    TODO List

    Have a second pass on chat display, anything with 2 rows or more gets somewhat messed up?
        3 rows shows clear overflow.

    Bugfixing session for chat display. If full list size has not been passed, switching between this and settings causes weird results,
        such as chat not going all the way to the bottom up to a certain amount of messages, or not all messages being removed?

    Check out the IEnumerator for chat display in build
        There was one extremely rare case of a timestamp that could not be parsed.

    Do I need a disclaimer for API Key usage guidelines?
        Youtube mentioned it somewhere, but where should I?


    In-build guide on how to get an API Key.
        So people wouldn't have to read the GitLab/Hub page.

    Text cleanup?
        Some people can send messages later than others, but due to a better connection, their message will come up first, causing
            the resulting text to pop up multiple times. Should I add some minor checks for the previous message bulk to ignore repeating ID / message content?
        I would have to implement the checking condition this needs with Focus Mode anyway.


    Settings screen
        Modifiable duration value, with the option to pick the "recommended" dynamic wait time from Youtube as well
        Options to delete saved data
        Options to delete the API Key


    Resizing
        Fix some of the elements with janky resizing so freeform resizing can be smaller width-wise without overlap. (mainly query panel)


    Text
        More language support? Any way to reduce memory on it?


    Focus Mode
        The first Screen Manager extra function. You already know how this should go.
        Also, maybe document all the other ideas here at some point?


    RecyclerView lists with resizable elements.
        So ChatMessageListener can increase the amount of messages to 500/1000 remembered with trivial performance hits.
        This could also make the movement of the chat display smoother?
        What about a RecyclerGrid for some other areas?


    Long term - Android Support?
        I'd have to make sure that touch inputs work okay, but other than that, Would I benefit from having this as a portable solution?
        What about frame rates? Would having a selectable 30/60 fps mode help?
    */

}
