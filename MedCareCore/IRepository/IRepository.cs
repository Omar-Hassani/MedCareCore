using System.Linq.Expressions;

namespace MedCareCore.IRepository
{
    public interface IRepository<T> where T : class
    {
        // جلب كل البيانات مع إمكانية عمل Include للجداول المرتبطة
        IEnumerable<T> GetAll(string? includeProperties = null);

        // جلب عنصر واحد بناءً على شرط معينة
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);

        // إضافة عنصر جديد
        void Add(T entity);

        // حذف عنصر
        void Remove(T entity);
    }
}