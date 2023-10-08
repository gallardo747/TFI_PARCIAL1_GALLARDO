using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GeneradorTraficoMensajes
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Bienvenido al Generador de Tráfico de Mensajes");

            int contadorMensajes = 0;

            while (true)
            {
                contadorMensajes++;

                Console.Write("Ingrese el nombre del documento: ");
                string nombre = Console.ReadLine();

                // Validar que el nombre no sea nulo o vacío
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    Console.WriteLine("El nombre del documento no puede estar vacío. Intente nuevamente.");
                    continue; // Volver a solicitar el nombre
                }

                Console.Write("Ingrese el cuerpo del documento: ");
                string cuerpo = Console.ReadLine();

                // Validar que el cuerpo no sea nulo o vacío
                if (string.IsNullOrWhiteSpace(cuerpo))
                {
                    Console.WriteLine("El cuerpo del documento no puede estar vacío. Intente nuevamente.");
                    continue; // Volver a solicitar el cuerpo
                }

                int prioridad;
                while (true)
                {
                    Console.Write("Ingrese la prioridad del mensaje (1-10): ");
                    if (int.TryParse(Console.ReadLine(), out prioridad) && prioridad >= 1 && prioridad <= 10)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("La prioridad debe ser un número entre 1 y 10. Intente nuevamente.");
                    }
                }

                // Crear un objeto JSON para enviar en el cuerpo de la solicitud
                var documento = new
                {
                    nombre = nombre,
                    cuerpo = cuerpo,
                    fecha_Envio = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                };


                // Convertir el objeto JSON en una cadena
                string jsonDocumento = Newtonsoft.Json.JsonConvert.SerializeObject(documento);

                // Crear un cliente HttpClient
                using (var client = new HttpClient())
                {
                    // URL de la API
                    string apiUrl = "https://localhost:7113/api/EnvioACola/EnviarDocumentoACola";

                    // Agregar el encabezado de prioridad
                    client.DefaultRequestHeaders.Add("Prioridad", prioridad.ToString());

                    // Configurar el contenido de la solicitud
                    var content = new StringContent(jsonDocumento, Encoding.UTF8, "application/json");

                    // Realizar una solicitud HTTP POST a la API
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    // Verificar la respuesta
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"\nDocumento {contadorMensajes} enviado con éxito a la API.");
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"\nError al enviar el documento {contadorMensajes} a la API. Código de estado: {response.StatusCode}");
                        Console.WriteLine($"Mensaje de error de la API: {errorMessage}");
                    }
                }

                Console.Write("\n¿Desea enviar otro mensaje? Presione 'S' para continuar, 'N' para salir: ");
                char respuesta = char.ToUpper(Console.ReadKey().KeyChar);
                if (respuesta != 'S')
                {
                    break;
                }

                Console.WriteLine();
            }

            Console.WriteLine("\nPresione cualquier tecla para salir.");
            Console.ReadKey();
        }
    }
}
