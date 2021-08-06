using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotvenues.Services
{
    public class LookUpDto
    {
        [Required]
        public long Id { get; set; }
        [Required, MaxLength(512)]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string Notes { get; set; }
    }

    public interface IModelService<T>
    {
        Task<T> FindAsync(long id);
        Task<List<T>> FetchAllAsync();
        Task<long> Save(T record);
        Task<long> Update(T record);
        Task<bool> Delete(long id);
    }

    interface IModelFilter<T>
    {
        IQueryable<T> BuildQuery(IQueryable<T> query);
    }

    public class Pager
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public int Skip() { return (Page - 1) * Size; }
    }

    public abstract class ModelFilter<T> : IModelFilter<T>
    {
        //[FromQuery(Name= "$pager")]
        public Pager Pager = new Pager();
        public abstract IQueryable<T> BuildQuery(IQueryable<T> query);
    }
}
