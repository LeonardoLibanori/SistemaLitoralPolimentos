using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;

namespace SistemaLitoralPolimentos.Services
{
    public class GoogleCalendarService
    {
        private readonly CalendarService _calendarService;
        private readonly string _calendarId;

        public GoogleCalendarService(IConfiguration config)
        {
            var credenciaisPath = config["GoogleCalendar:CredenciaisPath"]
                ?? "google-credentials.json";
            _calendarId = config["GoogleCalendar:CalendarId"]
                ?? throw new Exception("CalendarId não configurado no appsettings.json");

            // Carrega as credenciais do arquivo JSON
            GoogleCredential credential;
            using (var stream = new FileStream(credenciaisPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential
                    .FromStream(stream)
                    .CreateScoped(CalendarService.Scope.Calendar);
            }

            _calendarService = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Litoral Polimentos"
            });
        }

        public async Task<string> CriarEventoAsync(
            string nomeCliente,
            string tipoDocumento,
            decimal valor,
            string? observacoes,
            string endereco,
            DateTime dataServico,
            TimeSpan horaServico)
        {
            var inicio = dataServico.Date + horaServico;
            var fim = inicio.AddHours(2); // duração padrão de 2 horas

            var evento = new Event
            {
                Summary = $"Serviço - {nomeCliente}",
                Location = endereco,
                Description =
                    $"Tipo: {tipoDocumento}\n" +
                    $"Valor: R$ {valor:N2}\n" +
                    $"Observações: {observacoes ?? "—"}",
                Start = new EventDateTime
                {
                    DateTimeDateTimeOffset = new DateTimeOffset(inicio),
                    TimeZone = "America/Sao_Paulo"
                },
                End = new EventDateTime
                {
                    DateTimeDateTimeOffset = new DateTimeOffset(fim),
                    TimeZone = "America/Sao_Paulo"
                },
                Reminders = new Event.RemindersData
                {
                    UseDefault = false,
                    Overrides = new List<EventReminder>
                    {
                        // Lembrete 1 dia antes
                        new EventReminder { Method = "email", Minutes = 1440 },
                        // Lembrete 1 hora antes
                        new EventReminder { Method = "popup", Minutes = 60 }
                    }
                }
            };

            var request = _calendarService.Events.Insert(evento, _calendarId);
            var eventoCriado = await request.ExecuteAsync();

            return eventoCriado.Id;
        }
    }
}