using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RoomsLiberatorEngine
{
    public class CoreLogic
    {
        public void Loop()
        {
            var timer = new System.Threading.Timer((e) => { CallOutlook(); }, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(1));
        }

        private void CallOutlook()
        {
            var data = GetMockData();

            if (data.Time >= DateTime.Now.Add(TimeSpan.FromMinutes(-5)))
            {
                Debug.WriteLine(data.MaiList.First());
            }
        }

        OutlookResponse GetMockData()
        {
            DateTime localDate = DateTime.Now.Add(TimeSpan.FromMinutes(-2));
            var response = new OutlookResponse()
            {
                Time = DateTime.Now,
                MaiList = new List<string>() {"lviv@eleks.com", "ternopil@eleks.com", "if@eleks.com"}
            };

            return response;
        }
    }

    public struct OutlookResponse
    {
        public DateTime Time { get; set; }
        public List<string> MaiList { get; set; }
    }
}