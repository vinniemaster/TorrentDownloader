using MonoTorrent.Client;
using MonoTorrent;
using MonoTorrent.BEncoding;

namespace TPBApi.Helpers
{
    public class Helpers
    {
        private async void TorrentDownload(string magnetUri)
        {
            string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TorrentDownloads");
            MagnetLink magnetLink = MagnetLink.Parse(magnetUri);
            var engine = new ClientEngine();
            await engine.AddAsync(magnetLink, "temp");


        }

        public void MonoTorrentDownload(string magnetUri)
        {
            TorrentDownload(magnetUri);
        }
    }
}
