using MedCareCore.Models;

namespace MedCareCore.IRepository
{
    public interface IMedicalRecordRepository : IRepository<MedicalRecord>
    {
        void Update(MedicalRecord obj);
        void Detach(MedicalRecord entity);

    }
}