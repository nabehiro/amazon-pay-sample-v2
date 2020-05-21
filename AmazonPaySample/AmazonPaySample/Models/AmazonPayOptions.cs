using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmazonPaySample.Models
{
    public class AmazonPayOptions
    {
        public string MerchantId { get; set; }
        public string StoreId { get; set; }
        public string PublicKeyId { get; set; }
        public string PrivateKey { get; set; }
    }
}
