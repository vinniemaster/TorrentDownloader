namespace TPBApi.Classes
{
    public class TorrentProps
    {
        public string name { get; set; }
        public string size { get; set; }
        public string seeders { get; set; }
        public string leechers { get; set; }
        public string category { get; set; }
        public string hash { get; set; }
        public string magnet { get; set; }
        public string torrent { get; set; }
        public string url { get; set; }
        public string date { get; set; }
        public string downloads { get; set; }
    }

    public class TorrentPY
    {
        public List<TorrentProps> data { get; set; }
        public int current_page { get; set; }
        public int total_pages { get; set; }
        public double time { get; set; }
        public int total { get; set; }
    }
}
