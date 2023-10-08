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

        public List<DocumentoAImprimir> GetDocumentos()
        {
            return _context.DocumentoAImprimir.ToList();
        }

        public List<DocumentoProcesado> GetDocumentosProcesadosByName(string nombre)
        {
            
            return _context.DocumentoProcesado
               .Where(DocumentoProcesado => DocumentoProcesado.Nombre.Equals(nombre))
               .ToList();
        }
        
        public void InsertDocumento(DocumentoAImprimir pDocumentoAImprimir)
        {
            _context.DocumentoAImprimir.Add(pDocumentoAImprimir);
        }

    }
}
