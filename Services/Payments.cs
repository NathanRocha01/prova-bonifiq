namespace ProvaPub.Services
{
    public interface IPaymentProcessor
    {
        string Method { get; }                  // "pix", "creditcard", "paypal"
        Task ProcessAsync(decimal value, int customerId);
    }

    public sealed class PixProcessor : IPaymentProcessor
    {
        public string Method => "pix";
        public Task ProcessAsync(decimal v, int c) => Task.CompletedTask;
    }
    public sealed class CreditCardProcessor : IPaymentProcessor
    {
        public string Method => "creditcard";
        public Task ProcessAsync(decimal v, int c) => Task.CompletedTask;
    }
    public sealed class PaypalProcessor : IPaymentProcessor
    {
        public string Method => "paypal";
        public Task ProcessAsync(decimal v, int c) => Task.CompletedTask;
    }
}
