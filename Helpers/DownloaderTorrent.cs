using MonoTorrent.Client;
using MonoTorrent;
using System.Net;
using TPBApi.Classes;
using System.Xml.Linq;
using System.IO;

namespace TPBApi.Helpers
{
    public class DownloaderTorrent
    {
        private static List<DownloadStatus> ActiveDownloads;
        private static ClientEngine Engine;

        public DownloaderTorrent()
        {       
        }

        private async Task DownloadAsync(MagnetLink magnet, string path)
        {

            const int httpListeningPort = 55125;

            var engineSettingBuilder = new EngineSettingsBuilder
            {          
                AllowPortForwarding = true,
                AutoSaveLoadDhtCache = true,
                AutoSaveLoadFastResume = true,             
                AutoSaveLoadMagnetLinkMetadata = true,
                HttpStreamingPrefix = new Uri($"http://127.0.0.1:{httpListeningPort}")
            };
            var torrentSettingsBuilder = new TorrentSettingsBuilder
            {
                MaximumConnections = 60,
            };

            

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var downloadsPath = Path.Combine(Environment.CurrentDirectory, path);

            Engine = new ClientEngine(engineSettingBuilder.ToSettings());                    
            await Engine.AddAsync(magnet, downloadsPath, torrentSettingsBuilder.ToSettings());
            foreach (TorrentManager manager in Engine.Torrents)
            {

                //manager.PeerConnected += (o, e) => {
                //    lock (Listener)
                //        Listener.WriteLine($"Connection succeeded: {e.Peer.Uri}");
                //};
                //manager.ConnectionAttemptFailed += (o, e) => {
                //    lock (Listener)
                //        Listener.WriteLine(
                //            $"Connection failed: {e.Peer.ConnectionUri} - {e.Reason}");
                //};
                //// Every time a piece is hashed, this is fired.
                //manager.PieceHashed += delegate (object o, PieceHashedEventArgs e) {
                //    lock (Listener)
                //        Listener.WriteLine($"Piece Hashed: {e.PieceIndex} - {(e.HashPassed ? "Pass" : "Fail")}");
                //};

                //// Every time the state changes (Stopped -> Seeding -> Downloading -> Hashing) this is fired
                //manager.TorrentStateChanged += delegate (object o, TorrentStateChangedEventArgs e) {
                //    lock (Listener)
                //        Listener.WriteLine($"OldState: {e.OldState} NewState: {e.NewState}");
                //};

                //// Every time the tracker's state changes, this is fired
                //manager.TrackerManager.AnnounceComplete += (sender, e) => {
                //    Listener.WriteLine($"{e.Successful}: {e.Tracker}");
                //};
                await manager.StartAsync();

                var peers = await manager.GetPeersAsync();

            }
            ActiveDownloads = new List<DownloadStatus>();
            while (Engine.IsRunning)
            {
                Console.WriteLine($"DownloadSpeed: {Engine.TotalDownloadSpeed}");
                foreach(TorrentManager manager in Engine.Torrents)
                {
                    var peers = await manager.GetPeersAsync();
                    Console.WriteLine($"Peers: {peers.Count()}");
                    DownloadStatus downloadActive = ActiveDownloads.FirstOrDefault(x => x.InfoHash == manager.InfoHash);
                    
                    if (downloadActive != null)
                    {
                        var indexDownload = ActiveDownloads.FindIndex(x => x.InfoHash == manager.InfoHash);
                        ActiveDownloads[indexDownload].Name = manager.MagnetLink.Name;
                        ActiveDownloads[indexDownload].Peers = peers.Count();
                        ActiveDownloads[indexDownload].DownloadSpeed = Engine.TotalDownloadSpeed;
                        ActiveDownloads[indexDownload].State = manager.State.ToString();
                        ActiveDownloads[indexDownload].Progress = manager.Progress;
                    }
                    else
                    {
                        ActiveDownloads.Add(new DownloadStatus()
                        {
                            InfoHash = manager.InfoHash
                        });
                    }  

                }
                await Task.Delay(1000);
                Console.Clear();               
            }

            foreach (var manager in Engine.Torrents)
            {
                await manager.StopAsync();
            }


        }

        public async void TorrentDownload(string magnetUri, string path)
        {
            MagnetLink magnet = MagnetLink.Parse(magnetUri);

            await DownloadAsync(magnet, path);
        }

        public List<DownloadStatus> getDownloads()
        {
            
            var activeDownload = ActiveDownloads;
            return activeDownload;
        }
    }
}