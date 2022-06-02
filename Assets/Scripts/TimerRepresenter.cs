using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerRepresenter : MonoBehaviour
{


    //TODO: Visual timer update? Listen to YoutubeAPITimer.SendDeltaTimeEvent

    private void OnDeltaUpdate(float delta = 0)
    {
        /*currentTime += delta;

        if (currentTime >= APIRequestDelay)
        {
            currentTime -= APIRequestDelay;
            OnAPIMessagesRequested?.Invoke();
        }*/

        // Part 1 - Handling the text-based timer
        /*secondsLeft -= delta;
        _timerFormatted = TimeSpan.FromSeconds(secondsLeft);
        _timerText.text = _timerFormatted.ToString(@"hh\:mm\:ss");
        */

        /*
        //Determine if it is time to switch the fill direction (and do an API call)
        if (distance >= delayTime)
            StartCoroutine(LiveStreamChat.GetChatMessages());
        */


        //TODO: Formatted timer to visualize?
        //private TimeSpan _timerFormatted;

        /*public IEnumerator RevealTimer()
        {
            if (!revealed)
            {
                SendDeltaTimeEvent(0);

                _timerFormatted = TimeSpan.FromSeconds(secondsLeft);
                string time = _timerFormatted.ToString(@"hh\:mm\:ss");

                for (int i = 0; i < time.Length; i++)
                {
                    _timerText.text = time.Substring(0, i + 1);
                    yield return new WaitForSeconds(1 / 20f);
                }
                _timerText.text = time;
                revealed = true;
            }
            yield break;
        }*/
    }
}
