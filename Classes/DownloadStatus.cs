using MonoTorrent;
using MonoTorrent.Client;
using System.ComponentModel.DataAnnotations;

namespace TPBApi.Classes
{
    
    public class DownloadStatus
    {
        public InfoHash InfoHash { get; set; }
        public string? Name { get; set; }
        public double? Progress { get; set; }
        public int? Peers { get; set; }
        public long? DownloadSpeed { get; set; }  
        public string? State { get; set; }
        public Exception? Exception { get; set; }
        public Reason? Reason { get; set; }
    }

   

}
