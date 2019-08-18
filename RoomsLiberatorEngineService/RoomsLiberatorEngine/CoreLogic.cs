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

            var DbContext = new DatabaseContext();


            foreach (var appointment in appointments)
            {
                try
                {
                    var organaizer = _exchangeService.GetOrganizer(appointment);

                    var orqanaizerId = DbContext.Users.FirstOrDefault(x => x.UserMail == organaizer.Mailbox.Address).UserId;

                    var lastBip = DbContext.DeviceStates.FirstOrDefault(x => x.Value == orqanaizerId);


                    if (lastBip.Date > appointment.Start.Add((TimeSpan.FromMinutes(-1))))
                    {
                        continue;
                    }
                    else
                    {
                        if (appointment.Start <= DateTime.Now.Add(TimeSpan.FromMinutes(-2)))
                        {
                            _exchangeService.CancelAppointment(appointment);
                        }
                        else if (appointment.Start >= DateTime.Now.Add(TimeSpan.FromMinutes(-1)))
                        {
                            _exchangeService.SendWarning(appointment);
                            //warning
                        }
                    }
                }
                catch (Exception e)
                {
                    continue;
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