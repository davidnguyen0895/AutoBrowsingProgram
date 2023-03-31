using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text;
using System.Net.Http.Headers;

namespace GoogleSelenium
{
    static class GoogleSelenium
    {
        //アクセストークン
        static readonly string ACCESS_TOKEN = "BIaGHwnk3ZalpvYNJy0GO6Lp4mV0WCrdaf1yCgqTbJN";
        //URL
        static readonly string LINE_URL = "https://notify-api.line.me/api/notify";
        static readonly string baitoruHost = "https://baitoru.com";
        static readonly string folder = "C:\\Users\\thinh\\DATA\\SeleniumOutput\\";
        static readonly string fileName = $"Result_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
        [STAThread]
        private static void Main()
        {
            //ファイル名
            string fullPath = string.Concat(folder, fileName);
            bool exists = System.IO.Directory.Exists(folder);

            if (!exists)
            {
                System.IO.Directory.CreateDirectory(folder);
            }

            bool chromeBrowserHidden = true;

            var notIncludedStr = new List<string>
                                                        {
                                                            "新規登録",
                                                            "関東\r\n変更",
                                                            "すべての求人",
                                                            "キープリスト",
                                                            "お知らせ",
                                                            "沿線・駅から選ぶ",
                                                            "給与区分\r\nこだわらない\r\n時給\r\n日給\r\n月給\r\n年俸\r\n完全出来高制",
                                                            "追加・変更\r\n長期",
                                                            "この条件にヒットした仕事",
                                                            "アルバイト・パート",
                                                            "キープする",
                                                            "動画あり",
                                                            "低い",
                                                            "男性",
                                                            "一人で",
                                                            "しずか",
                                                            "キープする",
                                                            "派遣\r\n社員登用あり\r\n動画あり\r\n給与UP"
                                                        };

            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            //オプション設定(非表示にしたり、Userを選択したり)
            ChromeOptions options = new();
            if (chromeBrowserHidden)
            {
                options.AddArgument("--headless");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--window-position=-32000,-32000");
                options.AddArgument("--user-agent=NGUYEN");
            }
            else
            {
                options.AddArgument("--start-maximized");   //最大化
            }
            // Initialize the ChromeDriver
            IWebDriver driver = new ChromeDriver(service, options);
            //URLにアクセス
            driver.Navigate().GoToUrl("https://www.baitoru.com/kanto/jlist/kanagawa/kawasakishi/lightwork-business-office-education-medicalcare-clean-housekeeper-disinfection-guestroomclean/mrt4-mrt34-mrt5/trm5/nsu1/stp4-sly3/btp1/nkm12/");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            // locate the search result links and extract their URLs
            IList<IWebElement> links = driver.FindElements(By.ClassName("li01"));
            StringBuilder outputStr = new();
            string finalLink = "";

            outputStr.AppendLine("みぴ様 (｡･ω･｡)ﾉ♡");
            outputStr.AppendLine("以下のバイト情報をミチュけましたお(⋈◍＞◡＜◍)。✧♡");
            foreach (IWebElement link in links)
            {
                if (!string.IsNullOrEmpty(link.Text))
                {
                    if (!notIncludedStr.Contains(link.Text))
                    {
                        //テキストを書き込む
                        string hrefStr = link.GetAttribute("innerHTML");
                        string text = link.Text;
                        //outputStr.AppendLine($"Href：{hrefStr}");
                        outputStr.AppendLine(Environment.NewLine);
                        if (hrefStr.Contains("\""))
                        {
                            string linkPrm = link.GetAttribute("innerHTML").Split("\"")[1];
                            string prm1 = linkPrm.Split("?")[0];
                            finalLink = $"{baitoruHost}{prm1}";
                            if (prm1.Contains("/"))
                            {
                                outputStr.AppendLine($"【タイトル】{text}");
                                //outputStr.AppendLine($"Parameter：{prm1}");
                                outputStr.AppendLine($"【リンク】\r\n{finalLink}");
                            }
                        }
                    }
                }
            }
            //driver.Navigate().GoToUrl(finalLink);
            //IList<IWebElement> elements = driver.FindElements(By.TagName("em"));
            //if (elements != null)
            //{
            //    foreach (IWebElement element in elements)
            //    {
            //        if (!string.IsNullOrEmpty(element.Text))
            //        {
            //            outputStr.AppendLine(Environment.NewLine);
            //            outputStr.AppendLine(element.Text);
            //        }
            //    }
            //}
            //ファイルをオープンする
            using (StreamWriter sw = new(fullPath, false, Encoding.GetEncoding("UTF-8")))
            {
                sw.Write(outputStr.ToString());
                sw.Dispose();
            }

            if (driver != null)
            {
                driver.Quit();      //すべて閉じる
                driver.Dispose();
            }

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = folder,
                UseShellExecute = true,
                Verb = "open"
            });

            SendMessageByLineNotify("みぴたんおぱよう❣");
            Console.ReadKey();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private static async void SendMessageByLineNotify(string message)
        {
            using var client = new HttpClient();
            // 通知するメッセージ
            var content = new FormUrlEncodedContent(new Dictionary<string, string> { { "message", message } });
            // ヘッダーにアクセストークンを追加
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ACCESS_TOKEN);
            // 実行
            var result = await client.PostAsync(LINE_URL, content);
            //Console.WriteLine(result);
            result.Dispose();
            client.Dispose();
        }
    }
}