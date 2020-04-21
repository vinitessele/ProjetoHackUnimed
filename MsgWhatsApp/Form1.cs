using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System.Collections.Generic;
using System.Xml.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace MsgWhatsApp
{
    public partial class Form1 : Form
    {
        public class CityOrigem
        {
            public string cidadeOrigem { get; set; }
        }
        public class CityDestino
        {
            public string cidadeDestino { get; set; }
        }
        public class CityDestinoDuracao
        {
            public string cidadeDestino { get; set; }
            public string tempo { get; set; }
            public string km { get; set; }
        }
        public class Dacte
        {
            public string Destinatario { get; set; }
            public string Endereco { get; set; }
            public string Chave { get; set; }
        }

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

        private void button6_Click(object sender, EventArgs e)
        {
            string origin = "Rua Carlos DallAgnolo 121,Toledo, PR";
            string destination = " R. Santos Dumont, 2546, Toledo, PR";
            //URL do distancematrix - adicionando endereço de origem e destino
            string url = string.Format("http://maps.googleapis.com/maps/api/directions/xml?origin={0}&destination={1}&mode=driving&language=pt-BR&sensor=false",
                origin, destination);

            //Carregar o XML via URL
            XElement xml = XElement.Load(url);

            //Verificar se o status é OK
            if (xml.Element("status").Value == "OK")
            {
                string rota = string.Empty;
                //Pegar os detalhes de cada passo da rota
                foreach (var item in xml.Element("route").Element("leg").Elements("step"))
                {
                    //Pegar as instruções da rota em HTML
                    rota += item.Element("html_instructions").Value + "<br />";
                    //Pegar a distância deste trecho
                    rota += item.Element("distance").Element("text").Value + "<br />";
                }

                //Formatar a resposta
                string litResultado = string.Format("<strong>Origem</strong>: {0} <br /><strong>Destino:</strong> {1} <br /><strong>Distância</strong>: {2} <br /><strong>Duração</strong>: {3} <br /><strong>Como chegar ao destino</strong>:<br /> {4}",
                     //Pegar endereço de origem
                     xml.Element("route").Element("leg").Element("start_address").Value,
                     //Pegar endereço de destino
                     xml.Element("route").Element("leg").Element("end_address").Value,
                     //Pegar duração
                     xml.Element("route").Element("leg").Element("duration").Element("text").Value,
                     //Pegar a distância
                     xml.Element("route").Element("leg").Element("distance").Element("text").Value,
                     //Adicionar a rota gerada logo acima
                     rota
                     );
                //Atualizar o mapa
                //map.Src = "https://maps.google.com/maps?saddr=" + xml.Element("route").Element("leg").Element("start_address").Value + "&daddr=" + xml.Element("route").Element("leg").Element("end_address").Value + "&output=embed";
            }
            else
            {
                //Se ocorrer algum erro
                string litResultado = String.Concat("Ocorreu o seguinte erro: ", xml.Element("status").Value);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {

            List<CityOrigem> Listorigin = new List<CityOrigem>();
            List<CityDestinoDuracao> listdestinoTempo = new List<CityDestinoDuracao>();
            CityOrigem origem = new CityOrigem();
            origem.cidadeOrigem = "Rua Carlos DallAgnolo 121,Toledo, PR";
            Listorigin.Add(origem);
            CityOrigem origem1 = new CityOrigem();
            origem1.cidadeOrigem = "R. Santos Dumont, 2546, Toledo, PR";
            CityOrigem origem2 = new CityOrigem();
            origem2.cidadeOrigem = "R. Emílio de Menezes, 423, Toledo, PR";
            Listorigin.Add(origem2);
            CityOrigem origem3 = new CityOrigem();
            origem3.cidadeOrigem = "R. Emíliano Perneta, 611, Toledo, PR";
            Listorigin.Add(origem3);
            CityOrigem origem4 = new CityOrigem();
            origem4.cidadeOrigem = "R. Acapulco, 95, Toledo, PR";
            Listorigin.Add(origem4);
            CityOrigem origem5 = new CityOrigem();
            origem5.cidadeOrigem = "Rua Itália Piovesan Pasqualli, 374, Toledo, PR";
            Listorigin.Add(origem5);
            CityOrigem origem6 = new CityOrigem();
            origem6.cidadeOrigem = "R. Aloíso Anschau, 696, Toledo, PR";
            Listorigin.Add(origem6);
            CityOrigem origem7 = new CityOrigem();
            origem7.cidadeOrigem = "R. dos Pioneiros, 437, Toledo, PR";
            Listorigin.Add(origem7);
            CityOrigem origem8 = new CityOrigem();
            origem8.cidadeOrigem = "R. Luís Genari, 329, Toledo, PR";
            Listorigin.Add(origem8);
            CityOrigem origem9 = new CityOrigem();
            origem9.cidadeOrigem = "R. Garibalde, 1733, Toledo, PR";
            Listorigin.Add(origem9);
            CityOrigem origem10 = new CityOrigem();
            origem10.cidadeOrigem = "R. Paulo VI, 399, Toledo, PR";
            Listorigin.Add(origem10);
            string origin = "Rua Carlos DallAgnolo 121,Toledo, PR";
            //string destination = " R. Santos Dumont, 2546, Toledo, PR";
            foreach (var l in Listorigin)
            {
                string url = "https://maps.googleapis.com/maps/api/distancematrix/xml?origins=" + origin + "&destinations=" + l.cidadeOrigem + "&key=AIzaSyCNiXQqjhm3GQ83i2FmXXo835XUOfylz6c";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        DataSet dsResult = new DataSet();
                        dsResult.ReadXml(reader);
                        string duration = dsResult.Tables["duration"].Rows[0]["text"].ToString();
                        string distance = dsResult.Tables["distance"].Rows[0]["text"].ToString();
                        CityDestinoDuracao i = new CityDestinoDuracao();
                        i.cidadeDestino = l.cidadeOrigem;
                        i.tempo = duration;
                        i.km = distance;
                        listdestinoTempo.Add(i);
                        listdestinoTempo.Add(i);
                        listdestinoTempo.Add(i);
                        listdestinoTempo.Add(i);
                        listdestinoTempo.Add(i);
                        listdestinoTempo.Add(i);
                    }
                }
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            ITextExtractionStrategy its = new iTextSharp.text.pdf.parser.LocationTextExtractionStrategy();

            using (PdfReader reader = new PdfReader("c:\\temp\\adriana.pdf"))
            {
                StringBuilder text = new StringBuilder();
                Boolean ehdestinatario = false;

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string thePage = PdfTextExtractor.GetTextFromPage(reader, i, its);
                    string[] theLines = thePage.Split('\n');
                    List<Dacte> listdacte = new List<Dacte>();
                    foreach (var theLine in theLines)
                    {
                        try
                        {
                            Dacte dacte = new Dacte();
                            if (theLine.StartsWith("DESTINATARIO"))
                            {
                                if (theLine.StartsWith("DESTINATARIODESTINATARIO"))
                                {
                                    dacte.Destinatario = theLine.Substring(24).Remove(theLine.Length - 14);
                                    textBox1.AppendText(dacte.Destinatario);
                                    textBox1.AppendText(Environment.NewLine);
                                    ehdestinatario = true;
                                }
                                else if (theLine.StartsWith("DESTINATARIODESTINATARIODESTINATARIO"))
                                {
                                    dacte.Destinatario = theLine.Substring(36).Remove(theLine.Length - 21);
                                    textBox1.AppendText(dacte.Destinatario);
                                    textBox1.AppendText(Environment.NewLine);
                                    ehdestinatario = true;
                                }
                                else
                                {
                                    dacte.Destinatario = theLine.Substring(12);
                                    dacte.Destinatario = dacte.Destinatario.Remove(dacte.Destinatario.Length - 7);


                                    textBox1.AppendText(dacte.Destinatario);
                                    textBox1.AppendText(Environment.NewLine);
                                    ehdestinatario = true;
                                }
                            }
                            if (theLine.StartsWith("END") && ehdestinatario)
                            {
                                dacte.Endereco = theLine.Substring(3);
                                textBox1.AppendText(theLine.Substring(3));
                                textBox1.AppendText(Environment.NewLine);
                                ehdestinatario = false;
                            }
                            else if (theLine.StartsWith("ENDEND") && ehdestinatario)
                            {
                                dacte.Endereco = theLine.Substring(3);
                                textBox1.AppendText(theLine.Substring(3));
                                textBox1.AppendText(Environment.NewLine);
                                ehdestinatario = false;
                            }
                            else
                            if (theLine.StartsWith("ENDENDEND") && ehdestinatario)
                            {
                                dacte.Endereco = theLine.Substring(3);
                                textBox1.AppendText(theLine.Substring(3));
                                textBox1.AppendText(Environment.NewLine);
                                ehdestinatario = false;
                            }
                            else
                            {
                                dacte.Endereco = theLine.Substring(3);
                                textBox1.AppendText(theLine.Substring(3));
                                textBox1.AppendText(Environment.NewLine);
                                ehdestinatario = false;

                            }
                            //ehdestinatario = false;
                            //text.AppendLine(theLine);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
        }
    }
}





