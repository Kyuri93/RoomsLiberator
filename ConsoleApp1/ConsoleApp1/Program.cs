using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
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

            var rooms = GetAllRooms();

            var meetings = GetAllMeetings(rooms, service, new TimeWindow(DateTime.Now.AddMinutes(-30), DateTime.Now));
            //var a = meetings[0].Delete(DeleteMode.MoveToDeletedItems, SendCancellationsMode.SendToAllAndSaveCopy).Result;
            var org = service.ResolveName(meetings[0].Organizer.Name).Result;
            service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, org.FirstOrDefault().Mailbox.Address);

            var calendar = CalendarFolder.Bind(service, WellKnownFolderName.Calendar, new PropertySet()).Result;

            CalendarView cView = new CalendarView(DateTime.Now.AddMinutes(-30), DateTime.Now, 100);
            // Limit the properties returned to the appointment's subject, start time, and end time.
            //cView.PropertySet = new PropertySet(AppointmentSchema.Subject, AppointmentSchema.Start, AppointmentSchema.End);
            // Retrieve a collection of appointments by using the calendar view.
            FindItemsResults<Appointment> appointments = calendar.FindAppointments(cView).Result;
            //appointments.FirstOrDefault(x=>x.id)

            var res = appointments.Items[0].CancelMeeting("No one shoved up").Result;
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

        private static List<Appointment> GetAllMeetings(List<AttendeeInfo> rooms, ExchangeService service, TimeWindow timeWindow)
        {
            List<Appointment> appointments = new List<Appointment>();
            List<AttendeeInfo> attend = new List<AttendeeInfo>();
            foreach (AttendeeInfo inf in rooms)
            {

                FolderId folderid = new FolderId(WellKnownFolderName.Calendar, new Mailbox(inf.SmtpAddress));
                FindItemsResults<Appointment> aps = service.FindAppointments(folderid, new CalendarView(timeWindow.StartTime, timeWindow.EndTime)).Result;
                appointments.AddRange(aps.Items.ToList());
            }

            return appointments;
        }
    }
}
