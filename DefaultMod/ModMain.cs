using GooseShared;
using System;
using System.Drawing;
using System.Threading;

namespace GoosePresence
{
    public class ModEntryPoint : IMod
    {
        private RPC GoosePresence = new RPC("736878420754956318");
        void IMod.Init()
        {
            InjectionPoints.PostTickEvent += PostTick;
            GoosePresence.Init();
        }

        public void PostTick(GooseEntity g)
        {
            GoosePresence.Update(g);
        }
    }
}
