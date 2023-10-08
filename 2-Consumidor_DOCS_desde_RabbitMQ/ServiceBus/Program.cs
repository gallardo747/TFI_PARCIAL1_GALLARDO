using Consumidor_Patentes_Rabbit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using RabbitMQ.Client.Logging;
using System.Net;
using System.Security.Claims;
using Newtonsoft.Json;
using Consumidor_Docs_Rabbit;

doc xDoc = new doc();
var client = new HttpClient();
var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var canal = connection.CreateModel())
{
    canal.QueueDeclare(queue: "MENSAJES_INPUT", durable: false, exclusive: false, autoDelete: false, arguments: null);

    var consumidor = new EventingBasicConsumer(canal);
    consumidor.Received += async (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var mensaje = Encoding.UTF8.GetString(body);

        Console.WriteLine($"Documento a IMPRIMIR:  {mensaje}");

        string url = "https://localhost:7114/api/EnvioAColaProcesados/EnviarDocumentoACola";
        xDoc.cuerpo = mensaje;
        var json = JsonConvert.SerializeObject(xDoc);
        var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        var rest = await client.PostAsync(url, stringContent);

        if (rest.IsSuccessStatusCode)
        {
            string responseContent = await rest.Content.ReadAsStringAsync();
            Console.WriteLine($"Respuesta de la API: {responseContent}");
            Console.WriteLine($"Documento {mensaje} enviado con éxito a la API.");
        }
        else
        {
            string errorMessage = await rest.Content.ReadAsStringAsync();
            Console.WriteLine($"Error al enviar el documento {mensaje} a la API. Código de estado: {rest.StatusCode}");
            Console.WriteLine($"Mensaje de error de la API: {errorMessage}");
        }
    };

    canal.BasicConsume(queue: "MENSAJES_INPUT", autoAck: true, consumer: consumidor);

    Console.WriteLine("Running on localhost");
    Console.WriteLine("Press enter to exit");
    Console.ReadKey();
}
