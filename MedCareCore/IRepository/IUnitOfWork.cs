namespace MedCareCore.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IDoctorRepository Doctor { get; }
        IPatientRepository Patient { get; }
        IAppointmentRepository Appointment { get; }
        IMedicalRecordRepository MedicalRecord { get; }

        // حفظ التغييرات لكل المستودعات دفعة واحدة
        void Save();
    }
}