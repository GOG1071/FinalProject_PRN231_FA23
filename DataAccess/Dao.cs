namespace DataAccess;

using BusinessObject.Models;

public abstract class Dao<T> : IDao<T> where T : class, IModel
{
    public static T Get(object[]? objects)
    {
        try
        {
            using var context = new Prn231DBContext();
            var list = context.Set<T>().ToList();
            return list.FirstOrDefault(x => x.PrimaryKey != null && x.PrimaryKey.SequenceEqual(objects));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
    public static T       Get(T t)                  { throw new NotImplementedException(); }
    public static List<T> GetAll()
    {
        try
        {
            using var context = new Prn231DBContext();
            return context.Set<T>().ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
    public static void Add(T t)
    {
        try
        {
            using var context = new Prn231DBContext();
            context.Set<T>().Add(t);
            context.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public static void Update(T t)
    {
        try
        {
            using var context = new Prn231DBContext();
            context.Set<T>().Update(t);
            context.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public static void Delete(object[]? objects)
    {
        try
        {
            using var context = new Prn231DBContext();
            var t = Get(objects);
            context.Set<T>().Remove(t);
            context.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}