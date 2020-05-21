using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Pay.API.WebStore;
using Amazon.Pay.API.WebStore.CheckoutSession;
using AmazonPaySample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;

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
            model.Payload = _amazonPayHelper.CreateCheckoutSessionPayload(returnUrl);

            model.Signature = _amazonPayHelper.CreateButtonSignature(model.Payload);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Purchase(string amazonCheckoutSessionId)
        {
            var returnUrl = Url.Action("Complete", null, null, Request.Scheme);

            var checkoutSession = _amazonPayHelper.UpdateCheckoutSession(
                checkoutSessionId: amazonCheckoutSessionId,
                merchantReferenceId: "COMMERBLE-" + amazonCheckoutSessionId,
                amount: 999,
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
    }
}