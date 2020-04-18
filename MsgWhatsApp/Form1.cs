using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Collections.Generic;

namespace MsgWhatsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {

            //SMS
            const string accountSid = "ACb5b821706de321a8e86088e533e103bb";
            const string authToken = "583238c9e8ae185696e0ddc24c1ff8bd";

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: "Olá, José.Você confirma sua consulta com o Dr.Pedro no dia 10 / 08 / 2019 as 09:00 ? A Previsão do tempo é de sol, o tempo estimado para chegada no consultório é de 45min Responda SIM ou Não " +
                "Att Clínica Vida Saúde 44 9 9916 8055",
                from: new Twilio.Types.PhoneNumber("+12055831869"),
                to: new Twilio.Types.PhoneNumber("+5544999168055")
            );

            //Whats
            //TwilioClient.Init("ACb5b821706de321a8e86088e533e103bb", "583238c9e8ae185696e0ddc24c1ff8bd");

            //var message = MessageResource.Create(
            //    body: "Olá, José.Você confirma sua consulta com o Dr.Pedro no dia 10 / 08 / 2019 as 09:00 ? A Previsão do tempo é de sol, o tempo estimado para chegada no consultório é de 45min Responda SIM ou Não " +
            //    "Att Clínica Vida Saúde 44 9 9916 8055";
            //    from: new Twilio.Types.PhoneNumber("whatsapp:+12055831869"),
            //    to: new Twilio.Types.PhoneNumber("whatsapp:+14155238886")
            //);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string senderName = "vinicius";
            string senderEmail = "vinicius@bla";
            string message = "conteudo da mensagem";

            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("vinitessele@gmail.com", "6419vinikarate");
            MailMessage mail = new MailMessage();
            mail.Sender = new System.Net.Mail.MailAddress("vinitessele@gmail.com", "ENVIADOR");
            mail.From = new MailAddress("vinitessele@gmail.com", "ENVIADOR");
            mail.To.Add(new MailAddress("vinicius_tessele@hotmail.com", "RECEBEDOR"));
            mail.Subject = "Contato";
            mail.Body = " Mensagem do site:<br/> Nome:  " + senderName + "<br/> Email : " + senderEmail + " <br/> Mensagem : " + message;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            try
            {
                client.Send(mail);
            }
            catch (System.Exception erro)
            {
                //trata erro
            }
            finally
            {
                mail = null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string origin = "Rua Carlos DallAgnolo 121,Toledo, PR";
            string destination = " R. Santos Dumont, 2546, Toledo, PR";
            string url = "https://maps.googleapis.com/maps/api/distancematrix/xml?origins=" + origin + "&destinations=" + destination + "&key=AIzaSyCNiXQqjhm3GQ83i2FmXXo835XUOfylz6c";
            WebRequest request = WebRequest.Create(url);
            using (WebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    DataSet dsResult = new DataSet();
                    dsResult.ReadXml(reader);
                    string duration = dsResult.Tables["duration"].Rows[0]["text"].ToString();
                    string distance = dsResult.Tables["distance"].Rows[0]["text"].ToString();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //clima tempo previsao de 15 dias
            string Url = "http://apiadvisor.climatempo.com.br/api/v1/forecast/locale/6713/days/15?token=96d44d2abf11e7722035ed2714e4d495";

            var request = HttpWebRequest.Create(Url);
            request.ContentType = "application/json";
            request.Method = "GET";

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            var data = (JObject)JsonConvert.DeserializeObject(responseString);
            var date_br = data["data"].Children();

            foreach (var date_br1 in date_br)
            {
                var DataPrev = date_br1["date_br"].Value<string>();
                var chuva1 = date_br1["rain"].Children();
                var description = date_br1["text_icon"]["text"]["phrase"].Children();

                foreach (var child in chuva1)
                {
                    MessageBox.Show(child.ToString());
                }
                foreach (var description1 in description)
                {
                    MessageBox.Show(description1.ToString());
                }


                Console.WriteLine($"date_br: " + DataPrev + ", reduced: " + description);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string jsonFile = "My First Project-68c8b6a1526c.json";
            string calanderId = @"vinitessele@gmail.com";

            string[] Scopes = { CalendarService.Scope.Calendar };

            ServiceAccountCredential credential;

            using (var stream =
                new FileStream(jsonFile, FileMode.Open, FileAccess.Read))
            {
                var confg = Google.Apis.Json.NewtonsoftJsonSerializer.Instance.Deserialize<JsonCredentialParameters>(stream);
                credential = new ServiceAccountCredential(
                   new ServiceAccountCredential.Initializer(confg.ClientEmail)
                   {
                       Scopes = Scopes
                   }.FromPrivateKey(confg.PrivateKey));
            }

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Calendar API Sample",
            });

            var calander = service.Calendars.Get(calanderId).Execute();
            Console.WriteLine("Calander Name :");
            Console.WriteLine(calander.Summary);

            Console.WriteLine("click for more .. ");
            Console.Read();


            // Define parameters of request.
            EventsResource.ListRequest listRequest = service.Events.List(calanderId);
            listRequest.TimeMin = DateTime.Now;
            listRequest.ShowDeleted = false;
            listRequest.SingleEvents = true;
            listRequest.MaxResults = 10;
            listRequest.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = listRequest.Execute();
            Console.WriteLine("Upcoming events:");
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    string when = eventItem.Start.DateTime.ToString();
                    if (String.IsNullOrEmpty(when))
                    {
                        when = eventItem.Start.Date;
                    }
                    Console.WriteLine("{0} ({1})", eventItem.Summary, when);
                }
            }
            else
            {
                Console.WriteLine("No upcoming events found.");
            }
            Console.WriteLine("click for more .. ");
            Console.Read();

            var myevent = DB.Find(x => x.Id == "eventid" + 1);

            var InsertRequest = service.Events.Insert(myevent, calanderId);

            try
            {
                InsertRequest.Execute();
            }
            catch (Exception)
            {
                try
                {
                    service.Events.Update(myevent, calanderId, myevent.Id).Execute();
                    Console.WriteLine("Insert/Update new Event ");
                    Console.Read();

                }
                catch (Exception)
                {
                    Console.WriteLine("can't Insert/Update new Event ");

                }
            }
        }

        static List<Event> DB =
             new List<Event>() {
                new Event(){
                    Id = "eventid" + 1,
                    Summary = "Consulta Médica Vida Saúde",
                    Location = "800 Unimed., Cascavel, PR 94103",
                    Description = "Consulta clínica Vida Saúde.",
                    Start = new EventDateTime()
                    {
                        DateTime = new DateTime(2019, 08, 11, 15, 30, 0),
                        TimeZone = "America/Los_Angeles",
                    },
                    End = new EventDateTime()
                    {
                        DateTime = new DateTime(2019, 08, 12, 16, 30, 0),
                        TimeZone = "America/Los_Angeles",
                    },
                     Recurrence = new List<string> { "RRULE:FREQ=DAILY;COUNT=2" },
                    Attendees = new List<EventAttendee>
                    {
                        new EventAttendee() { Email = "julio_cvf@outlook.com"},
                        new EventAttendee() { Email = "samukantunes@gmail.com"},
                        new EventAttendee() { Email = "lukaz.17@gmail.com"}
                    }
                }
             };
    }
}


