using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgentApp.Wrapers;

namespace AgentApp
{
    class SensorReader
    {
        public void Start()
        {
            var mfrc = new Mfrc522();

            var task = new Task(async () => { await mfrc.InitIO(); });
            task.RunSynchronously();


            while (true)
            {
                if (mfrc.IsTagPresent())
                {
                    var uid = mfrc.ReadUid();

                    mfrc.HaltTag();
                }

            }

        }
    }
}
