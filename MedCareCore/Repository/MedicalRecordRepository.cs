using MedCareCore.Data;
using MedCareCore.IRepository;
using MedCareCore.Models;
using Microsoft.EntityFrameworkCore;

namespace MedCareCore.Repository
{
    public class MedicalRecordRepository : Repository<MedicalRecord>, IMedicalRecordRepository
    {
        private readonly ApplicationDbContext _db;

        public MedicalRecordRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(MedicalRecord obj)
        {
            _db.MedicalRecords.Update(obj);
        }

        public void Detach(MedicalRecord entity)
        {
            _db.Entry(entity).State = EntityState.Detached;
        }
    }
}