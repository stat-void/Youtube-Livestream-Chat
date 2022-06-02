using UnityEngine;
using Void.YoutubeAPI.LiveStreamChat.Messages;

public abstract class AListItem : MonoBehaviour
{
    public abstract void Bind(YoutubeChatMessage node, Transform active);
    public abstract void Unbind(Transform pool);

    public abstract float GetHeight();
    public abstract void UpdateFit();
}
