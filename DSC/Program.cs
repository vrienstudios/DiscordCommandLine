using System;
using System.IO;
using WebSocketSharp;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading;
using DSC.Data;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

namespace DSC
{
    public enum Commands
    {
        Post = 0,
        Delete = 1,
        Pin = 2,
        Select = 3,
    }
    static class Program
    {
        public static string Token = string.Empty;
        static WebSocket ws = new WebSocket("wss://gateway.discord.gg/?v=6&encoding=json");
        static string op1 = @"{""op"": 1, ""d"": 251}"; // ""presence"": { ""game"": { ""name"": ""test"", ""type"": 0 } }
        static string op2 = @"{""op"": 2, ""d"": { ""token"": ""&1"", ""properties"": { ""$os"": ""linux"", ""$browser"": ""etzyy - wrapper"", ""$device"": ""etzyy - wrapper"" }, ""compress"": false, ""large_threshold"": 250, ""status"": ""online"", ""since"": &2, ""afk"": false} }";

        static int flg = 0;
        static int heartbeat = 0;
        static int readycount = 0;

        public static string selectedGuildID = string.Empty; // Used for the current guild which is selected.
        public static Channel selectedChannel;
        public static Guild selectedGuild;
        static ReadyEvent ReadyEvent { get; set; }

        static HttpClient client = new HttpClient();
        static HttpRequestMessage req = new HttpRequestMessage();

        static bool debugFlag = false;
        static bool WSConnect()
        {
            bool fn = false;
            try
            {
                ws.Connect();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("FATAL! Can not connect!");
                return false;
            }
        }
        static bool yN() => Console.ReadLine().ToLower()  == "y";
        static void Exit([Optional] string message)
        {
            if (message != null)
                Console.WriteLine(message);
            Thread.Sleep(20); // provide time to read terminal
            Environment.Exit(0);
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Setting Events!");
            ws.OnOpen += Ws_OnOpen;
            ws.OnMessage += Ws_OnMessage;
            ws.OnClose += Ws_OnClose;

            Console.WriteLine("Setting Arguments!");
            Token = File.ReadAllLines(@"C:\Users\Chay\Source\Repos\tk.txt")[0];
            DateTime dt70 = new DateTime(1970, 1, 1);
            TimeSpan ts70 = DateTime.Now - dt70;
            op2 = op2.Replace("&1", Token).Replace("&2", "1577777392396");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Token);

            Console.WriteLine("Connecting!");
            int timeout = 0;
            while (!WSConnect() && timeout < 5)
            {
                timeout++;
            }
            if (timeout == 5)
                Exit("Failed to connect after 5 tries...");
            while (flg != 2)
                Thread.Sleep(30);
            Console.WriteLine(string.Format("Logged in as: {0} \nContinue? y/N", StaticData.CurrentUser.username));
            if (!yN())
                Exit("Replied 'no' to continuation message.");

            //var data = new { content = "Hello world!" };
            //Console.WriteLine(data.AsJson().ReadAsStringAsync().Result);//654860494414544905 539280524041125889
            //var response = client.PostAsync("https://discordapp.com/api/v6/channels/539280524041125889/messages", data.AsJson());
            //Console.WriteLine(response.Result);

            Console.WriteLine("D for Debug or N for Normal?\n");
            if ((Console.ReadLine().ToLower() == "d") == true)
                Debug();
            else
                Run();
            Console.ReadLine();
            ws.Close();
        }
        static void Run()
        {
            for(int i = 0; i < ReadyEvent.d.guilds.Count; i++)
            {
                Console.WriteLine(string.Format("{3}| Name: {0} | Members: {1} | ID: {2}", ReadyEvent.d.guilds[i].name, ReadyEvent.d.guilds[i].member_count, ReadyEvent.d.guilds[i].id, i));
            }
            Console.Write("Select a guild: \n");
            //selectedGuild = ReadyEvent.d.guilds.Find(root => root.id == selectedGuildID);
            selectedGuild = ReadyEvent.d.guilds[int.Parse(Console.ReadLine())]; // will fix this later.
            Console.WriteLine("Gathering information about: " + selectedGuild.name);
            for(int i = 0; i < selectedGuild.channels.Count; i++)
            {
                Console.WriteLine(string.Format("{0}| {1}", i, selectedGuild.channels[i].name));
            }
            Console.Write("Select Channel: ");
            int ik = int.Parse(Console.ReadLine()); // Too lazy. (refer to above comment)
            selectedChannel = selectedGuild.channels[ik];
            Console.Write("\nSend: ");

            //https://discordapp.com/api/v6/channels/572638894538096650/messages
            object message = new Message
            {
                content = Console.ReadLine()
            };
            StringContent data = new StringContent(JsonConvert.SerializeObject(message), UnicodeEncoding.UTF8, "application/json");
            System.Threading.Tasks.Task<HttpResponseMessage> response = client.PostAsync(string.Format("https://discordapp.com/api/v6/channels/{0}/messages", selectedChannel.id), data);
            Console.WriteLine(response.Result);
        }
        class Message // temporary
        {
            public string content { get; set; }
        }
        static void Debug()
        {
            debugFlag = true;
        }

