using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LOGISTICA_BLL;
using VETLY_DAL.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using VETLY_BE.Entities;
using VETLY_BLL.BusinessExceptions;
using Microsoft.AspNetCore.Connections;


namespace LOGISTICA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvioAColaController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public EnvioAColaController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        [Route("getDocumentosEnviados")]
        public IActionResult getDocumentosEnviados()
        {
            using (_unitOfWork)
            {
                var envioACola_BLL = new EnvioACola_BLL(_unitOfWork);
                var documento = envioACola_BLL.GetEnvios();
                return Ok(documento);
            }
        }

        [HttpGet]
        [Route("getDocumentosProcesadoByName")]
        public IActionResult getDocumentosProcesadoByName(string nombre)
        {
            using (_unitOfWork)
            {
                var envioACola_BLL = new EnvioACola_BLL(_unitOfWork);
                var documento = envioACola_BLL.GetProcesadosByName(nombre);
                return Ok(documento);
            }
        }


        [HttpPost]
        [Route("EnviarDocumentoACola")]
        public IActionResult EnviarDocumentoACola([FromBody] DocumentoAImprimir documento)
        {
            if (!Request.Headers.ContainsKey("Prioridad"))
            {
              return BadRequest("El encabezado Prioridad no se proporcionó.");
            }

            if (!int.TryParse(Request.Headers["Prioridad"], out int pPrioridad) || pPrioridad < 1 || pPrioridad > 10)
            {
            return BadRequest("El valor de Prioridad no es válido. Debe ser un número del 1 al 10.");
            }


            if (documento == null)
            {
                return BadRequest("Datos de Documento no proporcionados.");
            }

            try
            {
                using (_unitOfWork)
                {
                    var envioACola_BLL = new EnvioACola_BLL(_unitOfWork);

                    envioACola_BLL.EnviarDocumento(documento, pPrioridad);

                }

                return Ok("Documento enviado a la cola exitosamente.");
            }
            catch (EnvioMQErroneaException ex)
            {
                // Captura de manera genérica las excepciones
                return BadRequest("Error en el envío a la Cola: " + ex.Message);
            }
        }

    }
}
