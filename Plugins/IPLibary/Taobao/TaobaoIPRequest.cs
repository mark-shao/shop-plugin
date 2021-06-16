using Hishop.Plugins.IPLibary;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hishop.Plugins.IPLibary.Taobao
{
    public class TaobaoIPRequest : Hishop.Plugins.IPlibaryRequest
    {
        #region 私有成员
        private string IPAddress;
        private string RequestUrl = "http://ip.taobao.com/service/getIpInfo.php?ip={0}";
        #endregion
        public TaobaoIPRequest(string UserIP, string Url)
        {
            if (!string.IsNullOrEmpty(UserIP) && UserIP != "")
                this.IPAddress = UserIP;
            if (!string.IsNullOrEmpty(Url) && Url != "")
                this.RequestUrl = Url;
        }

        public TaobaoIPRequest()
        {
            this.IPAddress = Globals.IPAddress;
        }

        public override IPData IPLocation()
        {
            RequestUrl = string.Format(RequestUrl, this.IPAddress);
            string result = Globals.Get(RequestUrl);
            try
            {
                //保存access_token，用于收货地址获取
                JsonData jd = JsonMapper.ToObject(result);
                if ((int)jd["code"] == 0)
                {
                    IPData ipdata = new IPData();
                    ipdata.Country = jd["data"]["country"].ToString();
                    ipdata.Province = jd["data"]["region"].ToString();
                    ipdata.City = jd["data"]["city"].ToString();
                    ipdata.ISP = jd["data"]["isp"].ToString();
                    ipdata.IPAddress = IPAddress;
                    ipdata.Area = "";
                    ipdata.CityCountry = "";
                    return ipdata;
                }
                else
                {
                    IDictionary<string, string> param = new Dictionary<string, string>();
                    param.Add("IPAddress", this.IPAddress);
                   // Globals.AppendLog(param, result, RequestUrl,"code");
                }
            }
            catch (Exception ex)
            {
                IDictionary<string, string> param = new Dictionary<string, string>();
                param.Add("IPAddress", this.IPAddress);
                Globals.AppendLog(param, result, RequestUrl, ex.Message);
            }
            return null;
        }
    }
}
