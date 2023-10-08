using LOGISTICA_DAL.Data;
using VETLY_DAL.Repository;

namespace VETLY_DAL.Contracts
{
    public class UnitOfWork : IDisposable
    {
        private readonly LOGISTICAContext _context;

        public UnitOfWork()
        {
            _context = new LOGISTICAContext();
        }

        public DocumentoRepository DocumentoRepository => new DocumentoRepository(_context);
        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
