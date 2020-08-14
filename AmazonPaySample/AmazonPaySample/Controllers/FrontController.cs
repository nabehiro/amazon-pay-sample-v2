using AmazonPaySample.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AmazonPaySample.Controllers
{
    public class FrontController : Controller
    {
        private readonly AmazonPayHelper _amazonPayHelper;

        public FrontController(AmazonPayHelper amazonPayHelper)
        {
            _amazonPayHelper = amazonPayHelper;
        }

        public IActionResult Cart()
        {
            var model = new FrontCartViewModel
            {
                MerchantId = _amazonPayHelper.Options.MerchantId,
                PublicKeyId = _amazonPayHelper.Options.PublicKeyId,
            };

            var returnUrl = Url.Action("Confirm", null, null, Request.Scheme);

            var createCheckoutSessionRequest = _amazonPayHelper.CreateCheckoutSessionRequest(returnUrl);

            model.Payload = createCheckoutSessionRequest.ToJson();
            model.Signature = _amazonPayHelper.CreateButtonSignature(createCheckoutSessionRequest);

            return View(model);
        }

        public IActionResult Confirm(string amazonCheckoutSessionId)
        {
            if (amazonCheckoutSessionId == null)
                throw new ArgumentNullException(nameof(amazonCheckoutSessionId));

            var model = new FrontConfirmViewModel
            {
                CheckoutSession = _amazonPayHelper.GetCheckoutSession(amazonCheckoutSessionId)
            };

            return View(model);
        }

        private const decimal Amount = 999;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Purchase(string amazonCheckoutSessionId)
        {
            var returnUrl = Url.Action("Complete", null, null, Request.Scheme);

            var checkoutSession = _amazonPayHelper.UpdateCheckoutSession(
                checkoutSessionId: amazonCheckoutSessionId,
                merchantReferenceId: "SAMPLE-" + amazonCheckoutSessionId,
                amount: Amount,
                checkoutResultReturnUrl: returnUrl);

            // OK
            if (checkoutSession.Success &&
                !string.IsNullOrEmpty(checkoutSession.WebCheckoutDetails.AmazonPayRedirectUrl))
            {
                return Redirect(checkoutSession.WebCheckoutDetails.AmazonPayRedirectUrl);
            }

            // NG
            var model = new FrontConfirmViewModel
            {
                CheckoutSession = checkoutSession
            };

            return View("Confirm", model);
        }

        public IActionResult Complete(string amazonCheckoutSessionId)
        {
            var completeCheckoutSession = _amazonPayHelper.CompleteCheckoutSession(amazonCheckoutSessionId, Amount);

            var model = new FrontCompleteViewModel
            {
                CheckoutSession = completeCheckoutSession,
            };

            return View(model);
        }
    }
}