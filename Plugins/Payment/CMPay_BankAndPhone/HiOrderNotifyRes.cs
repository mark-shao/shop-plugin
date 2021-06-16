using System;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
//using log4net;

namespace Com.HisunCmpay
{
    /// <summary>
    /// 订单对象返回
    /// </summary>
    public class HiOrderNotifyRes 
    {
        //log4net.ILog log = log4net.LogManager.GetLogger(typeof(HiOrderNotifyRes));

        private String mMerchantId;
        private String mPayNo;
        private String mReturnCode;
        private String mMessage;
        private String mSignType;
        private String mType;
        private String mVersion;
        private String mHmac;
        private String mServerCert;
        private String mErrMsg;

        private String mAmount;
        private String mAmtItem;
        private String mBankAbbr;
        private String mMobile;
        private String mOrderId;
        private String mPayDate;
        private String mAccountDate;
        private String mReserved1;
        private String mReserved2;
        private String mStatus;
        private String mOrderDate;
        private String mFee;


        public String MerchantId
        {
            get { return mMerchantId; }
            set { mMerchantId = value; }
        }



        public String PayNo
        {
            get { return mPayNo; }
            set { mPayNo = value; }
        }



        public String ReturnCode
        {
            get { return mReturnCode; }
            set { mReturnCode = value; }
        }




        public String Message
        {
            get { return mMessage; }
            set { mMessage = value; }
        }



        public String SignType
        {
            get { return mSignType; }
            set { mSignType = value; }
        }



        public String Type
        {
            get { return mType; }
            set { mType = value; }
        }



        public String Version
        {
            get { return mVersion; }
            set { mVersion = value; }
        }



        public String Hmac
        {
            get { return mHmac; }
            set { mHmac = value; }
        }



        public String ServerCert
        {
            get { return mServerCert; }
            set { mServerCert = value; }
        }



        public String ErrMsg
        {
            get { return mErrMsg; }
            set { mErrMsg = value; }
        }



        public String Amount
        {
            get { return mAmount; }
            set { mAmount = value; }
        }


        public String AmtItem
        {
            get { return mAmtItem; }
            set { mAmtItem = value; }
        }


        public String BankAbbr
        {
            get { return mBankAbbr; }
            set { mBankAbbr = value; }
        }


        public String Mobile
        {
            get { return mMobile; }
            set { mMobile = value; }
        }



        public String OrderId
        {
            get { return mOrderId; }
            set { mOrderId = value; }
        }


        public String PayDate
        {
            get { return mPayDate; }
            set { mPayDate = value; }
        }

        public String AccountDate
        {
            get { return mAccountDate; }
            set { mAccountDate = value; }
        }

        public String Reserved1
        {
            get { return mReserved1; }
            set { mReserved1 = value; }
        }


        public String Reserved2
        {
            get { return mReserved2; }
            set { mReserved2 = value; }
        }


        public String Status
        {
            get { return mStatus; }
            set { mStatus = value; }
        }



        public String OrderDate
        {
            get { return mOrderDate; }
            set { mOrderDate = value; }
        }



        public String Fee
        {
            get { return mFee; }
            set { mFee = value; }
        }

        /// <summary>
        /// 后台通知验签
        /// </summary>
        /// <param name="param">手机支付平台返回的报文</param>
        public HiOrderNotifyRes(NameValueCollection param)
        {
            String resSource = IPosMUtil.keyValueToString(param);
            //log.Info("notify data: [" + resSource + "]");
            this.parseSource(resSource);
            String sour = getSource();
            if ("000000".Equals(mReturnCode))
            {
                if ("MD5".Equals(GlobalParam.getInstance().signType))
                {
                    if (!SignUtil.verifySign(sour, GlobalParam.getInstance().signKey, mHmac))
                    {
                        mErrMsg = "verify signature failed：" + "returnCode = " + mReturnCode + "&message = " + mMessage;
                        //log.Error(mErrMsg);
                        throw new Exception(mErrMsg);
                    }
                }
            }
            else
            {
                mErrMsg = "notify failed：" + "returnCode = " + mReturnCode + "&message = " + mMessage;
                //log.Error(mErrMsg);
                throw new Exception(mErrMsg);
            }
        }

       
        /// <summary>
        /// 取得手机支付平台返回的值
        /// </summary>
        /// <param name="source">&符合连接的的字符串</param>
        /// <returns>是否取得平台返回的值</returns>
        private Boolean parseSource(String source)
        {
            Hashtable ht = IPosMUtil.parseStringToMap(source);
            
            mHmac = (String)ht["hmac"];
            mMerchantId = (String)ht["merchantId"];
            mPayNo = (String)ht["payNo"];
            mReturnCode = (String)ht["returnCode"];
            mMessage = (String)ht["message"];
            mSignType = (String)ht["signType"];
            mType = (String)ht["type"];
            mVersion = (String)ht["version"];

            mAmount = (String)ht["amount"];
            mAmtItem = (String)ht["amtItem"];
            mBankAbbr = (String)ht["bankAbbr"];
            
            mMobile = (String)ht["mobile"];
            mOrderId = (String)ht["orderId"];
            mPayDate = (String)ht["payDate"];
            mAccountDate = (String)ht["accountDate"];
            mReserved1 = (String)ht["reserved1"];
            mReserved2 = (String)ht["reserved2"];
            mStatus = (String)ht["status"];
            mOrderDate = (String)ht["orderDate"];
            mFee = (String)ht["fee"];
           
            return true;
        }

        /// <summary>
        /// 组织需要生成mac值的原文
        /// </summary>
        /// <returns>生成mac值的原文</returns>
        private String getSource()
        {
            String source = "";
            source += mMerchantId;
            source += mPayNo;
            source += mReturnCode;
            source += mMessage;
            source += mSignType;
            source += mType;
            source += mVersion;
            source += mAmount;
            source += mAmtItem;
            source += mBankAbbr;
            source += mMobile;
            source += mOrderId;
            source += mPayDate;
            source += mAccountDate;
            source += mReserved1;
            source += mReserved2;
            source += mStatus;
            source += mOrderDate;
            source += mFee;
            return source;
        }

    }
}