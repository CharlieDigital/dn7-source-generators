namespace runtime;

public interface IRepository<T> where T: Entity {
  Task Add(T Entity);

  Task Delete(T Entity);

  Task Update(T Entity);
}

public abstract class RepositoryBase<T>
  : IRepository<T> where T: Entity {
  public virtual async Task Add(T Entity) {
    await Task.CompletedTask;
    Console.WriteLine($"Added → {typeof(T).Name}");
  }

  public virtual async Task Delete(T Entity) {
    await Task.CompletedTask;
    Console.WriteLine($"Deleted → {typeof(T).Name}");
  }

  public virtual async Task Update(T Entity) {
    await Task.CompletedTask;
    Console.WriteLine($"Updated → {typeof(T).Name}");
  }
}

public abstract class Entity {

}

public class User : Entity {

}

public class Order : Entity {

}