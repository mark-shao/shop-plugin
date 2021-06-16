using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hishop.Alipay.OpenHome;
using Hishop.Alipay.OpenHome.Model;
using Hishop.Alipay.OpenHome.Utility;

namespace UnitTest
{
    [TestClass]
    public class SerialiseTest
    {
        [TestMethod]
        public void Request_DeserialiseTest()
        {
            string xmlString = "<?xml version=\"1.0\" encoding=\"gbk\"?><XML><AppId><![CDATA[2014010300073039]]></AppId><FromUserId><![CDATA[234234234234]]></FromUserId><CreateTime><![CDATA[1389171239298]]></CreateTime><MsgType><![CDATA[event]]></MsgType><EventType><![CDATA[verifygw]]></EventType><ActionParam>abcdef</ActionParam><AgreementId>asdfasf</AgreementId><AccountNo><![CDATA[564645498]]></AccountNo></XML>";
            AliRequest request = XmlSerialiseHelper.Deserialize<AliRequest>(xmlString);
            Assert.AreEqual(request.AppId, "2014010300073039");
            Assert.AreEqual(request.FromUserId, "234234234234");
            Assert.AreEqual(request.CreateTime, 1389171239298);
            Assert.AreEqual(request.MsgType, "event");
            Assert.AreEqual(request.EventType, "verifygw");
            Assert.AreEqual(request.ActionParam, "abcdef");
            Assert.AreEqual(request.AgreementId, "asdfasf");
            Assert.AreEqual(request.AccountNo, "564645498");

        }


        //[TestMethod]
        //public void Response_Serialise()
        //{
        //    string bizContent="MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDfrBvcPJAzVX/5Ri8bjpraA7b0HsS0LVJWKtaj3kMXSub23KNoK2rPpno2lrdRK0Aw4BEkfJiJ9lzEtqV07RQqknFCfz4shARhhzqshj1wrlkVrdbso9Vo5PpGy4x/BN4qJeRwi+lxYdS1I97xDAE+v6rpos/QSa6+dXEE5gXIDwIDAQAB";
        //    bool success = true;
        //    string priKey="MIICWwIBAAKBgQDIjA3RjIS1uV+7dVITf9GoXBxdRBTp9EXl6rwo0M/gcWEaeszQNlRwA4LMg4OazY5WgNSAIniWAsmxNxkP5GX4wrSX9IkDbopXiJqnXuMnx5qNurLtxtQ034ax8meitiGNpfgIXXcugkXfGwbEurqFgNEdIxT9VPyS6DKRGLqxiQIDAQABAoGAFMJqq8Zf9m+5+S2r0Vjp2wOt6/mgMJeYpLQnxhHXcuRJqPQNP4BgPTgf0gvLE7szgMDVPm5TDTQ8rJuHfJ2n8GlFA/pYk99SdE+FoFbRM9i6S89pCJrxE1CnkJWTA0BkLzoK0RmUfU7eUwW9QvfFgIq9VBjuQqyXusOMUKI1QAECQQDrs1PkPOX7d0I34dS2UuM6fxXu2J3+gmeXdzb24glx9+lw3QDcz6lLrtKOLSbCpEJbgsHb7+SJ7GRixpT7mJ2ZAkEA2dGr79LS4yM/gUCtW9N87mKKwDLDjpiCWSjKyeBvejjADEYUnqHTk7xpYMONei60CtIF6N4EdYndyzFCz6DJcQJAOlWbwbVAxkfx35rI5ocFLgkeCIscL01fDiG3PMscw1Q0Nna8K+pJYC3Yds+99BZrRfLKwABTc79J6Rh07wAKoQJATgxWOoHldOP9blf5hky6mESRCRtnfSHimwYF295is33AOuWln83GnUpGzBhmCmDPvIzS90UOfftfkN5e3Tz00QJADegWg6FA/NV8bgiMc0jmD55DIpvN6p7S8/u8j2IeYIpQn5pTBk1RXUA952IRSqNlMnxs2SdiTdWd/CJNO4//YQ==";
        //    AliResponse response = new AliResponse(priKey, success, bizContent);
        //    string expected="<?xmlversion=\"1.0\"encoding=\"utf-16\"?><alipay xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><response><success>true</success><biz_content>MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDfrBvcPJAzVX/5Ri8bjpraA7b0HsS0LVJWKtaj3kMXSub23KNoK2rPpno2lrdRK0Aw4BEkfJiJ9lzEtqV07RQqknFCfz4shARhhzqshj1wrlkVrdbso9Vo5PpGy4x/BN4qJeRwi+lxYdS1I97xDAE+v6rpos/QSa6+dXEE5gXIDwIDAQAB</biz_content></response></alipay>";
        //    string actual = XmlSerialiseHelper.Serialise<AliResponse>(response).Replace(" ", "").Replace("\r\n", "");
        //    Assert.AreEqual(expected, actual);
        //}


