using System;
using System.Collections.Generic;
using System.Linq;
using LOGISTICA_DAL.Data;
using Microsoft.EntityFrameworkCore;
using VETLY_BE.Entities;

namespace VETLY_DAL.Repository
{
    public class DocumentoRepository
    {
        private readonly LOGISTICAContext _context;

        public DocumentoRepository(LOGISTICAContext context)
        {
            _context = context;
        }



        public void InsertDocumento(DocumentoARegistrar pDocumentoAImprimir)
        {
            _context.DocumentoARegistrar.Add(pDocumentoAImprimir);
        }

    }
}
