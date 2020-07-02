using Emilie.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Media.Playback;

namespace Emilie.UWP.Media
{
    public class MediaPlayerWrapper : IDisposable
    {
        List<IMediaPlayerPlugin> _plugins { get; } = new List<IMediaPlayerPlugin>();
        public IReadOnlyList<IMediaPlayerPlugin> Plugins => _plugins;

        public MediaPlayer MediaPlayer { get; private set; }

        public MediaPlayerWrapper()
        {
            MediaPlayer = new MediaPlayer();
        }

        public MediaPlayerWrapper(MediaPlayer player)
        {
            MediaPlayer = player;
        }

        public void AddPlugin(IMediaPlayerPlugin plugin)
        {
            _plugins.Add(plugin);
            plugin.Attach(MediaPlayer);
        }

        public void DetachPlugins()
        {
            while (_plugins.Count > 0)
            {
                var plugs = Plugins.ToArray();
                _plugins.Clear();

                foreach (var plug in plugs)
                    plug.Detach();
            }
        }

        public void Dispose()
        {
            try
            {
                DetachPlugins();

                MediaPlayer.Dispose();
                MediaPlayer = null;
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }
    }
}
