namespace runtime.models;

/// <summary>
/// A partial class that extends the generated class.
/// </summary>
public partial class OrderRepository {

  /// <summary>
  /// This method doesn't exist in the contract, but we can extend the generated
  /// code using partial classes.
  /// </summary>
  /// <returns>For this use case, just an empty array.</returns>
  public async Task UpdateIfNotShipped(
    Order entity
  ) {
    await Task.CompletedTask;
    Console.WriteLine("Order → UpdateIfNotShipped");
  }
}