        private static void Ws_OnOpen(object sender, EventArgs e)
        {
            Console.WriteLine("Connected! Waiting for handshake...");
        }

        private static void Ws_OnClose(object sender, CloseEventArgs e)
        {
            Console.WriteLine(e.Reason);
            Console.WriteLine(e.Code);
        }

        //Console.WriteLine("DATA FLAG");
        //Console.WriteLine(e.Data);
        //Console.WriteLine("END DATA");
        //int x = 0;
        //int.TryParse(e.Data.Split(',')[2].Split(':')[1], out x);
        private static void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            ReadyEvent RO = JsonConvert.DeserializeObject<ReadyEvent>(e.Data); // Prone to error.
            Console.WriteLine(String.Format("DEBUG| t: {0} | s: {1} | op: {2}", RO.t, RO.s, RO.op));
            if(flg != 2)
            {
                if (readycount >= 20)
                    Exit("FAILED TO RECIEVE CRITICAL OP FROM SERVER");
                if (flg < 1)
                {
                    readycount++;
                    if (RO.op == 10)
                    {
                        Console.WriteLine("Op10 has been recieved!, parsing...");
                        HeartBeat heart = JsonConvert.DeserializeObject<HeartBeat>(e.Data);
                        heartbeat = heart.d.heartbeat_interval;
                        Console.WriteLine("Heartbeat: " + heartbeat);
                        Thread thr = new Thread(() => PostOp1(heartbeat));
                        thr.Start();
                        flg = 1;
                        ws.Send(op2);
                        Console.WriteLine("Replying with OP2... waiting.");
                        readycount = 0;
                    }
                }
                else
                {
                    if (RO.t == "READY")
                    {
                        Console.WriteLine("Recieved 'READY' event! Handshake complete!");
                        StaticData.CurrentUser = RO.d.user;
                        StaticData.Guilds = RO.d.guilds;
                        ReadyEvent = RO;
                        flg = 2;
                    }
                    else
                        readycount++;
                }
            }
            else
            {
                switch (RO.t)
                {
                    default:
                        if(debugFlag)
                            Console.WriteLine("UNKNOWN EVENT: " + string.Format("{0}\n{1}", RO.t, JsonConvert.DeserializeObject(e.Data)));
                        break;
                    case "MESSAGE_CREATE":
                        Data.EventTypes.MESSAGE_CREATE.Event_message_create MC = JsonConvert.DeserializeObject<Data.EventTypes.MESSAGE_CREATE.Event_message_create>(e.Data);
                        Console.WriteLine(string.Format("user: {0} | content: {1}", MC.d.author.username, MC.d.content));
                        break;
                    case "PRESENCE_UPDATE":
                        Data.EventTypes.PRESENCE_UPDATE _UPDATE = JsonConvert.DeserializeObject<Data.EventTypes.PRESENCE_UPDATE>(e.Data);
                        Console.WriteLine(JsonConvert.DeserializeObject(e.Data));
                        break;
                    case "null": // ACK
                        break;
                }
            }
        }   

        static void PostOp1(int heartbeat_interval)
        {
            while (true)
            {
                ws.Send(op1);
                Thread.Sleep(heartbeat_interval);
            }
        }
        static void Push(string token, string msg, string channel)
        {

        }
    }
}
