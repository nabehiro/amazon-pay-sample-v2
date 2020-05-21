using Amazon.Pay.API;
using Amazon.Pay.API.Types;
using Amazon.Pay.API.WebStore;
using Amazon.Pay.API.WebStore.CheckoutSession;
using Amazon.Pay.API.WebStore.Types;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        
        public string CreateButtonSignature(string payload)
        {
            var canonicalBuilder = new CanonicalBuilder();
            var signatureHelper = new SignatureHelper(_config, canonicalBuilder);
            
            string stringToSign = signatureHelper.CreateStringToSign(payload);

            return signatureHelper.GenerateSignature(stringToSign, _config.PrivateKey);
        }

        public string CreateCheckoutSessionPayload(string checkoutReviewReturnUrl)
        {
            // WARN:Following payload occured UnknownException on AmazonPay checkout page.
            //var request = new CreateCheckoutSessionRequest
            //(
            //    checkoutReviewReturnUrl: checkoutReviewReturnUrl,
            //    storeId: Options.StoreId
            //);

            //var addressRestrictions = request.DeliverySpecifications.AddressRestrictions;
            //addressRestrictions.Type = RestrictionType.Allowed;
            //addressRestrictions.Restrictions.Add("JP", new Restriction());

            //return request.ToJson();

            var payload = new
            {
                storeId = Options.StoreId,
                webCheckoutDetails = new
                {
                    checkoutReviewReturnUrl
                }
            };

            return JsonConvert.SerializeObject(payload);
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
            throw new NotImplementedException("WebStoreClient has no CompleteCheckoutSession method!");
        }
    }
}
