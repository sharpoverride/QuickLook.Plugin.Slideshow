using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using QuickLook.Common.Plugin;

namespace QuickLook.Plugin.Slideshow
{
    public class Plugin : IViewer
    {
        private SlideshowPanel _viewer;
        private List<string> _entries;

        public int Priority => 0;

        public void Init()
        {
        }

        public bool CanHandle(string path)
        {
            return Directory.Exists(path) &&
                File.GetAttributes(path).HasFlag(FileAttributes.Directory);
        }

        public void Prepare(string path, ContextObject context)
        {
            context.PreferredSize = new Size { Width = 600, Height = 400 };
        }

        public void View(string path, ContextObject context)
        {
            _viewer = new SlideshowPanel();

            context.ViewerContent = _viewer;
            context.Title = $"{Path.GetFileName(path)}";

            _entries = new List<string>(
                Directory.GetFiles(path, "*.*", SearchOption.AllDirectories));
            _viewer.ShowEntriesFound(_entries);

            _viewer.StartSlideshow += () =>
            {
                var cts = new CancellationTokenSource();
                var token = cts.Token;

                int numberOfSeconds = _viewer.NumberOfSeconds * 1000;
                Task.Run(async () =>
                {
                    foreach (var entry in _entries)
                    {
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }
                        PipeServerManager.SendMessage(
                            PipeMessages.Switch, entry);

                        await Task.Delay(numberOfSeconds);
                    };
                });

                Task.Run(() =>
                {
                    var PipeName = "QuickLook.App.Pipe." + WindowsIdentity.GetCurrent().User?.Value;

                    var _server = new NamedPipeServerStream(PipeName, PipeDirection.In, 2);

                    using (var reader = new StreamReader(_server))
                    {
                        while (true)
                        {
                            _server.WaitForConnection();
                            var msg = reader.ReadLine();

                            _server.Disconnect();
                            if(msg.IndexOf("Close") > -1)
                            {
                                _server.Dispose();
                                cts.Cancel();
                                break;
                            }

                        }
                    }
                });
            };

            context.IsBusy = false;
        }

        public void Cleanup()
        {
            if (_viewer == null)
                return;

            _viewer = null;
        }
    }
}