using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AgentApp.Wrapers;

namespace AgentApp
{
    class SensorReader
    {
        private bool _isStoping = false;
        private HttpSender _sender = new HttpSender();

        public SensorReader()
        {
            
        }

        public void Start()
        {
            var mfrc = new Mfrc522();

            var task = new Task(async () => { await mfrc.InitIO(); });
            task.RunSynchronously();


            while (true)
            {
                if (_isStoping)
                {
                    break;
                }

                if (mfrc.IsTagPresent())
                {
                    var uid = mfrc.ReadUid();

                    var sendTask = new Task(async () => { await _sender.SendUserId(uid.ToString()); });
                    sendTask.Start();

                    mfrc.HaltTag();
                }

                Task.Delay(100).Wait();
            }

        }

        public void Stop()
        {
            _isStoping = true;
        }

        

    }
}
