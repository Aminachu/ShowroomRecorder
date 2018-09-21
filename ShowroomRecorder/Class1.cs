using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowroomRecorder
{
    class Livestream
    {

        private string ID;
        private string URL;
        private string Pfad;
        private Process proc;

        public Livestream(string id, string url, string pfad)
        {
            ID = id;
            URL = url;
            Pfad = pfad;
        }

        public event EventHandler<ShowroomArgs> RecordingStarted;

        public event EventHandler<ShowroomArgs> RecordingEnded;

        public void Start()
        {
            bool test = Directory.Exists(System.IO.Path.GetDirectoryName(Pfad));

            if (!test)
            {
                Directory.CreateDirectory(Pfad);
            }
            string now = DateTime.Now.ToString("dd_MM_yyyy-HH_mm");
            proc = new Process();
            proc.StartInfo.FileName = @"Streamlink\streamlink.exe";
            // proc.StartInfo.Arguments = "--twitch-oauth-token 64mcnqf3ja8d03bbd0pfn679u3ihda twitch.tv/bakacrew best";
            proc.StartInfo.Arguments = "--hls-segment-threads 3 --hls-segment-timeout 10 --hls-playlist-reload-attempts 3 --http-timeout 1.5 -o " + Pfad  + ID + "_" + now + ".mp4 \"hlsvariant://" + URL + "\" best";
            proc.StartInfo.CreateNoWindow = false;
            proc.EnableRaisingEvents = true;
            proc.Exited += new EventHandler(proc_exited);
            proc.Start();

            //  MainWindow.mw.proc.Add(ID);

            OnRecordingStarted(ID);

        }

        protected virtual void OnRecordingStarted(string id)
        {
            if (RecordingStarted != null)
                RecordingStarted(this, new ShowroomArgs() { ID = id });
        }

        protected virtual void OnRecordingEnded(string id)
        {
            if (RecordingEnded != null)
                RecordingEnded(this, new ShowroomArgs() { ID = id });
        }

        private void proc_exited(object source, EventArgs e)
        {
            OnRecordingEnded(ID);
        }

    }

}
