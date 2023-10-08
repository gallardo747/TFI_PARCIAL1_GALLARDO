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
    public class AutogestionProcesadosController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public AutogestionProcesadosController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpPost]
        [Route("RegistrarDocumentoProcesado")]
        public IActionResult RegistrarDocumentoProcesado([FromBody] DocumentoARegistrar documento)
        {
            if (documento == null)
            {
                return BadRequest("Datos de Documento no proporcionados.");
            }
            try
            { 

            using (_unitOfWork)
            {
                var registroProcesados_BLL = new RegistroProcesados_BLL(_unitOfWork);

                registroProcesados_BLL.InsertDocumento(documento);

            }

            return Ok("Documento Registrado exitosamente.");
            }
            catch (AutogestionErroneaException ex)
            {
                // Captura de manera genérica las excepciones derivadas
                return BadRequest("Error en la Autogestión: " + ex.Message);
            }
        }

    }
}
