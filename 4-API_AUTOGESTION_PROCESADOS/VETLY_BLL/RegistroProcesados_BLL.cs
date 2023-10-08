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
    public class RegistroProcesados_BLL
    {
        private readonly UnitOfWork _unitOfWork;

        public RegistroProcesados_BLL(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }



        public void InsertDocumento(DocumentoARegistrar pDocumento)
        {
            // Asignar la fecha y hora actual a la propiedad Fecha_Insercion
            pDocumento.Fecha_Insercion = DateTime.Now;

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
