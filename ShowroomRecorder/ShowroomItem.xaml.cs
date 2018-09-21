using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ShowroomRecorder
{
    /// <summary>
    /// Interaktionslogik für ShowroomItem.xaml
    /// </summary>
    public partial class ShowroomItem : UserControl
    {
        private static string basepath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        private static string dbpath = System.IO.Path.Combine(basepath, "data.db");
        private dynamic data;
        private Thread buildInTime;


        private int roomid;
        private byte[] image;
        private string alias;
        private string handle;

        private bool isRecording = false;

        public ShowroomItem()
        {
            InitializeComponent();
        }

        public ShowroomItem(int _roomid)
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(20);
            timer.Tick += bgw;
            timer.Start();

            this.roomid = _roomid;

            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + dbpath + ";"))
            {
                conn.Open();

                SQLiteCommand command = new SQLiteCommand("Select * from showroom_rooms WHERE roomid = '" + roomid + "'", conn);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                    byte[] erg = (byte[])reader["profilegfx"];

                    MemoryStream stream = new MemoryStream(erg);

                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.EndInit();
                    Dispatcher.Invoke(new Action(() =>
                    {
                        this.uc_alias.Content = reader["alias"].ToString();
                        this.uc_handle.Content = reader["handle"].ToString();
                        this.uc_image.Source = image;
                    }));

                }
                reader.Close();
                conn.Close();
                command.Dispose();
            }

            //buildInTime = new Thread(buildItemAsync);
            //buildInTime.IsBackground = true;
            //buildInTime.Start();
        }

        private void bgw(object sender, EventArgs e)
        {

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += roomIsLive;
            //worker.ProgressChanged += worker_ProgressChanged;
            //worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();

        }

        private void buildItemAsync()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + dbpath + ";"))
            {
                conn.Open();

                SQLiteCommand command = new SQLiteCommand("Select * from showroom_rooms WHERE roomid = '" + roomid + "'", conn);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    byte[] erg = (byte[])reader["profilegfx"];

                    MemoryStream stream = new MemoryStream(erg);

                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.EndInit();
                    Dispatcher.Invoke(new Action(() =>
                    {
                        this.uc_alias.Content = reader["alias"].ToString();
                        this.uc_handle.Content = reader["handle"].ToString();
                        this.uc_image.Source = image;
                    }));

                }
                reader.Close();
                conn.Close();
                command.Dispose();
            }
        }

        private void roomIsLive(object sender, EventArgs e)
        {

            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString("https://www.showroom-live.com/room/is_live?room_id=" + roomid);

                dynamic stuff = JsonConvert.DeserializeObject(json);

                var json2 = webClient.DownloadString("https://www.showroom-live.com/api/room/profile?room_id=" + roomid);

                dynamic data = JsonConvert.DeserializeObject(json2);

                var streamurl = webClient.DownloadString("https://www.showroom-live.com/api/live/streaming_url?room_id=" + roomid);

                dynamic url = JsonConvert.DeserializeObject(streamurl);

                int ok = stuff.ok;

                if (ok == 0)
                {

                    Dispatcher.Invoke(new Action(() =>
                    {
                        uc_bg.Fill = new SolidColorBrush(System.Windows.Media.Colors.Gray);

                        uc_livesince.Visibility = Visibility.Hidden;


                    }));
                }
                else
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        // Unix timestamp is seconds past epoch
                        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

                        int secs = Int32.Parse(data.current_live_started_at.ToString());

                        dtDateTime = dtDateTime.AddSeconds(secs).ToLocalTime();

                        uc_livesince.Content = String.Format("live since: {0}",
                             dtDateTime.ToString("dd,MM yyyy - HH:mm:ss"));

                        uc_livesince.Visibility = Visibility.Visible;

                        uc_bg.Fill = new SolidColorBrush(System.Windows.Media.Colors.LightPink);
                    }));
                    if (isRecording == false)
                    {
                        Livestream ls = new Livestream(data.room_url_key.ToString(), url.streaming_url_list[1].url.ToString(), basepath + @"\Recordings\" + data.room_url_key.ToString() + "\\");
                        isRecording = true;

                        ls.RecordingStarted += OnRecordingStarted;
                        ls.RecordingEnded += OnRecordingEnded;

                        ls.Start();
                    }

                }

            }

        }

        public void OnRecordingStarted(object source, ShowroomArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                MainWindow.mw.proc.Add(e.ID);
                MainWindow.mw.proclist.Items.Refresh();
            }));
        }

        public void OnRecordingEnded(object source, ShowroomArgs e)
        {
            this.isRecording = false;

            Dispatcher.Invoke(new Action(() =>
            {
                MainWindow.mw.proc.Remove(e.ID);
                MainWindow.mw.proclist.Items.Refresh();
            }));
        }
    }
}