        //[TestMethod]
        //public void RSATest()
        //{
        //    string priKey = "MIICeAIBADANBgkqhkiG9w0BAQEFAASCAmIwggJeAgEAAoGBAK5DenevdvrGDh82HvODr05dgT5Vs44uBE5v9bInA5L2QAm5fdeewOn4XgKb67fjaIuFpnmYrqv0QkQ/xyLxsh5Swc6RxZKxhur5+snPZRoMJe1yWj5a5yDW8BpoBNd4p3CwbvI1Uomj45UBrgO8WJP3oPm194zKgE9t/RewRnIZAgMBAAECgYEAjrhBEsin3xZZ9oHys2mEJ+A4u36hAa76Y+saBIAjUzdOlyhNwkwInxaEmApu8TnUI4CgKTI8zCPOKes6AoqEXypd5w9c5Cy9X6KpxA6B3y/HYwhfBshSKHETwvVPnXt7yUVt5lCVmLMTYqxy6vNY4bsp1L1zyZC7nSlrqnNdIYkCQQDj1AxVL6cJttebyfZEuzVAz0ORMQ7JA3feJFpBGAkGT0m1xSkcsgQAwMmZ5EVy18UcLrdPPckMPVcSNeO3beNvAkEAw8/T72VNVlxf3v/oE/W2MpYwnrsDe8ERV6umxeQQmroJ4RLoRH9lcI8ynnLpepA6QeNzT75WqVY4clMVpHce9wJAK5qG7brcDljnNRLXRlpKG+hPNzeba891Hpf4iiLOf12nbtmYP7y2VWqQaxqsTAmN2RH71Xeuxd/rjTmxJocqkwJBAL8ZnWUOIKQgExC3/+865k4Idfrz2Tp1+k7tnx2SrwHSfsHCihwPmRh5KJWq4GJVUAXtzSQORtWM6BnrLDwOZZECQQCIx0o5RXlabjEh7Ww5+BucWOiKyFKpdM8QfWc/aCIYUX4b+TJGEq2tGkYEJOxj2aHMisW+KWEygnwy4ND6+Hhh";
        //    string pubKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCnxj/9qwVfgoUh/y2W89L6BkRAFljhNhgPdyPuBV64bfQNN1PjbCzkIM6qRdKBoLPXmKKMiFYnkd6rAoprih3/PrQEB/VsW8OoM8fxn67UDYuyBTqA23MML9q1+ilIZwBC2AQ2UBVOrFXfFl75p6/B5KsiNG9zpgmLCUYuLkxpLQIDAQAB";
        //    string content = "<success>true</success><biz_content>MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCnxj/9qwVfgoUh/y2W89L6BkRAFljhNhgPdyPuBV64bfQNN1PjbCzkIM6qRdKBoLPXmKKMiFYnkd6rAoprih3/PrQEB/VsW8OoM8fxn67UDYuyBTqA23MML9q1+ilIZwBC2AQ2UBVOrFXfFl75p6/B5KsiNG9zpgmLCUYuLkxpLQIDAQAB</biz_content>";
        //    string actual = AlipayOHClient.encryptAndSign(content, pubKey, priKey, "GBK", false, true);
        //    Assert.IsTrue(actual.Contains("OK"));
        //}

        [TestMethod]
        public void CDataSerialize()
        {
            Item message = new Item() { Description = "this is description", ImageUrl = "/adfads.jpg", Title = "this is title", Url = "afdasdfasd/afsdaf.html" };
            string actual = XmlSerialiseHelper.Serialise<Item>(message);

            Assert.IsTrue(actual.Contains("CDATA"));


        }

    }
}
