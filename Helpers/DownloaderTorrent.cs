using MonoTorrent.Client;
using MonoTorrent;
using System.Net;
using TPBApi.Classes;
using System.Xml.Linq;
using System.IO;
using System;
using System.Runtime.CompilerServices;

namespace TPBApi.Helpers
{
    public class DownloaderTorrent
    {
        private static List<DownloadStatus> ActiveDownloads = new List<DownloadStatus>();
        private static EngineSettingsBuilder engineSettingBuilder = new EngineSettingsBuilder
        {
            AllowPortForwarding = true,
            AutoSaveLoadDhtCache = true,
            AutoSaveLoadFastResume = true,
            AutoSaveLoadMagnetLinkMetadata = true,
            HttpStreamingPrefix = new Uri($"http://127.0.0.1:55125")
        };
        private static ClientEngine Engine = new ClientEngine(engineSettingBuilder.ToSettings());
        private List<string> log = new List<string>();
        public DownloaderTorrent()
        {
        }

        private async Task DownloadAsync(MagnetLink magnet, string path)
        {

            var torrentSettingsBuilder = new TorrentSettingsBuilder
            {
                MaximumConnections = 60,
            };
          
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var downloadsPath = Path.Combine(Environment.CurrentDirectory, path);
            
            await Engine.AddAsync(magnet, downloadsPath, torrentSettingsBuilder.ToSettings());

            foreach (TorrentManager manager in Engine.Torrents)
            {

                //manager.PeerConnected += (o, e) =>
                //{
                //    lock (Listener)
                //        Listener.WriteLine($"Connection succeeded: {e.Peer.Uri}");
                //};
                //manager.ConnectionAttemptFailed += (o, e) =>
                //{
                //    lock (Listener)
                //        Listener.WriteLine(
                //            $"Connection failed: {e.Peer.ConnectionUri} - {e.Reason}");
                //};
                //// Every time a piece is hashed, this is fired.
                //manager.PieceHashed += delegate (object o, PieceHashedEventArgs e)
                //{
                //    lock (Listener)
                //        Listener.WriteLine($"Piece Hashed: {e.PieceIndex} - {(e.HashPassed ? "Pass" : "Fail")}");
                //};

                //// Every time the state changes (Stopped -> Seeding -> Downloading -> Hashing) this is fired
                //manager.TorrentStateChanged += delegate (object o, TorrentStateChangedEventArgs e)
                //{
                //    lock (Listener)
                //        Listener.WriteLine($"OldState: {e.OldState} NewState: {e.NewState}");
                //};

                //// Every time the tracker's state changes, this is fired
                //manager.TrackerManager.AnnounceComplete += (sender, e) =>
                //{
                //    Listener.WriteLine($"{e.Successful}: {e.Tracker}");
                //};
                await manager.StartAsync();

            }
            
            try
            {
                while (Engine.IsRunning)
                {
                    //Console.WriteLine($"DownloadSpeed: {Engine.TotalDownloadSpeed}");
                    foreach (TorrentManager manager in Engine.Torrents)
                    {
                        var peers = await manager.GetPeersAsync();
                        //Console.WriteLine($"Peers: {peers.Count()}");
                        DownloadStatus downloadActive = ActiveDownloads.FirstOrDefault(x => x.InfoHash == manager.InfoHash);

                        if (downloadActive != null)
                        {
                            var indexDownload = ActiveDownloads.FindIndex(x => x.InfoHash == manager.InfoHash);
                            ActiveDownloads[indexDownload].Index = indexDownload;
                            ActiveDownloads[indexDownload].Name = manager.MagnetLink.Name;
                            ActiveDownloads[indexDownload].Peers = peers.Count();
                            ActiveDownloads[indexDownload].DownloadSpeed = manager.Monitor.DownloadSpeed;
                            ActiveDownloads[indexDownload].State = manager.State.ToString();
                            ActiveDownloads[indexDownload].Progress = manager.Progress;
                            if (manager.Error != null)
                            {
                                ActiveDownloads[indexDownload].Exception = manager.Error.Exception;
                                ActiveDownloads[indexDownload].Reason = manager.Error.Reason;
                                log.Add($"ERRO {manager.Error.Exception}, {manager.Error.Reason}");
                            }
                            //log.Add($"{manager.MagnetLink.Name},{peers.Count()},{Engine.TotalDownloadSpeed},{manager.State.ToString()}, {manager.Progress}");

                            //if (manager.Progress == 100)
                            //{
                            //    StreamWriter txt = new StreamWriter("log"+ manager.MagnetLink.Name+".txt");
                            //    foreach (var item in log)
                            //    {
                            //        txt.Write(item.ToString() + "\n");
                            //    }
                            //    txt.Close();


                            //}
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
                    //Console.Clear();
                }
            }
            catch(Exception ex)
            {
                StreamWriter txt = new StreamWriter("ErrorLog.txt");
                txt.Write(ex.ToString() + "\n");
                txt.Write($"{magnet.Name},{magnet.Size},{downloadsPath}");
                txt.Close();
            }


        }

        public async Task<Response<string>> TorrentDownload(string magnetUri, string path)
        {
            MagnetLink magnet = MagnetLink.Parse(magnetUri);

            try
            {
                await DownloadAsync(magnet, path);
            }
            catch(Exception ex)
            {
                return new Response<string>() { Message = "ERRO", Data = ex.ToString() };
            }

            return new Response<string>() { Message = "OK" };
        }

        public List<DownloadStatus> getDownloads()
        {
            
            var activeDownload = ActiveDownloads;
            return activeDownload;
        }

        public void pauseDownload(int index)
        {
            var ActiveDownloadList = ActiveDownloads[index];

            var downloadActive = Engine.Torrents.Where(x => x.InfoHash == ActiveDownloadList.InfoHash).FirstOrDefault();

            downloadActive.PauseAsync();

        }

        public void retomarDownload(int index)
        {
            var ActiveDownloadList = ActiveDownloads[index];

            var downloadActive = Engine.Torrents.Where(x => x.InfoHash == ActiveDownloadList.InfoHash).FirstOrDefault();

            downloadActive.StartAsync();

        }



        public async Task pararDownload(int index)
        {

            var ActiveDownloadList = ActiveDownloads[index];

            var downloadActive = Engine.Torrents.Where(x => x.InfoHash == ActiveDownloadList.InfoHash).FirstOrDefault();


            var stoppingTask = downloadActive.StopAsync();
            while (downloadActive.State != TorrentState.Stopped)
            {
                ActiveDownloads[index].State = downloadActive.State.ToString();
            }

            await stoppingTask;
        }

        public async Task excluirDownload(int index)
        {
            var ActiveDownloadList = ActiveDownloads[index];

            var downloadActive = Engine.Torrents.Where(x => x.InfoHash == ActiveDownloadList.InfoHash).FirstOrDefault();
            
            var stoppingTask = downloadActive.StopAsync();
            while (downloadActive.State != TorrentState.Stopped)
            {
                ActiveDownloads[index].State = downloadActive.State.ToString();
            }

            await stoppingTask;

            await Engine.RemoveAsync(downloadActive);
            ActiveDownloads.Remove(ActiveDownloadList);

        }
    }
}