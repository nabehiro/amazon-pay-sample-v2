using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmazonPaySample.Models
{
    public class FrontCartViewModel
    {
        public string Payload { get; set; }
        public string Signature { get; set; }
        public string PublicKeyId { get; set; }
        public string MerchantId { get; set; }
    }
}
