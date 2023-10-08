using Consumidor_Patentes_Rabbit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Consumidor_Docs_Rabbit;

class Program
{
    static async Task Main(string[] args)
    {
        var client = new HttpClient();
        var factory = new ConnectionFactory() { HostName = "localhost" };

        using (var connection = factory.CreateConnection())
        using (var canal = connection.CreateModel())
        {
            canal.QueueDeclare(queue: "MENSAJES_PROCESADOS_OK", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumidor = new EventingBasicConsumer(canal);
            consumidor.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var mensaje = Encoding.UTF8.GetString(body);

                // Deserializar el mensaje en un objeto 'xDoc'
                var xDoc = JsonConvert.DeserializeObject<doc>(mensaje);

                // Deserializar el campo 'Cuerpo' para obtener el campo 'nombre'
                var cuerpoObj = JsonConvert.DeserializeObject<doc>(xDoc.cuerpo);
                xDoc.nombre = cuerpoObj.nombre;
                xDoc.cuerpo = cuerpoObj.cuerpo;
                

                string url = "https://localhost:7115/api/AutogestionProcesados/RegistrarDocumentoProcesado";

                var json = JsonConvert.SerializeObject(xDoc);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                var rest = await client.PostAsync(url, stringContent);

                if (rest.IsSuccessStatusCode)
                {
                    string responseContent = await rest.Content.ReadAsStringAsync();
                    Console.WriteLine($"Documento Procesado: {xDoc.nombre}");
                }
                else
                {
                    string errorMessage = await rest.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al enviar el documento {xDoc.nombre} a la API. Código de estado: {rest.StatusCode}");
                    Console.WriteLine($"Mensaje de error de la API: {errorMessage}");
                }
            };

            canal.BasicConsume(queue: "MENSAJES_PROCESADOS_OK", autoAck: true, consumer: consumidor);

            Console.WriteLine("Running on localhost - REGISTRO DE DOCUMENTOS EN COLA DE PROCESADOS_OK:");
            Console.WriteLine("Press enter to exit");
            Console.ReadKey();
        }
    }
}
