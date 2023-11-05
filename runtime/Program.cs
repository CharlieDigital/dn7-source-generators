using System.Text.Json;
using System.Text.Json.Serialization;
using runtime.models;

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

var products = new ProductRepository();
await products.Add(new());

// Example of generating a class from a sample JSON snippet.
var json = JsonSerializer
  .Deserialize<SampleJsonLd>(
    SampleJsonLd.Json,
    new JsonSerializerOptions() {
      PropertyNameCaseInsensitive = true
    }
  );

if (json != null){
  Console.WriteLine($"Name: {json.Name}");
  Console.WriteLine($"Type: {json.Type}");
  Console.WriteLine($"BirthDate: {json.BirthDate}");
  Console.WriteLine($"Nationality: {json.Nationality}");
  Console.WriteLine($"SameAs Count: {json.SameAs.Length}");
}
