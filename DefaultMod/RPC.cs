using DiscordRPC;
using DiscordRPC.Logging;
using DiscordRPC.Message;
using GooseShared;
using System;
using System.Diagnostics;
using System.Reflection;

namespace GoosePresence
{
    class RPC
    {
        private Timestamps Now;
        private DiscordRpcClient Client;
        private GooseEntity Goose;
        private string[] tasks;
        private string prevTask;

        private float prevSpeed = 0;
        private string speedTier;
        private string currentTask;
        private bool isRunning;
        public static Taskz t = new Taskz();
        Random rand = new Random();

        public string[] Run = { "Doing the running man challenge.", "Running away from good choices.", "Trying to reduce weight.", "Wait, why am I running?" };
        public string[] Walk = { "Running at a slow pace.", "Doing the reverse moonwalk.", "Tap-tap-Tap-Tap!", "Going out on a walk.", "Why is my feet so loud?", "Exploring the monitor.", "Walking Loudly." };
        public string[] Charge = { "Walking at an incredibly fast pace.", "Surpassing the Speed Limit.", "Found some Nuggies.", "Probably Angry." };


        // Custom Tasks || Speed

        private string[] Custom = { "I wonder what this is?","I didn't know the goose could do this.", "Doing Something new.", "Well, this is new." };

        public class Taskz
        {
            public string[] Wandering = { "Contemplating life choices.", "Thinking about memes","Thinking of causing more chaos.","Finding more ways to cause trouble.","Searching for something.." };
            public string[] NabbingMouse = { "Biting the Mouse.","MONCH.", "Figuring out if the mouse is a mice.", "Chasing the mouse." };
            public string[] CollectingMemes = { "Dragging Memes.", "Sending Memes.","Giving some honky memes.","Mm yes. The honk is made out of honk." };
            public string[] CollectingNotepads = { "Dragging a Not-epad.", "Being an amazing poet.","Giving life advice.","Typing with two feet.","Getting a typewriter.","Typing with eyes closed." };
            public string[] TrackingMud = { "Making the screen dirty.","Might wanna clean that.","This is my monitor now.","Fertilizing the screen." };
            public string[] CustomMouseNab = { "ONE PUNCHH!!", "Chasing the mouse at an incredibly fast speed.", "Why do I hear boss music?" };
            public string[] Sleeping = { "Zzzzzz...", "Dreaming about getting infinte bells.", "Causing chaos in a dream.","Ooh, Comfy.","Dreaming.", "Chillin." };
            public string[] RunToBed = { "Going to bed.", "Found a comfy bed.", "About to take a nap.", "About to sleep.","Going towards the comfy zone." };
            public string[] ChargeToStick = { "Ooh! A stick!", "Running towards a stick.", "GIMMIE GIMMIE GIMMIE-", "Stick Motion Detected!"};
            public string[] ReturnStick = { "Returning stick!", "Stick has been obtained!" };
            public string[] ChaseLaser = { "Laser Detected!", "Endlessly chasing a red dot." };
        }


        public RPC(string ClientId)
        {
            Client = new DiscordRpcClient(ClientId);
            Now = Timestamps.Now;
        }

        public void Init()
        {
            tasks = API.TaskDatabase.getAllLoadedTaskIDs.Invoke();
            // CLIENT RPC //
            Client.Initialize();
            Client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };
            Client.OnConnectionEstablished += Client_OnConnectionEstablished;
            Client.OnConnectionFailed += Client_OnConnectionFailed;
            Client.OnReady += Client_OnReady;
        }

        private void Client_OnConnectionFailed(object sender, ConnectionFailedMessage args)
        {
            Console.WriteLine("[{1}] Pipe Connection Failed. Could not connect to pipe #{0}", args.FailedPipe, args.TimeCreated);
            isRunning = false;
        }

        private void Client_OnReady(object sender, ReadyMessage args)
        {
            Console.WriteLine("[{0}][{1}] Recieved Ready from user {2}", args.Version, args.TimeCreated, args.User);
            isRunning = true;
        }

        private void Client_OnConnectionEstablished(object sender, ConnectionEstablishedMessage args)
        {
            Console.WriteLine("[{1}] Connection established! Connected to pipe #{0}", args.ConnectedPipe, args.TimeCreated);
        }

        private string getRand(string[] s)
        {
            return s[rand.Next(s.Length)];
        }

        public void Disconnect()
        {
            Client.Dispose();
        }

        public void Update(GooseEntity g)
        {
            if (!isRunning) return;
            Goose = g;
            float walkSpeed = Goose.parameters.WalkSpeed;
            float runSpeed = Goose.parameters.RunSpeed;
            float chargeSpeed = Goose.parameters.ChargeSpeed;
            if (prevSpeed != Goose.currentSpeed || prevTask != tasks[Goose.currentTask])
            {
                speedTier = (Goose.currentSpeed == walkSpeed) ? $"{getRand(Walk)} (Walking)" :
                    ((Goose.currentSpeed == runSpeed) ? $"{getRand(Run)} (Running)" : 
                    ((Goose.currentSpeed == chargeSpeed) ? $"{getRand(Charge)} (Charging)" : 
                    $"{getRand(Custom)} (Speed: {Goose.currentSpeed})"));
                foreach(FieldInfo fieldInfo in typeof(Taskz).GetFields())
                {
                    if(fieldInfo.Name.Replace("ing","").Replace("bb","b").Replace("mes","me").Replace("pads","pad") == tasks[Goose.currentTask] || fieldInfo.Name == tasks[Goose.currentTask])
                    {
                        currentTask = $"{getRand((string[])fieldInfo.GetValue(t))} ({fieldInfo.Name})";
                        break;
                    }
                    else
                    {
                        currentTask = $"{getRand(Custom)} ({tasks[Goose.currentTask]})";
                    }
                }
            }
            prevSpeed = Goose.currentSpeed;
            prevTask = tasks[Goose.currentTask];
            Client.SetPresence(new RichPresence()
            {
                Details = currentTask,
                State = speedTier,
                Timestamps = Now,
                Assets = new Assets()
                {
                    LargeImageKey = "big",
                    LargeImageText = "Honk!",
                    SmallImageKey = null
                }
            });
        }
    }
}
