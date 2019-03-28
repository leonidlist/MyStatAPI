using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyStatAPI.Full
{
    #region CityEnum
    public enum Cities
    {
        Dnipro, Lviv, Kharkiv = 3, Poltava, Mariupil,
        Odessa, Donetsk, Kyiv, Vinnitca, Zaporizhya,
        Nicolaev, Rivne, Lutsk, Simferopil = 15, Chernihiv,
        Minsk, Astana, Gomil, RioDeZhaneiro, Kishinev = 23,
        Moscow, Bucharest, Sofia, Tbilisi = 28, Almaty, Kherson = 31,
        Khmelnick, Ternopil, KriviyRig, Pnompen, Seattle, Karaganda,
        Zhitomir = 39, Kamyanske, StPetersburg, Baku, Bratislava, Prague,
        Bobruisk, Chernivtsi, Tijuana, Warsaw, Geneve = 50, Rostov, Novgorod,
        Voronezh, IvanoFrankivsk, Tula, Krasnodar, Novosibirsk, Brno,
        Grodno, Mogilev, Vitebsk, Brest, Uzhgorod, Cherkasy, MoscowMar,
        MoscowVoy, MoscowBel, Kramatorsk, Telavi, Tashkent,
        Yekaterinburh, Chelyabinsk, Omsk, Krasnoyarsk, Perm, Kazan,
        Samara, Ufa, Volgograd, Yaroslavl
    }
    #endregion
    public class Api
    {
        #region Properties
        public string ApplicationKey { get; private set; } = "6a56a5df2667e65aab73ce76d1dd737f7d1faef9c52e8b8c55ac75f565d8e8a6";
        private string LoginUrl { get; set; } = @"https://msapi.itstep.org/api/v1/auth/login";
        private string UserInfoUrl { get; set; } = @"https://msapi.itstep.org/api/v1/settings/user-info";
        private string LatestNewsUrl { get; set; } = @"https://msapi.itstep.org/api/v1/news/operations/latest-news";
        private string UserActivitiesUrl { get; set; } = @"https://msapi.itstep.org/api/v1/dashboard/progress/activity";
        private string GroupInfoUrl { get; set; } = @"https://msapi.itstep.org/api/v1/dashboard/progress/leader-group";
        private string DailyPointsUrl { get; set; } = @"https://msapi.itstep.org/api/v1/feedback/students/comment-academy-day";
        private string EvaluationIdUrl { get; set; } = @"https://msapi.itstep.org/api/v1/feedback/students/evaluate-academy-day?evaluation=2U8ftB5OCmgrsK7c76pKE1TXHB_stVr4";
        private string ScheduleUrl { get; set; } = $@"https://msapi.itstep.org/api/v1/schedule/operations/get-month?date_filter={DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}";
        private string HomeworksUrl { get; set; } = @"https://msapi.itstep.org/api/v1/homework/operations/list?page=1";
        public string Username { get; set; }
        public string Password { private get; set; }
        public Cities City { get; set; }
        public string AccessToken { get; private set; }
        public UserEntity CurrentUser { get; private set; }
        public List<NewsEntity> LatestNews { get; private set; }
        public List<UserActivityEntity> CurrentUserActivities { get; private set; }
        public List<GroupUserEntity> GroupUsers { get; private set; }
        public List<ScheduleEntity> Schedule { get; private set; }
        public List<HomeworkEntity> Homeworks { get; private set; }
        #endregion

        #region .ctor
        public Api() { }
        public Api(string username, string password, Cities city)
        {
            Username = username;
            Password = password;
            City = city;
        }
        #endregion

        #region AsyncWrap
        public async Task TryLoginAsync()
        {
            await Task.Run(() => TryLogin());
        }

        public async Task CollectDailyPointsAsync()
        {
            await Task.Run(() => CollectDailyPoints());
        }

        public async Task DownloadHomeworkFileAsync(HomeworkEntity homework, string downloadPath)
        {
            await Task.Run(() => DownloadHomeworkFile(homework, downloadPath));
        }

        public async Task UploadHomeworkFileAsync(HomeworkEntity homework, string pathToFile)
        {
            await Task.Run(() => UploadHomeworkFile(homework, pathToFile));
        }
        #endregion

        public bool TryLogin(bool debug = false)
        {
            try
            {
                Logger.Log("Authorization process started.", ConsoleColor.Yellow);
                var baseAddress = new Uri("https://mystat.itstep.org/");
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler() {CookieContainer = cookieContainer})
                using (var client = new HttpClient(handler) {BaseAddress = baseAddress})
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    var homePageResult = client.GetAsync("/");
                    homePageResult.Result.EnsureSuccessStatusCode();
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("application_key",
                            "6a56a5df2667e65aab73ce76d1dd737f7d1faef9c52e8b8c55ac75f565d8e8a6"),
                        new KeyValuePair<string, string>("id_city", "8"),
                        new KeyValuePair<string, string>("username", $"{Username}"),
                        new KeyValuePair<string, string>("password", $"{Password}"),
                    });
                    var loginResult = client.PostAsync("https://msapi.itstep.org/api/v1/auth/login", content).Result;
                    loginResult.EnsureSuccessStatusCode();
                    dynamic result = JsonConvert.DeserializeObject(loginResult.Content.ReadAsStringAsync().Result);
                    Logger.Log("Access token reading. DONE.", ConsoleColor.Yellow);
                    AccessToken = result.access_token;
                }

                if (debug)
                    return true;

                LoadUserInfo();
                LoadUserActivities();
                LoadSchedule();
                LoadLatestNews();
                LoadHomeworks();
                LoadGroupInfo();

                Logger.Log("======================", ConsoleColor.DarkGray);
                Logger.Log("Succesfully logged in.", ConsoleColor.Green);
                return true;
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, ConsoleColor.Red);
                return false;
            }
        }

        private bool LoadUserInfo()
        {
            try
            {
                Logger.Log("Getting user info.", ConsoleColor.Yellow);
                
                using(HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
                    client.DefaultRequestHeaders.Add("Accept-Language", "ru_RU, ru");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
                    client.DefaultRequestHeaders.Add("Origin", "https://mystat.itstep.org");
                    client.DefaultRequestHeaders.Add("Referer", "https://mystat.itstep.org/ru/auth/login/index");
                    var resp = client.GetAsync(UserInfoUrl).Result;
                    resp.EnsureSuccessStatusCode();

                    CurrentUser = JsonConvert.DeserializeObject<UserEntity>(resp.Content.ReadAsStringAsync().Result);
                }

                Logger.Log("Getting user info DONE.", ConsoleColor.Green);
                return true;
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, ConsoleColor.Red);
                return false;
            }
        }

        private bool LoadLatestNews()
        {
            try
            {
                Logger.Log("Getting latest news.", ConsoleColor.Yellow);
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
                    client.DefaultRequestHeaders.Add("Accept-Language", "ru_RU, ru");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
                    client.DefaultRequestHeaders.Add("Origin", "https://mystat.itstep.org");
                    client.DefaultRequestHeaders.Add("Referer", "https://mystat.itstep.org/ru/auth/login/index");
                    var resp = client.GetAsync(LatestNewsUrl).Result;
                    resp.EnsureSuccessStatusCode();

                    LatestNews = JsonConvert.DeserializeObject<List<NewsEntity>>(resp.Content.ReadAsStringAsync().Result);
                }
                Logger.Log("Getting latest news DONE.", ConsoleColor.Green);
                return true;
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, ConsoleColor.Red);
                return false;
            }
        }

        private string GetEvaluationId()
        {
            try
            {
                Logger.Log("Getting evaluation id.", ConsoleColor.DarkCyan);
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
                    client.DefaultRequestHeaders.Add("Accept-Language", "ru_RU, ru");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer null");
                    client.DefaultRequestHeaders.Add("Origin", "https://mystat.itstep.org");
                    client.DefaultRequestHeaders.Add("Referer", "https://mystat.itstep.org/ru/2U8ftB5OCmgrsK7c76pKE1TXHB_stVr4s");
                    var resp = client.GetAsync(EvaluationIdUrl).Result;
                    resp.EnsureSuccessStatusCode();

                    dynamic evaluation = JsonConvert.DeserializeObject(resp.Content.ReadAsStringAsync().Result);
                    Logger.Log("Getting evaluation id DONE.", ConsoleColor.DarkCyan);
                    return evaluation.id;
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, ConsoleColor.Red);
                return null;
            }
        }

        public bool CollectDailyPoints()
        {
            try
            {
                Logger.Log("Getting daily points...", ConsoleColor.Yellow);
                using (HttpClient client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, DailyPointsUrl);
                    request.Headers.Add("Accept", "application/json, text/plain, */*");
                    request.Headers.Add("Accept-Language", "ru_RU, ru");
                    request.Headers.Add("Authorization", $"Bearer {AccessToken}");
                    request.Headers.Add("Origin", "https://mystat.itstep.org");
                    request.Headers.Add("Referer", "https://mystat.itstep.org/ru/2U8ftB5OCmgrsK7c76pKE1TXHB_stVr4s");

                    MultipartFormDataContent form = new MultipartFormDataContent();
                    form.Add(new StringContent(GetEvaluationId()), "evaluation_id");
                    form.Add(new StringContent("Coal Bot"), "evaluation_comment");

                    request.Content = form;
                    var resp = client.SendAsync(request).Result;
                    resp.EnsureSuccessStatusCode();

                    //dynamic points = JsonConvert.DeserializeObject(resp.Content.ReadAsStringAsync().Result);
                    Logger.Log("Getting daily points DONE.", ConsoleColor.Green);
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, ConsoleColor.Red);
                return false;
            }
        }

        private bool LoadUserActivities()
        {
            try
            {
                Logger.Log("Getting user activities.", ConsoleColor.Yellow);
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
                    client.DefaultRequestHeaders.Add("Accept-Language", "ru_RU, ru");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
                    client.DefaultRequestHeaders.Add("Origin", "https://mystat.itstep.org");

                    var resp = client.GetAsync(UserActivitiesUrl).Result;
                    resp.EnsureSuccessStatusCode();

                    CurrentUserActivities = JsonConvert.DeserializeObject<List<UserActivityEntity>>(resp.Content.ReadAsStringAsync().Result);
                    Logger.Log("Getting user activities DONE.", ConsoleColor.Green);
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, ConsoleColor.Red);
                return false;
            }
        }

        private bool LoadGroupInfo()
        {
            try
            {
                Logger.Log("Getting group info.", ConsoleColor.Yellow);
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
                    client.DefaultRequestHeaders.Add("Accept-Language", "ru_RU, ru");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
                    client.DefaultRequestHeaders.Add("Origin", "https://mystat.itstep.org");

                    var resp = client.GetAsync(GroupInfoUrl).Result;
                    resp.EnsureSuccessStatusCode();

                    GroupUsers = JsonConvert.DeserializeObject<List<GroupUserEntity>>(resp.Content.ReadAsStringAsync().Result);
                    Logger.Log("Getting group info DONE.", ConsoleColor.Green);
                    return true;
                }               
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, ConsoleColor.Red);
                return false;
            }
        }

        private bool LoadSchedule()
        {
            try
            {
                Logger.Log("Getting schedule.", ConsoleColor.Yellow);
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
                    client.DefaultRequestHeaders.Add("Accept-Language", "ru_RU, ru");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
                    client.DefaultRequestHeaders.Add("Origin", "https://mystat.itstep.org");

                    var resp = client.GetAsync(ScheduleUrl).Result;
                    resp.EnsureSuccessStatusCode();

                    Schedule = JsonConvert.DeserializeObject<List<ScheduleEntity>>(resp.Content.ReadAsStringAsync().Result);
                    Logger.Log("Getting schedule DONE.", ConsoleColor.Green);
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, ConsoleColor.Red);
                return false;
            }
        }

        private bool LoadHomeworks()
        {
            try
            {
                Logger.Log("Getting homeworks.", ConsoleColor.Yellow);
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
                    client.DefaultRequestHeaders.Add("Accept-Language", "ru_RU, ru");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
                    client.DefaultRequestHeaders.Add("Origin", "https://mystat.itstep.org");

                    var resp = client.GetAsync(HomeworksUrl).Result;
                    resp.EnsureSuccessStatusCode();

                    Homeworks = JsonConvert.DeserializeObject<List<HomeworkEntity>>(resp.Content.ReadAsStringAsync()
                        .Result);
                    Logger.Log("Getting schedule DONE.", ConsoleColor.Green);
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, ConsoleColor.Red);
                return false;
            }
        }

        public bool DownloadHomeworkFile(HomeworkEntity homework, string downloadPath = null)
        {
            try
            {
                Logger.Log("Downloading homework file...", ConsoleColor.Yellow);
                DirectoryInfo info = null;
                if (downloadPath != null && Directory.Exists(downloadPath))
                    info = new DirectoryInfo(downloadPath);
                else
                    info = new DirectoryInfo(Environment.CurrentDirectory);

                using (var client = new WebClient())
                {
                    client.DownloadFile(homework.Filepath, info.FullName + $"\\{homework.Filename}");
                }
                Logger.Log($"Downloading done. Path: {info.FullName}\\{homework.Filename}", ConsoleColor.Green);
                return true;
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, ConsoleColor.Red);
                return false;
            }
        }

        public bool UploadHomeworkFile(HomeworkEntity homework, string pathToFile)
        {
            try
            {
                Logger.Log("Uploading file...", ConsoleColor.Yellow);

                HttpClient httpClient = new HttpClient();
                MultipartFormDataContent form = new MultipartFormDataContent();

                form.Add(new StringContent(AccessToken), "token");
                form.Add(new StringContent("prod"), "env");
                form.Add(new StringContent("create"), "action");
                byte[] data = File.ReadAllBytes(pathToFile);
                form.Add(new ByteArrayContent(data, 0, data.Length), "file", Path.GetFileName(pathToFile));
                form.Add(new StringContent("1"), "type");

                var httpReqMes = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://mystatfiles.itstep.org/index.php"),
                    Headers =
                    {
                        { "Authorization", $"Bearer {AccessToken}" },
                        { "Accept", "application/json, text/plain, */*" },
                        { "Origin", "https://mystat.itstep.org" },
                        { "Referer", "https://mystat.itstep.org/en/main/homework/page/index" }
                    },
                    Content = form
                };

                HttpResponseMessage responseMessage = httpClient.SendAsync(httpReqMes).Result;
                responseMessage.EnsureSuccessStatusCode();

                dynamic filenameServ = JsonConvert.DeserializeObject(responseMessage.Content.ReadAsStringAsync().Result);
                var content = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("filename", $"{filenameServ.name}"),
                    new KeyValuePair<string, string>("id", $"{homework.Id}")
                });

                var create = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://msapi.itstep.org/api/v1/homework/operations/create"),
                    Headers =
                    {
                        { "Authorization", $"Bearer {AccessToken}" },
                        { "Accept", "application/json, text/plain, */*" },
                        { "Origin", "https://mystat.itstep.org" },
                    },
                    Content = content
                };

                HttpResponseMessage responseMessage2 = httpClient.SendAsync(create).Result;
                responseMessage2.EnsureSuccessStatusCode();                

                httpClient.Dispose();
                Logger.Log("Uploading file DONE.", ConsoleColor.Green);
                return true;
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, ConsoleColor.Red);
                return false;
            }
        }
    }
}
