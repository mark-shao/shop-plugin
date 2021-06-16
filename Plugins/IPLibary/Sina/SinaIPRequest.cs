using Hishop.Plugins.IPLibary;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hishop.Plugins.IPLibary.Sina
{
    public class SinaIPRequest : Hishop.Plugins.IPlibaryRequest
    {
        #region 私有成员
        private string IPAddress;
        private string RequestUrl = "http://int.dpool.sina.com.cn/iplookup/iplookup.php?format=json&ip={0}";
        #endregion
        public SinaIPRequest(string UserIP, string Url)
        {
            if (!string.IsNullOrEmpty(UserIP) && UserIP != "")
                this.IPAddress = UserIP;
            if (!string.IsNullOrEmpty(Url) && Url != "")
                this.RequestUrl = Url;
        }

        public SinaIPRequest()
        {
            this.IPAddress = Globals.IPAddress;
        }

        public override IPData IPLocation()
        {
            RequestUrl = string.Format(RequestUrl, this.IPAddress);
            string result = Globals.Get(RequestUrl);
            try
            {
                JsonData jd = JsonMapper.ToObject(result);
                IPData ipdata = new IPData();
                ipdata.Country = jd["country"].ToString();
                ipdata.Province = jd["province"].ToString();
                ipdata.City = jd["city"].ToString();
                ipdata.ISP = jd["isp"].ToString();
                ipdata.IPAddress = IPAddress;
                //ipdata.Area = jd["area"].ToString();
                ipdata.CityCountry = "";
                return ipdata;
            }
            catch (Exception ex)
            {
                IDictionary<string, string> param = new Dictionary<string, string>();
                param.Add("IPAddress", IPAddress);
                Globals.AppendLog(param, result, RequestUrl, ex.Message);
            }
            return null;

        }
    }
}
