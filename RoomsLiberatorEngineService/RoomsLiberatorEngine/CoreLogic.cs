using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RoomsLiberatorEngine
{
    public class CoreLogic
    {
        private ExchangeServiceWrapper _exchangeService = new ExchangeServiceWrapper();

        public void Loop()
        {
            var timer = new System.Threading.Timer((e) => { CallOutlook(); }, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(1));
        }

        private void CallOutlook()
        {
            var appointments = _exchangeService.GetAppointments(DateTime.Now.AddMinutes(-15), DateTime.Now);
            foreach (var appointment in appointments)
            {
                if (appointment.Start >= DateTime.Now.Add(TimeSpan.FromMinutes(-20)))
                {
                    _exchangeService.CancelAppointment(appointment);
                }
                else if (appointment.Start >= DateTime.Now.Add(TimeSpan.FromMinutes(-5)))
                {
                    _exchangeService.SendWarning(appointment);
                    //warning
                }
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