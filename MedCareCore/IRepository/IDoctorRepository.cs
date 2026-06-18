using MedCareCore.Models;

namespace MedCareCore.IRepository
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        void Update(Doctor obj);
    }
}