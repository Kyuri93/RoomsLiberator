using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.WebServices.Data;


namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;

            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2016);
            service.Url = new Uri("https://172.25.10.12/EWS/Exchange.asmx");
            service.Credentials = new WebCredentials("administrator@hco.local", "Qwerty1234");
            service.TraceEnabled = false;
            service.GetRoomLists();


            //service.AutodiscoverUrl("user1@hco.com", RedirectionUrlValidationCallback);
            //EmailMessage email = new EmailMessage(service);
            //email.ToRecipients.Add("user1@hco.local");
            //email.Subject = "HelloWorld";
            //email.Body = new MessageBody("This is the first email I've sent by using the EWS Managed API");
            //email.Send();

            //var calendar = CalendarFolder.Bind(service, WellKnownFolderName.Calendar, new PropertySet());

            //var startDate = DateTime.Today;
            //var endDate = DateTime.Today + TimeSpan.FromDays(1);
            //var NUM_APPTS = 100;

            //CalendarView cView = new CalendarView(startDate, endDate, NUM_APPTS);
            //// Limit the properties returned to the appointment's subject, start time, and end time.
            //cView.PropertySet = new PropertySet(AppointmentSchema.Subject, AppointmentSchema.Start, AppointmentSchema.End);
            //// Retrieve a collection of appointments by using the calendar view.
            //FindItemsResults<Appointment> appointments = calendar.FindAppointments(cView);
            //Console.WriteLine("\nThe first " + NUM_APPTS + " appointments on your calendar from " + startDate.Date.ToShortDateString() +
            //                  " to " + endDate.Date.ToShortDateString() + " are: \n");

            //foreach (Appointment a in appointments)
            //{
            //    Console.Write("Subject: " + a.Subject.ToString() + " ");
            //    Console.Write("Start: " + a.Start.ToString() + " ");
            //    Console.Write("End: " + a.End.ToString());
            //    Console.WriteLine();
            //}


            var rooms = GetAllRooms();

            var meetings = GetAllMeetings(rooms, service, new TimeWindow(DateTime.Today, DateTime.Now + TimeSpan.FromDays(1)));

            Console.ReadLine();
        }

        private static bool CertificateValidationCallBack(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            return true;
        }

        private static bool RedirectionUrlValidationCallback(string redirectionurl)
        {
            throw new NotImplementedException();
        }

        private static List<AttendeeInfo> GetAllRooms()
        {
            var ldap = new DirectoryEntry("LDAP://172.25.10.11:389", "hco\\administrator", "Qwerty1234");


            string filter = "(&(objectClass=*)(msExchRecipientDisplayType=7))";
            //Assembly System.DirectoryServices.dll
            DirectorySearcher search = new DirectorySearcher(ldap);
            search.Filter = filter;
            List<AttendeeInfo> rooms = new List<AttendeeInfo>();
            foreach (SearchResult result in search.FindAll())
            {
                ResultPropertyCollection r = result.Properties;
                DirectoryEntry entry = result.GetDirectoryEntry();
                // entry.Properties["displayName"].Value.ToString() will bring the room name
                rooms.Add(new AttendeeInfo(entry.Properties["mail"].Value.ToString().Trim()));
            }

            return rooms;
        }

        private static List<CalendarEvent> GetAllMeetings(List<AttendeeInfo> rooms, ExchangeService service, TimeWindow timeWindow)
        {
            List<CalendarEvent> events = new List<CalendarEvent>();
            List<AttendeeInfo> attend = new List<AttendeeInfo>();
            foreach (AttendeeInfo inf in rooms)
            {
                attend.Clear();
                attend.Add(inf.SmtpAddress);

                AvailabilityOptions options = new AvailabilityOptions();
                options.MaximumSuggestionsPerDay = 48;
                // service is ExchangeService object contains your authentication with exchange server
                GetUserAvailabilityResults results = service.GetUserAvailability(attend, timeWindow, AvailabilityData.FreeBusyAndSuggestions, options).Result;

                foreach (AttendeeAvailability attendeeAvailability in results.AttendeesAvailability)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    if (attendeeAvailability.ErrorCode == ServiceError.NoError)
                    {
                        foreach (Microsoft.Exchange.WebServices.Data.CalendarEvent calendarEvent in
                            attendeeAvailability.CalendarEvents)
                        {
                            Console.WriteLine("Calendar event");
                            Console.WriteLine(" Starttime: " + calendarEvent.StartTime.ToString());
                            Console.WriteLine(" Endtime: " + calendarEvent.EndTime.ToString());
                            if (calendarEvent.Details != null)
                            {
                                Console.WriteLine(" Subject:" + calendarEvent.Details.Subject);

                            }

                            events.Add(calendarEvent);
                        }
                    }
                }    
            }

            return events;
        }
    }
}
