using Amazon.Pay.API;
using Amazon.Pay.API.Types;
using Amazon.Pay.API.WebStore;
using Amazon.Pay.API.WebStore.CheckoutSession;
using Amazon.Pay.API.WebStore.Types;
using Microsoft.Extensions.Options;

namespace AmazonPaySample.Models
{
    public class AmazonPayHelper
    {
        public AmazonPayOptions Options { get; }

        private readonly ApiConfiguration _config;

        public AmazonPayHelper(IOptions<AmazonPayOptions> options)
        {
            Options = options.Value;

            _config = new ApiConfiguration
            (
                region: Amazon.Pay.API.Types.Region.Japan,
                environment: Amazon.Pay.API.Types.Environment.Sandbox,
                publicKeyId: Options.PublicKeyId,
                privateKey: Options.PrivateKey
            );
        }

        public WebStoreClient CreateWebStoreClient() => new WebStoreClient(_config);

        public string CreateButtonSignature(CreateCheckoutSessionRequest request)
        {
            var client = new WebStoreClient(_config);
            return client.GenerateButtonSignature(request);
        }

        public CreateCheckoutSessionRequest CreateCheckoutSessionRequest(string checkoutReviewReturnUrl)
        {
            var request = new CreateCheckoutSessionRequest
            (
                checkoutReviewReturnUrl: checkoutReviewReturnUrl,
                storeId: Options.StoreId
            );

            var addressRestrictions = request.DeliverySpecifications.AddressRestrictions;
            addressRestrictions.Type = RestrictionType.Allowed;
            addressRestrictions.Restrictions.Add("JP", new Restriction());

            return request;
        }

        public CheckoutSessionResponse GetCheckoutSession(string checkoutSessionId)
        {
            var client = new WebStoreClient(_config);
            return client.GetCheckoutSession(checkoutSessionId);
        }

        public CheckoutSessionResponse UpdateCheckoutSession(string checkoutSessionId,
            string merchantReferenceId, decimal amount, string checkoutResultReturnUrl)
        {
            var request = new UpdateCheckoutSessionRequest();
            request.WebCheckoutDetails.CheckoutResultReturnUrl = checkoutResultReturnUrl;
            request.PaymentDetails.ChargeAmount.Amount = amount;
            request.PaymentDetails.ChargeAmount.CurrencyCode = Amazon.Pay.API.Types.Currency.JPY;
            request.PaymentDetails.PaymentIntent = PaymentIntent.Authorize;
            request.MerchantMetadata.MerchantReferenceId = merchantReferenceId;
            request.MerchantMetadata.MerchantStoreName = "AMAZONPAY SAMPLE SHOP";
            request.MerchantMetadata.NoteToBuyer = "THANK YOU!";

            var client = new WebStoreClient(_config);
            return client.UpdateCheckoutSession(checkoutSessionId, request);
        }

        public CheckoutSessionResponse CompleteCheckoutSession(string checkoutSessionId,
            decimal amount)
        {
            var request = new CompleteCheckoutSessionRequest(amount, Currency.JPY);

            var client = new WebStoreClient(_config);
            return client.CompleteCheckoutSession(checkoutSessionId, request);
        }
    }
}