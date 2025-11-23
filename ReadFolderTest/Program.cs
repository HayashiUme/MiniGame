using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

class Program
{
    private static readonly List<string> ImagePaths = new List<string>();
    public static string Https = "https://api.lolicon.app/setu/v2";

    public static bool CheckUrl()
    {
        Console.WriteLine("输入你想下载的API，若API不合法，将自动替换为：" + Https);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("注意！一定要使用完整API，如https://www.网站名称.com");
        Console.ForegroundColor = ConsoleColor.White;
        string newHttps = Console.ReadLine();

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(newHttps);
        request.Method = "HEAD";
        request.Timeout = 10000; // 设置超时时间为10秒
        request.AllowAutoRedirect = false; // 禁止自动重定向

        try
        {
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                return response.StatusCode == HttpStatusCode.OK;
            }
        }
        catch (WebException)
        {
            return false;
        }
    }
    public static async Task Main()
    {
        CheckUrl();
        bool IsTure = CheckUrl();
        if (IsTure)
        {
            Console.WriteLine("输入你想生成的图片个数");
            var num = Console.ReadLine();
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string storageDir = Path.Combine(desktopPath, $"ACG {timestamp}");

            Directory.CreateDirectory(storageDir);
            Console.WriteLine($"创建了文件夹: {storageDir}");

            // 下载图片
            using (HttpClient client = new HttpClient())
            {
                for (int i = 0; i < int.Parse(num); i++)
                {
                    try
                    {
                        byte[] imageData = await client.GetByteArrayAsync(Https);
                        string fileName = $"image_{(i + 1)}.png";
                        string filePath = Path.Combine(storageDir, fileName);

                        if (!System.IO.File.Exists(filePath))
                        {
                            // 使用FileStream异步写入
                            using (FileStream fs = new FileStream(
                                filePath,
                                FileMode.CreateNew,
                                FileAccess.Write,
                                FileShare.None,
                                bufferSize: 4096,
                                useAsync: true))
                            {
                                await fs.WriteAsync(imageData, 0, imageData.Length).ConfigureAwait(false);
                            }

                            ImagePaths.Add(filePath);
                            Console.WriteLine($"正在下载: {filePath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"下载失败：{i + 1}: {ex.Message}");
                    }
                }
            }

        }
    }
}