using System.Net.Http;
using System.Drawing;
using faabBot.GUI.Helpers;
using System.Drawing.Imaging;
using System;
using System.Text;

namespace faabBot.GUI.Controllers
{
    public class HttpClientController
    {
        private readonly HttpClient _httpClient;
        private readonly MainWindow _mainWindow;

        public HttpClientController(MainWindow window)
        {
            _httpClient = new();
            _mainWindow = window;
        }

        public void DownloadImage(string src, string? fileName, string? subImageDirectory, int index)
        {
            if (fileName == null)
            {
                Guid g = Guid.NewGuid();
                fileName = g.ToString();
            }

            fileName = RemoveSpecialCharacters(fileName);

            subImageDirectory ??= DirectoryHelper.CreateSubImageDirectory(_mainWindow, _mainWindow.LogInstance);

            var req = _httpClient.GetAsync(src).ContinueWith(res =>
            {
                var result = res.Result;
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var readData = result.Content.ReadAsStreamAsync();
                    readData.Wait();

                    var readStream = readData.Result;
                    var image = Image.FromStream(readStream);
                    image.Save(string.Format("{0}{1}", subImageDirectory + "\\", index + " " + fileName + ".Jpeg"), ImageFormat.Jpeg);
                }
            });
        }

        private static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public void CloseHttpClient()
        {
            _httpClient.Dispose();
        }
    }
}
