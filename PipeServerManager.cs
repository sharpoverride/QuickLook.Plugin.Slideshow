// Copyright © 2017 Paddy Xu
// 
// This file is part of QuickLook program.
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Windows.Threading;

namespace QuickLook
{
    internal static class PipeMessages
    {
        public const string RunAndClose = "QuickLook.App.PipeMessages.RunAndClose";
        public const string Switch = "QuickLook.App.PipeMessages.Switch";
        public const string Invoke = "QuickLook.App.PipeMessages.Invoke";
        public const string Toggle = "QuickLook.App.PipeMessages.Toggle";
        public const string Forget = "QuickLook.App.PipeMessages.Forget";
        public const string Close = "QuickLook.App.PipeMessages.Close";
        public const string Quit = "QuickLook.App.PipeMessages.Quit";
    }

    internal class PipeServerManager : IDisposable
    {
        private static readonly string PipeName = "QuickLook.App.Pipe." + WindowsIdentity.GetCurrent().User?.Value;
        private static PipeServerManager _instance;

        private DispatcherOperation _lastOperation;

        private NamedPipeServerStream _server;

        public PipeServerManager()
        {
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (_server != null)
                SendMessage(PipeMessages.Quit);
            _server?.Dispose();
            _server = null;
        }

        public static void SendMessage(string pipeMessage, string path = null)
        {
            if (path == null)
                path = "";

            try
            {
                using (var client = new NamedPipeClientStream(".", PipeName, PipeDirection.Out))
                {
                    client.Connect();

                    using (var writer = new StreamWriter(client))
                    {
                        writer.WriteLine($"{pipeMessage}|{path}");
                        writer.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }
    }
}