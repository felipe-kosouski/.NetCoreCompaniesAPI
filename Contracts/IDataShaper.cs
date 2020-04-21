using System.Collections.Generic;
using System.Dynamic;
using CompanyEmployees.Models;

namespace CompanyEmployees.Contracts
{
    public interface IDataShaper<T>
    {
         IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string fieldsString);
		 ShapedEntity ShapeData(T entity, string fieldsString);
    }
}