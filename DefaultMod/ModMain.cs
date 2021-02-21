using GooseShared;
using System;
using System.Diagnostics;
using System.Drawing;

namespace GoosePresence
{
    public class ModEntryPoint : IMod
    {
        private RPC GoosePresence = new RPC("812955051705761822");
        void IMod.Init()
        {
            InjectionPoints.PreTickEvent += PreTick;
            GoosePresence.Init();
            Process.GetCurrentProcess().Exited += new EventHandler(OnProcessExit);
        }

        public void PreTick(GooseEntity g)
        {
            GoosePresence.Update(g);
        }

        public void OnProcessExit(object sender, EventArgs e)
        {
            GoosePresence.Disconnect();
        }
    }
}
