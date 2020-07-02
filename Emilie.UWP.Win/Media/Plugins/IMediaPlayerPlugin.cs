using Windows.Media.Playback;

namespace Emilie.UWP.Media
{
    public interface IMediaPlayerPlugin
    {
        MediaPlayer MediaPlayer { get; }
        void Attach(MediaPlayer player);
        void Detach();
    }
}
