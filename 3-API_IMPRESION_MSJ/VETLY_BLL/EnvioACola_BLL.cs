using System;
using System.Collections.Generic;
using System.Text;
using VETLY_BE.Entities;
using VETLY_DAL.Contracts;
using VETLY_DAL.Repository;
using RabbitMQ.Client;
using System.Text;
using Serilog;
using Serilog.Sinks.Seq;

namespace LOGISTICA_BLL
{
    public class EnvioACola_BLL
    {
        private readonly UnitOfWork _unitOfWork;

        public EnvioACola_BLL(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public void EnviarDocumentoImpresoOK(DocumentoProcesado pDocumento)
        {
            // PASO 1: SIMULO IMPRESION, para validar su OUTPUT
            Random random = new Random();
            int vRandomImpresion = random.Next(1, 11);

            string resultadoImpresion = (vRandomImpresion % 2 == 0) ? "ERROR" : "OK"; 

            Console.WriteLine($"Resultado de la impresión: {resultadoImpresion}");

            // Solo continuamos si la impresión es "OK"
            if (resultadoImpresion == "OK")
            {

                // Agregar la fecha de impresión actual antes de enviarlo a la cola
                pDocumento.Fecha_Impresion = DateTime.Now;
                //pDocumento.Nombre = 

                // PASO 2: EN CASO DE IMPRESION OK: Conectarme a la COLA DE PROCESADOS OK y enviar el documento

                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                {
                    using (var canal = connection.CreateModel())
                    {
                        // Defino el nombre de la cola
                        var queueName = "MENSAJES_PROCESADOS_OK";

                        // Serializo el objeto pDocumento a JSON
                        string mensaje = Newtonsoft.Json.JsonConvert.SerializeObject(pDocumento);

                        // Convierto el mensaje serializado en bytes
                        var body = Encoding.UTF8.GetBytes(mensaje);

                        // Declaramos la cola
                        canal.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                        // Enviamos el mensaje serializado a la cola
                        canal.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);

                        Console.WriteLine($"[x] Enviado {mensaje}");
                        
                    }
                }
            }
            else
            {
                // Registro de error con Serilog
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File("error.log", rollingInterval: RollingInterval.Day)
                    .CreateLogger();

                Log.Error("ERROR: No hay tinta para imprimir - Documento: {@Documento}", pDocumento);
                Console.WriteLine("La impresión fue un error. No se enviará a la cola.");
            }
        }

        public void SaveChanges()
        {
            _unitOfWork.SaveChanges();
        }
    }
}
