﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Discord.Net;
using WebSocketSharp;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading;
using System.Web.Script.Serialization;
using DSC.Data;
using System.Runtime.InteropServices;

namespace DSC
{
    static class Program
    {
        public static string Token = string.Empty;
        static WebSocket ws = new WebSocket("wss://gateway.discord.gg/?v=6&encoding=json");
        static string op1 = @"{""op"": 1, ""d"": 251}"; // ""presence"": { ""game"": { ""name"": ""HECC"", ""type"": 0 } }
        static string op2 = @"{""op"": 2, ""d"": { ""token"": ""&1"", ""properties"": { ""$os"": ""linux"", ""$browser"": ""etzyy - wrapper"", ""$device"": ""etzyy - wrapper"" }, ""compress"": false, ""large_threshold"": 250, ""status"": ""online"", ""since"": &2, ""afk"": false} }";
        static string op2p = string.Empty;

        static RootObject ReadyEvent { get; set; }
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
        static bool yN() => Console.ReadLine().ToLower()  == "y" ? true : false;
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

            HttpClient client = new HttpClient();
            HttpRequestMessage req = new HttpRequestMessage();
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Token);
            //var data = new { content = "Hello world!" };
            //Console.WriteLine(data.AsJson().ReadAsStringAsync().Result);//654860494414544905 539280524041125889
            //var response = client.PostAsync("https://discordapp.com/api/v6/channels/539280524041125889/messages", data.AsJson());
            //Console.WriteLine(response.Result);

            Console.WriteLine("PAUSED");
            Console.ReadLine();
            ws.Close();
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
        static int flg = 0;
        static int heartbeat = 0;
        private static void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            RootObject RO = JsonConvert.DeserializeObject<RootObject>(e.Data);
            Console.WriteLine(String.Format("t: {0} | s: {1} | op: {2}", RO.t, RO.s, RO.op));
            if(RO.t == "MESSAGE_CREATE")
            {
                Data.EventTypes.MESSAGE_CREATE.Event_message_create MC = JsonConvert.DeserializeObject<Data.EventTypes.MESSAGE_CREATE.Event_message_create>(e.Data);
                Console.WriteLine(string.Format("user: {0} | content: {1}", MC.d.author.username, MC.d.content));
            }
            if (RO.op == 10)
            {
                Console.WriteLine("Op10 has been recieved!, parsing...");
                Data.HeartBeat heart = JsonConvert.DeserializeObject<HeartBeat>(e.Data);
                heartbeat = heart.d.heartbeat_interval;
                Console.WriteLine("Heartbeat: " + heartbeat);
                ws.Send(op2);
                Console.WriteLine("Replying with OP2... waiting.");
            }
            if (flg == 0)
            {
                Thread thr = new Thread(() => PostOp1(heartbeat));
                thr.Start();
                flg = 1;
            }
            if(RO.t == "READY")
            {
                Console.WriteLine("Recieved 'READY' event! Handshake complete!");
                StaticData.CurrentUser = RO.d.user;
                StaticData.Guilds = RO.d.guilds;
                ReadyEvent = RO;
                flg = 2;
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