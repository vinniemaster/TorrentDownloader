namespace TPBApi.Helpers
{
    public static class Helpers
    {
        public static string ConvertFileSize(int number)
        {
            var retorno = "";
            if (number.ToString().Length >= 13 && number.ToString().Length< 16)
            {
                retorno = ((((number / 1000) / 1000) / 1000) / 1000).ToString("0.00");
                retorno = retorno + " TB";
            }
            else if (number.ToString().Length>= 10 && number.ToString().Length< 13)
            {
                retorno = (((number / 1000) / 1000) / 1000).ToString("0.00");
                retorno = retorno + " GB";
            }
            else if (number.ToString().Length>= 7 && number.ToString().Length< 10)
            {
                retorno = ((number / 1000) / 1000).ToString("0.00");
                retorno = retorno + " MB";
            }
            else if (number.ToString().Length>= 4 && number.ToString().Length< 7)
            {
                retorno = (number / 1000).ToString("0.00");
                retorno = retorno + " KB";
            }
            else if (number.ToString().Length< 4)
            {
                retorno = number + " B";
            }

            return retorno;
        }
    }
}
