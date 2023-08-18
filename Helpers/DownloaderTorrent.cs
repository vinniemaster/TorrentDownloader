using MonoTorrent.Client;
using MonoTorrent;
using System.Net;
using TPBApi.Classes;

namespace TPBApi.Helpers
{
    public class DownloaderTorrent
    {
        Top10Listener Listener { get; }

        public DownloaderTorrent()
        {
            Listener = new Top10Listener(10);
        }

        private async Task DownloadAsync(MagnetLink magnet)
        {
            const int httpListeningPort = 55125;
            // Give an example of how settings can be modified for the engine.
            var settingBuilder = new EngineSettingsBuilder
            {          
                AllowPortForwarding = true,
                AutoSaveLoadDhtCache = true,
                AutoSaveLoadFastResume = true,             
                AutoSaveLoadMagnetLinkMetadata = true,
                //httpStreamingEndpoint = new Dictionary<string, IPEndPoint> {
                //    { "ipv4", new IPEndPoint (IPAddress.Any, 55123) },
                //    { "ipv6", new IPEndPoint (IPAddress.IPv6Any, 55123) }
                //},
                //DhtEndPoint = new IPEndPoint(IPAddress.Any, 55123),
                HttpStreamingPrefix = new Uri($"http://127.0.0.1:{httpListeningPort}")
            };
            var settingsBuilder = new TorrentSettingsBuilder
            {
                MaximumConnections = 60,
            };

            var dpath = "Downloads";

            if (!Directory.Exists(dpath))
                Directory.CreateDirectory(dpath);

            var downloadsPath = Path.Combine(Environment.CurrentDirectory, dpath);
            var engine = new ClientEngine(settingBuilder.ToSettings());
            
            
            await engine.AddAsync(magnet, downloadsPath, settingsBuilder.ToSettings());

            foreach (TorrentManager manager in engine.Torrents)
            {
                manager.PeerConnected += (o, e) => {
                    lock (Listener)
                        Listener.WriteLine($"Connection succeeded: {e.Peer.Uri}");
                };
                manager.ConnectionAttemptFailed += (o, e) => {
                    lock (Listener)
                        Listener.WriteLine(
                            $"Connection failed: {e.Peer.ConnectionUri} - {e.Reason}");
                };
                // Every time a piece is hashed, this is fired.
                manager.PieceHashed += delegate (object o, PieceHashedEventArgs e) {
                    lock (Listener)
                        Listener.WriteLine($"Piece Hashed: {e.PieceIndex} - {(e.HashPassed ? "Pass" : "Fail")}");
                };

                // Every time the state changes (Stopped -> Seeding -> Downloading -> Hashing) this is fired
                manager.TorrentStateChanged += delegate (object o, TorrentStateChangedEventArgs e) {
                    lock (Listener)
                        Listener.WriteLine($"OldState: {e.OldState} NewState: {e.NewState}");
                };

                // Every time the tracker's state changes, this is fired
                manager.TrackerManager.AnnounceComplete += (sender, e) => {
                    Listener.WriteLine($"{e.Successful}: {e.Tracker}");
                };
                await manager.StartAsync();
            }
            while (engine.IsRunning)
            {
                Console.WriteLine($"DownloadSpeed: {engine.TotalDownloadSpeed}");
                foreach(TorrentManager manager in engine.Torrents)
                {
                    var peers = await manager.GetPeersAsync();
                    Console.WriteLine($"Peers: {peers.Count()}");
                }
                await Task.Delay(500);
                Console.Clear();

                
            }


            
            }

        public async void TorrentDownload(string magnetUri)
        {
            MagnetLink magnet = MagnetLink.Parse(magnetUri);

            await DownloadAsync(magnet);
        }
    }
}
