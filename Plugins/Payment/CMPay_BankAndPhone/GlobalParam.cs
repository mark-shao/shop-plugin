using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

//using log4net;

namespace Com.HisunCmpay
{
    public class GlobalParam
    {
        //log4net.ILog log = log4net.LogManager.GetLogger(typeof(GlobalParam));
        private static GlobalParam instance = null;
        private static String resourceFile = "profile/MerchantInfo.xml";
        private String mCharacterSet;

        public String characterSet
        {
            get { return mCharacterSet; }
            set { mCharacterSet = value; }
        }

        private String mMerchantId;

        public String merchantId
        {
            get { return mMerchantId; }
            set { mMerchantId = value; }
        }

       
        private String mReqUrl;

        public String reqUrl
        {
            get { return mReqUrl; }
            set { mReqUrl = value; }
        }
      

        private String mSignKey;

        public String signKey
        {
            get { return mSignKey; }
            set { mSignKey = value; }
        }

        private String mVersion;

        public String version
        {
            get { return mVersion; }
            set { mVersion = value; }
        }
        private String mSignType;

        public String signType
        {
            get { return mSignType; }
            set { mSignType = value; }
        }

        private String mCallbackUrl;

        public String callbackUrl
        {
            get { return mCallbackUrl; }
            set { mCallbackUrl = value; }
        }

     
        private String mNotifyUrl;

        public String notifyUrl
        {
            get { return mNotifyUrl; }
            set { mNotifyUrl = value; }
        }

       

        private GlobalParam()
        {
            init();
        }

        public static GlobalParam getInstance()
        {
            if (instance == null)
            {
                instance = new GlobalParam();
            }

            return instance;
        }

        /// <summary>
        /// 初始化全局参数
        /// </summary>
        private void init()
        {
            XmlDocument document = new XmlDocument();
            document.XmlResolver = null;
            try
            {
                string projectBase = System.AppDomain.CurrentDomain.BaseDirectory;
                document.Load(projectBase + resourceFile);
            }
            catch (Exception ee)
            {
                //log.Debug(ee.Message);
                return;
            }

            XmlElement root = document.DocumentElement;
            if (root == null)
            {
                //log.Info("The file '" + resourceFile + "' is empty!");
                return;
            
            }
            
            mCharacterSet = getXMLValue(root, "characterSet");
            mCallbackUrl = getXMLValue(root, "callbackUrl");
            mNotifyUrl = getXMLValue(root,"notifyUrl");
            mMerchantId = getXMLValue(root, "merchantId");
            mSignKey = getXMLValue(root, "signKey");
            mReqUrl = getXMLValue(root, "reqUrl");
            mVersion = getXMLValue(root, "version");
            mSignType = getXMLValue(root, "signType");
        }

        /// <summary>
        /// 取得节点值
        /// </summary>
        private String getXMLValue(XmlElement root, String name)
        {
            XmlNodeList nodeList = root.GetElementsByTagName(name);
            if (nodeList.Count <= 0)
            {
                //log.Warn("The" + name + "is not in the file '" + resourceFile + "'");
                return "";
            
            }
            XmlNode node = nodeList.Item(0);
            return node.FirstChild.Value;
        }
    }
}
