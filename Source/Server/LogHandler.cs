using System;
using System.IO;
using System.Net;

namespace CommService
{
    /// <summary>
    /// Default log handler.
    /// Writes to stdout by default. Writes messages for every event
    /// in Verbose mode, otherwise only new and closed control connections.
    /// </summary>
    public class LogHandler : ILogHandler
    {
        private IPEndPoint peer;
        private TextWriter writer;
        private bool verbose;

        public LogHandler(TextWriter writer, bool verbose)
        {
            this.writer = writer;
            this.verbose = verbose;
        }

        public LogHandler(bool verbose) : this(Console.Out, verbose)
        {
        }

        public LogHandler() : this(false)
        {
        }

        private LogHandler(IPEndPoint peer, TextWriter writer, bool verbose)
        {
            this.peer = peer;
            this.writer = writer;
            this.verbose = verbose;
        }

        public ILogHandler Clone(IPEndPoint peer)
        {
            return new LogHandler(peer, writer, verbose);
        }

        private void Write(string format, params object[] args)
        {
            DateTime dt = DateTime.Now;
            string dtTime = string.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                (int)(dt.Hour), dt.Minute, dt.Second, dt.Millisecond);
            string now = dt.ToString("dd/MM/yyyy ") + dtTime;

            writer.WriteLine(String.Format("{0} -> {1}: {2}",
                now, peer, String.Format(format, args)));
            writer.Flush();
        }

        public void NewControlConnection()
        {
            Write("New control connection");
        }

        public void ClosedControlConnection()
        {
            Write("Closed control connection");
        }

        public void ReceivedCommand(string verb, string arguments)
        {
            if (verbose)
            {
                string argtext = (arguments == null || arguments == "" ? "" : ' ' + arguments);
                Write("Received command: {0}{1}", verb, argtext);
            }
        }

        public void SentResponse(uint code, string description)
        {
            if (verbose)
            {
                if (code == 211)
                {
                    string desc = description.Replace("\r\n", "");
                    Write("Sent response: {0} {1}", code, desc);
                }
                else
                {
                    Write("Sent response: {0} {1}", code, description);
                }
            }
        }

        public void NewDataConnection(IPEndPoint remote, IPEndPoint local, bool passive)
        {
            if (verbose)
            {
                Write("New data connection: {0} <-> {1} ({2})",
                    remote, local, (passive ? "Passive" : "Active"));
            }
        }

        public void ClosedDataConnection(IPEndPoint remote, IPEndPoint local, bool passive)
        {
            if (verbose)
            {
                Write("Closed data connection: {0} <-> {1} ({2})",
                    remote, local, (passive ? "Passive" : "Active"));
            }
        }
    }
}
