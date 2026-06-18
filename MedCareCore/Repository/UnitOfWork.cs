using MedCareCore.Data;
using MedCareCore.IRepository;

namespace MedCareCore.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public IDoctorRepository Doctor { get; private set; }
        public IPatientRepository Patient { get; private set; }
        public IAppointmentRepository Appointment { get; private set; }
        public IMedicalRecordRepository MedicalRecord { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Doctor = new DoctorRepository(_db);
            Patient = new PatientRepository(_db);
            Appointment = new AppointmentRepository(_db);
            MedicalRecord = new MedicalRecordRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}