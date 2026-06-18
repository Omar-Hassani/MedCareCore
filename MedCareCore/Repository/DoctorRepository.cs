using MedCareCore.Data;
using MedCareCore.IRepository;
using MedCareCore.Models;

namespace MedCareCore.Repository
{
    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        private readonly ApplicationDbContext _db;

        public DoctorRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Doctor obj)
        {
            _db.Doctors.Update(obj);
        }
    }
}