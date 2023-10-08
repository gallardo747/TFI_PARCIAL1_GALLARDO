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
    public class EnvioAColaProcesadosController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public EnvioAColaProcesadosController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpPost]
        [Route("EnviarDocumentoACola")]
        public IActionResult EnviarDocumentoACola([FromBody] DocumentoProcesado documento)
        {

            if (documento == null)
            {
                return BadRequest("Datos de Documento no proporcionados.");
            }
            try
            {
                using (_unitOfWork)
                {
                    var envioACola_BLL = new EnvioACola_BLL(_unitOfWork);

                    envioACola_BLL.EnviarDocumentoImpresoOK(documento);

                }

                return Ok("Documento enviado a la cola de PROCSADOS exitosamente.");
            }
            catch (ImpresionErroneaException ex)
            {
                // Captura de manera genérica las excepciones derivadas de ConsultaProfesionalErroneaException
                return BadRequest("Error en la Impresión: " + ex.Message);
            }
        }

    }
}
