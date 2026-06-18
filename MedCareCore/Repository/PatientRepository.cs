using MedCareCore.Data;
using MedCareCore.IRepository;
using MedCareCore.Models;

namespace MedCareCore.Repository
{
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        private readonly ApplicationDbContext _db;

        public PatientRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Patient obj)
        {
            _db.Patients.Update(obj);
        }
    }
}