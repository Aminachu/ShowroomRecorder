using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SQLite;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using System.Drawing;
using System.Net;
using System.ComponentModel;

namespace ShowroomRecorder
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public List<string> proc = new List<string>();

        private static string basepath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        private static string dbpath = System.IO.Path.Combine(basepath, "data.db");
        internal static ShowroomRecorder.MainWindow mw;
        private dynamic data;
        public byte[] prgfx;

        public MainWindow()
        {
            InitializeComponent();
            mw = this;
            proclist.ItemsSource = proc;
            List<string> roomIDS = LoadData();
            Task<bool> StartForm = updateAsync(roomIDS);
        }

        public List<string> LoadData()
        {
            List<string> roomIDS = new List<string>();

            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + dbpath + ";"))
            {
                conn.Open();

                SQLiteCommand command = new SQLiteCommand("Select * from showroom_rooms ORDER BY alias ASC", conn);
                SQLiteDataReader reader = command.ExecuteReader();

                prog.Maximum = reader.StepCount;

                while (reader.Read())
                {
                    roomIDS.Add(reader["roomid"].ToString());
                }

                reader.Close();
            }

            List<ShowroomItem> temp = new List<ShowroomItem>();

            foreach (string roomid in roomIDS)
            {
                Int32 id = 0;
                Int32.TryParse(roomid, out id);
                ShowroomItem si = new ShowroomItem(id);

                temp.Add(si);
            }
            foreach (ShowroomItem showroomItem in temp)
            {
                roomlist.Children.Add(showroomItem);
            }

            return roomIDS;
        }

        private async Task<bool> updateAsync(List<string> roomIDS)
        {
            bool didIt = false;

            foreach (string id in roomIDS)
            {
                byte[] imagees;

                using (WebClient webClient = new WebClient())
                {
                    var json = webClient.DownloadString("https://www.showroom-live.com/api/room/profile?room_id=" + id);

                    data = JsonConvert.DeserializeObject(json);
                    string url = data.image;
                    byte[] imageBytes = webClient.DownloadData(url);
                    imagees = imageBytes;

                    rooms.Items.Add(data.room_url_key);
                }
                using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + dbpath + ";"))
                {
                    conn.Open();

                    SQLiteCommand cmd = new SQLiteCommand("UPDATE showroom_rooms SET handle = '" + data.room_url_key.ToString() + "', profilegfx = @blob WHERE roomid = '" + data.room_id.ToString() + "'", conn);
                    cmd.Parameters.Add(new SQLiteParameter("@blob", System.Data.DbType.Binary));
                    SQLiteParameter parameter = new SQLiteParameter("@blob", System.Data.DbType.Binary);
                    parameter.Value = imagees;
                    cmd.Parameters.Add(parameter);

                    conn.Close();
                    didIt = true;
                }
            }
            return didIt;
        }

        public void getNewShowroom(string ID)
        {
            byte[] imagees;

            using (WebClient webClient = new WebClient())
            {
                var json = webClient.DownloadString("https://www.showroom-live.com/api/room/profile?room_id=" + ID);

                data = JsonConvert.DeserializeObject(json);
                string url = data.image;
                byte[] imageBytes = webClient.DownloadData(url);
                imagees = imageBytes;

                rooms.Items.Add(data.room_url_key);
            }
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + dbpath + ";"))
            {
                conn.Open();

                SQLiteCommand cmd = new SQLiteCommand("INSERT INTO showroom_rooms SET handle = '" + data.room_url_key.ToString() + "', profilegfx = @blob WHERE roomid = '" + data.room_id.ToString() + "'", conn);
                cmd.Parameters.Add(new SQLiteParameter("@blob", System.Data.DbType.Binary));
                SQLiteParameter parameter = new SQLiteParameter("@blob", System.Data.DbType.Binary);
                parameter.Value = imagees;
                cmd.Parameters.Add(parameter);

                conn.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (!roomIsLive(Int32.Parse(roomID.Text)))
            {
                MessageBox.Show("Raum offline");
            }
            else
            {
                MessageBox.Show("Raum online");
            }

            roomData(Int32.Parse(roomID.Text));

        }

        #region Unbenutzt

        private bool roomIsLive(int id)
        {
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString("https://www.showroom-live.com/room/is_live?room_id=" + id);

                dynamic stuff = JsonConvert.DeserializeObject(json);

                int ok = stuff.ok;

                if (ok == 0)
                {
                    return false;
                }
                else
                {

                    return true;
                }
            }
        }

        public void refreshList()
        {
            proclist.Items.Refresh();
        }

        private void roomData(int id) // Testfunktion
        {

            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString("https://www.showroom-live.com/api/room/profile?room_id=" + id);

                dynamic stuff = JsonConvert.DeserializeObject(json);

                var streamurl = webClient.DownloadString("https://www.showroom-live.com/api/live/streaming_url?room_id=" + id);

                dynamic url = JsonConvert.DeserializeObject(streamurl);

                htmlTxt.Text = stuff.room_url_key;
                if (roomIsLive(id) == false)
                    return;
            }

        }

        #endregion
        #region In Progress

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;

            worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {

            //(sender as BackgroundWorker).ReportProgress();


        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            prog.Value = e.ProgressPercentage;
        }
    }
    #endregion

    public class ShowroomArgs : EventArgs
    {
        public string ID { get; set; }

    }
}
