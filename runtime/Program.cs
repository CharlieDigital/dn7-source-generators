using runtime;

// Fully generated
var users = new UserRepository();

await users.Add(new ());
await users.Delete(new ());
await users.Update(new());

// Generated + partial class in this namespace.
var orders = new OrderRepository();

await orders.Add(new ());
await orders.Delete(new ());
await orders.Update(new ());
await orders.UpdateIfNotShipped(new()); // Added via partial