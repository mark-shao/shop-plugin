using HiShop.API.Setting.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiShop.API.HiPOS.AdvancedAPIs.Merchant.MerchantJson
{
    public class MerchantResult : HiShopJsonResult
    {
        public MerchantUpdateResponse merchant_update_response { get; set; }
    }

    public class MerchantUpdateResponse
    {
        public string expire_at { get; set; }
    }

    public class AlipayKeyResult : HiShopJsonResult
    {
        public MerchantAlipayKeyResponse merchant_alipaykey_response { get; set; }
    }

    public class MerchantAlipayKeyResponse
    {
        public string public_key { get; set; }
    }


    public class HishopO2OResponse
    {
        public string message { get; set; }
    }

    public class HishopO2OResult : HiShopJsonResult
    {
        public HishopO2OResponse merchant_hishopo2o_response { get; set; }
    }


    public class PaymentsResponse
    {
        public string message { get; set; }
    }

    public class PaymentsResult : HiShopJsonResult
    {
        public PaymentsResponse merchant_payments_response { get; set; }
    }

    public class AuthCodeResponse
    {
        public string qr { get; set; }
    }

    public class AuthCodeResult : HiShopJsonResult
    {
        public AuthCodeResponse merchant_authcode_response { get; set; }
    }

    public class DeviceResponse
    {
        public string device_id { get; set; }
        public decimal total { get; set; }
        public int count { get; set; }

    }

    public class StoreResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public decimal total { get; set; }
        public int count { get; set; }

        public IEnumerable<DeviceResponse> devices { get; set; }
    }

    public class TradesResponse : IPaging
    {

        public int page { get; set; }
        public int page_size { get; set; }

        public int items_count { get; set; }

        public decimal total { get; set; }
        public int count { get; set; }
        public IEnumerable<StoreResponse> detail { get; set; }



    }

    public class TradesResult : HiShopJsonResult
    {
        public TradesResponse merchant_trades_response { get; set; }
    }


    public class DetailResult : HiShopJsonResult
    {
        public DetailResponse merchant_trades_detail_response { get; set; }
    }

    public class DetailResponse : IPaging
    {

        public int page { get; set; }

        public int page_size { get; set; }


        public IEnumerable<HiPOSResponse> detail { get; set; }
    }

    public class HiPOSResponse
    {
        public string id { get; set; }
        public string code { get; set; }
        public decimal amount { get; set; }
        public string method { get; set; }
        public string tid { get; set; }
        public string device_id { get; set; }

        public string paid_at { get; set; }

        public string method_alias { get; set; }
    }

}
