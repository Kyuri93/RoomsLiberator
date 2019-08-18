using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;

namespace RoomsLiberatorEngine
{
    public class ExchangeServiceWrapper
    {
        private const string AdminPassword = "Qwerty1234";
        private const string AdminAdLogin = "hco\\administrator";
        private const string AdminExLogin = "administrator@hco.local";

        private const string AdAddress = "LDAP://172.25.10.11:389";
        private const string ExAddress = "https://172.25.10.12/EWS/Exchange.asmx";

        public ExchangeServiceWrapper()
        {
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;

        }

        private bool CertificateValidationCallBack(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            return true;
        }

        public List<Appointment> GetAppointments(DateTime from, DateTime to)
        {
            var service = InitService();
            var rooms = GetAllRooms();
            return GetAppointments(service, rooms, from, to);
        }

        public void SendWarning(Appointment appointment)
        {
            var service = InitService();
            var email = new EmailMessage(service);

            var organizer = service.ResolveName(appointment.Organizer.Name).Result.FirstOrDefault();

            if (organizer != null)
            {
                email.ToRecipients.Add(organizer.Mailbox.Address);
            }

            email.Subject = "Appointment auto cancellation reminder";
            email.Body = new MessageBody($"Click this link to prevent appointment from cancellation {appointment.ICalUid}");
            email.Send();
        }

        public void CancelAppointment(Appointment appointment)
        {
            var service = InitService();

            var organizer = service.ResolveName(appointment.Organizer.Name).Result.FirstOrDefault();
            if (organizer != null)
            {
                service.ImpersonatedUserId =
                    new ImpersonatedUserId(ConnectingIdType.SmtpAddress, organizer.Mailbox.Address);

                var calendar = CalendarFolder.Bind(service, WellKnownFolderName.Calendar, new PropertySet()).Result;

                CalendarView cView = new CalendarView(appointment.Start, appointment.End, 100);
                // Limit the properties returned to the appointment's subject, start time, and end time.
                //cView.PropertySet = new PropertySet(AppointmentSchema.Subject, AppointmentSchema.Start, AppointmentSchema.End);
                // Retrieve a collection of appointments by using the calendar view.
                FindItemsResults<Appointment> appointments = calendar.FindAppointments(cView).Result;

                var a = appointments.Items.FirstOrDefault(x => x.ICalUid == appointment.ICalUid);
                //appointments.FirstOrDefault(x=>x.id)

                var res = a.CancelMeeting("No one shoved up").Result;
            }
        }

        public NameResolution GetOrganizer(Appointment appointment)
        {
            var service = InitService();
            return service.ResolveName(appointment.Organizer.Name).Result.FirstOrDefault();
        }

        private List<Appointment> GetAppointments(ExchangeService service, List<AttendeeInfo> rooms, DateTime from, DateTime to)
        {
            List<Appointment> appointments = new List<Appointment>();
            List<AttendeeInfo> attend = new List<AttendeeInfo>();
            foreach (AttendeeInfo inf in rooms)
            {

                FolderId folderid = new FolderId(WellKnownFolderName.Calendar, new Mailbox(inf.SmtpAddress));
                FindItemsResults<Appointment> aps = service.FindAppointments(folderid, new CalendarView(from, to)).Result;
                appointments.AddRange(aps.Items.ToList());
            }

            return appointments;
        }

        private ExchangeService InitService()
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2016);
            service.Url = new Uri(ExAddress);
            service.Credentials = new WebCredentials(AdminExLogin, AdminPassword);
            service.TraceEnabled = false;
            return service;
        }

        private static List<AttendeeInfo> GetAllRooms()
        {
            var ldap = new DirectoryEntry(AdAddress, AdminAdLogin, AdminPassword);


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
    }
}
