using MedCareCore.Models;

namespace MedCareCore.IRepository
{
    public interface IPatientRepository : IRepository<Patient>
    {
        void Update(Patient obj);
    }
}