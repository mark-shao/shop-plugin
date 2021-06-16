using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hishop.Plugins.Payment
{
    public class PayUserInfo
    {
        /// <summary>
        /// 商户系统内针对会员的唯一标识，不同会员的标识必须唯一
        /// </summary>
        public string outMemberId { get; set; }
        /// <summary>
        /// 商户会员注册时间yyyyMMddHHmmss,如:20110707112233
        /// </summary>
        public string outMemberRegisterTime
        {
            get
            {
                if (string.IsNullOrEmpty(_outMemberRegisterTime))
                {
                    return "20161205151515";
                }
                else
                {
                    return _outMemberRegisterTime;
                }
            }
            set
            {
                _outMemberRegisterTime = value;
            }
        }
        /// <summary>
        /// 商户会员注册IP
        /// </summary>
        public string outMemberRegistIP
        {
            get
            {
                if (string.IsNullOrEmpty(_outMemberRegistIP))
                {
                    return "127.0.0.1";
                }
                else
                {
                    return _outMemberRegistIP;
                }
            }
            set { _outMemberRegistIP = value; }
        }
        /// <summary>
        /// 是否进行实名认证
        /// </summary>
        public Int32 outMemberVerifyStatus { get; set; }
        /// <summary>
        /// 商户会员注册姓名
        /// </summary>
        public string outMemberName { get; set; }
        /// <summary>
        /// 商户会员注册手机号
        /// </summary>
        public string outMemberMobile { get; set; }
        /// <summary>
        /// 默认值未进行实名认证
        /// </summary>
        private Int32 _outMemberVerifyStatus = 0;
        private string _outMemberRegistIP = "127.0.0.1";
        /// <summary>
        /// 备注
        /// </summary>
        public string attach { get; set; }

        private string _outMemberRegisterTime = "20161205151515";


    }
}
