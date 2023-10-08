using System;
using System.Collections.Generic;
using System.Text;
using VETLY_BE.Entities;
using VETLY_DAL.Contracts;
using VETLY_DAL.Repository;
using RabbitMQ.Client;
using System.Text;

namespace LOGISTICA_BLL
{
    public class EnvioACola_BLL
    {
        private readonly UnitOfWork _unitOfWork;

        public EnvioACola_BLL(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public List<DocumentoAImprimir> GetEnvios()
        {
            var DocumentoRepository = _unitOfWork.DocumentoRepository;
            return DocumentoRepository.GetDocumentos();
        }


        public List<DocumentoProcesado> GetProcesadosByName(string nombre)
        {
            var DocumentoRepository = _unitOfWork.DocumentoRepository;
            return DocumentoRepository.GetDocumentosProcesadosByName(nombre);
        }
        

        public void InsertDocumento(DocumentoAImprimir pDocumento)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var documentoRepository = unitOfWork.DocumentoRepository;

                documentoRepository.InsertDocumento(pDocumento);
                unitOfWork.SaveChanges();
            }

        }

        public void EnviarDocumento(DocumentoAImprimir pDocumento, int prioridad)
        {
            // Conectarme a la Cola y enviar el documento
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var canal = connection.CreateModel())
                {
                    // Defino el nombre de la cola
                    var queueName = "MENSAJES_INPUT";

                    // Configurar la prioridad del mensaje
                    var properties = canal.CreateBasicProperties();
                    properties.Priority = (byte)prioridad; // Convertir la prioridad a byte

                    // Crear un objeto JSON que contenga el nombre y el cuerpo
                    var mensaje = new
                    {
                        nombre = pDocumento.Nombre,
                        cuerpo = pDocumento.Cuerpo
                    };

                    // Serializar el objeto a JSON
                    string mensajeJson = Newtonsoft.Json.JsonConvert.SerializeObject(mensaje);

                    // Convertir el mensaje serializado en bytes
                    var body = Encoding.UTF8.GetBytes(mensajeJson);

                    // Enviar el mensaje a la cola
                    canal.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    canal.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);

                    Console.WriteLine($"[x] Enviado: {mensajeJson}");
                }
            }

            // Registrarlo internamente (A modo de Bitácora de Envío)
            using (var unitOfWork = new UnitOfWork())
            {
                var documentoRepository = unitOfWork.DocumentoRepository;
                documentoRepository.InsertDocumento(pDocumento);
                unitOfWork.SaveChanges();
            }
        }

        public void SaveChanges()
        {
            _unitOfWork.SaveChanges();
        }
    }
}
