using System;
using System.Collections.Generic;
using System.Text;

namespace Hishop.Plugins
{
    public class IPData
    {
        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 城市 
        /// </summary>
        public string City { set; get; }
        /// <summary>
        /// 区域,华中，华南，华北等
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// 所在区县
        /// </summary>
        public string CityCountry { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 完整地址
        /// </summary>
        public string FullAddress { get { return Country + (string.IsNullOrEmpty(Country) ? "" : " ") + Province + (string.IsNullOrEmpty(Province) ? "" : " ") + City  + (string.IsNullOrEmpty(City) ? "" : " ") + CityCountry + (string.IsNullOrEmpty(CityCountry) ? "" : " ") + Address; } }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// 网络提供商
        /// </summary>
        public string ISP { get; set; }
    }
}
