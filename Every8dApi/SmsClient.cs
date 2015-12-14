using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;

namespace Every8dApi
{
    #region Every8D企業簡訊平台
    
    /// <summary>
    /// WebService:
    ///     https://oms.every8d.com/API21/SOAP/SMS.asmx
    /// Api Doc:
    ///     http://tw.every8d.com/api20/doc/EVERY8D%20Web%20Services%20API%E6%96%87%E4%BB%B6-v2%201-https.pdf
    /// Example: 
    ///     http://tw.every8d.com/E8DPortal/InformationDownload.aspx
    ///     (技術文件與程式下載 (中文版) -> 企業簡訊應用 API 2.1範例程式)
    /// </summary>
    public class SMSClient : IDisposable
    {
        private string userID;
        private string password;
        private string sessionKey;
        private double credit;

        private com.every8d.oms.SMS smsService;

        public SMSClient(string userID, string password)
        {
            this.userID = userID;
            this.password = password;
            this.smsService = new com.every8d.oms.SMS();

            this.IsAvailable = GetConnection(userID, password);
        }

        public bool IsAvailable { get; private set; }

        // 目前沒有這個需求
        //public string sendParamSMS(string sessionKey, string subject, string contentXML, string sendTime) { }

        #region 1.1 建立連線

        private bool GetConnection(string userID, string password) 
        {
            string resultXml = this.smsService.getConnection(userID, password);

            var result = DeserializeXml<SMS_GET_CONNECTION>(resultXml);

            /*
            0：取得連線成功 
            -100 ：取得連線失敗，無此帳號
            -101 ：取得連線失敗，密碼錯誤
            -999 ：帳號已封鎖 ：帳號已封鎖
             */
            if (result.GET_CONNECTION.CODE != 0)
            {
                //throw new Exception(string.Format("取得連線失敗 {0}", result.GET_CONNECTION.DESCRIPTION));
                return false;
            }
            else
            {
                this.sessionKey = result.GET_CONNECTION.SESSION_KEY;
                return true;
            }
        }
        #endregion

        #region 1.2 簡訊發送

        public const string SendTimeFormat = "yyyyMMddHHmmss";

        /// <summary>
        /// Sends the SMS.
        /// </summary>
        /// <param name="subject">簡訊類別描述，發送紀錄查詢時參考用，可不填</param>
        /// <param name="content">簡訊發送 內容</param>
        /// <param name="mobile">格式為 : +886912345678，多筆接收人時，請以逗點隔開</param>
        /// <param name="sendTime">
        /// 簡訊預定發送時間
        ///     -立即發送：if sendTime = null
        ///     -預約發送：請傳入計時間，若小於系統接單將不予，若傳遞時間已逾現在之，將立即發送。
        /// </param>
        /// <returns></returns>
        private SEND_SMS_RESULT SendSMS(DateTime? sendTime, string subject, string content, params string[] mobiles)
        {
            EnsureSessionKey();
            
            // "4.0,1,1.0,0,65408eb7-1df2-449c-9cb9-4a9162c70cda"
            string resultString = this.smsService.sendSMS(
                this.sessionKey, 
                subject, 
                content, 
                string.Join(",", mobiles),
                sendTime.HasValue ? sendTime.Value.ToString(SMSClient.SendTimeFormat) : string.Empty);
            /* 
             * 傳送成功 回傳字串內容格式為：CREDIT,SENDED,COST,UNSEND,BATCH_ID，各值中間以逗號分隔。
             * CREDIT：發送後剩餘點數。負值代表發送失敗，系統無法處理該命令
             * SENDED：發送通數。
             * COST：本次發送扣除點數
             * UNSEND：無額度時發送的通數，當該值大於0而剩餘點數等於0時表示有部份的簡訊因無額度而無法被發送。
             * BATCH_ID：批次識別代碼。為一唯一識別碼，可藉由本識別碼查詢發送狀態。格式範例：220478cc-8506-49b2-93b7-2505f651c12e
             */
            string[] tokens = resultString.Split(',');
            if (tokens.Length != 5)
                throw new Exception(string.Format("解析錯誤 {0}", resultString));

            SEND_SMS_RESULT result = new SEND_SMS_RESULT();
            result.CREDIT = Convert.ToDouble(tokens[0]);
            result.SENDED = Convert.ToInt32(tokens[1]);
            result.COST = Convert.ToDouble(tokens[2]);
            result.UNSEND = Convert.ToInt32(tokens[3]);
            result.BATCH_ID = tokens[4];

            return result;
        }



        /// <summary>
        /// Sends the SMS.
        /// </summary>
        /// <param name="sendTime">
        /// 簡訊預定發送時間
        ///     -立即發送：if sendTime = null
        ///     -預約發送：請傳入計時間，若小於系統接單將不予，若傳遞時間已逾現在之，將立即發送。
        /// </param>
        /// <returns></returns>
        public SEND_SMS_RESULT SendParamSMS(DateTime? sendTime, string subject, List<Every8d_MessageReceiver> messageReceivers)
        {
            EnsureSessionKey();

            string contentXML = string.Format("<REPS>{0}</REPS>", string.Join("", messageReceivers.Select(p => p.ToString())));

            // "4.0,1,1.0,0,65408eb7-1df2-449c-9cb9-4a9162c70cda"
            string resultString = this.smsService.sendParamSMS(
                this.sessionKey, 
                subject ?? string.Empty, 
                contentXML, 
                sendTime.HasValue ? sendTime.Value.ToString(SendTimeFormat) : string.Empty);

            /* 
             * 傳送成功 回傳字串內容格式為：CREDIT,SENDED,COST,UNSEND,BATCH_ID，各值中間以逗號分隔。
             * CREDIT：發送後剩餘點數。負值代表發送失敗，系統無法處理該命令
             * SENDED：發送通數。
             * COST：本次發送扣除點數
             * UNSEND：無額度時發送的通數，當該值大於0而剩餘點數等於0時表示有部份的簡訊因無額度而無法被發送。
             * BATCH_ID：批次識別代碼。為一唯一識別碼，可藉由本識別碼查詢發送狀態。格式範例：220478cc-8506-49b2-93b7-2505f651c12e
             */
            string[] tokens = resultString.Split(',');
            if (tokens.Length != 5)
                throw new Exception(string.Format("解析錯誤 {0}", resultString));

            SEND_SMS_RESULT result = new SEND_SMS_RESULT();
            result.CREDIT = Convert.ToDouble(tokens[0]);
            result.SENDED = Convert.ToInt32(tokens[1]);
            result.COST = Convert.ToDouble(tokens[2]);
            result.UNSEND = Convert.ToInt32(tokens[3]);
            result.BATCH_ID = tokens[4];

            return result;
        }
        #endregion

        #region 1.3 關閉連線

        private void CloseConnection() 
        {
            if (!string.IsNullOrEmpty(this.sessionKey))
            {
                string resultString = this.smsService.closeConnection(this.sessionKey);
                /*
                “1” :關閉連線成功
                “-1”:關閉連線失敗
                */
                if (resultString == "1")
                {
                    this.sessionKey = string.Empty;
                }
            }
        }
        #endregion

        #region 3.2 發送狀態查詢(取得派送結果)

        /// <summary>
        /// Gets the delivery status.
        /// </summary>
        /// <param name="batchID">發送識別碼</param>
        /// <param name="pageNo">可傳入空字串，若查詢筆數超過1000筆時，欲查詢第1001~2000傳入 ”2”。2001~3000 筆時， 傳入 ”3”。 依此類推。</param>
        /// <returns></returns>
        public SMS_LOG GetDeliveryStatus(string batchID, string pageNo = "")
        {
            EnsureSessionKey();

            string resultXml = this.smsService.getDeliveryStatus(this.sessionKey, batchID, pageNo);

            var result = DeserializeXml<SMS_LOG>(resultXml);

            return result;
        }

        #endregion

        #region 4.1 餘額查詢

        public double GetCredit() 
        {
            EnsureSessionKey();

            string resultString = this.smsService.getCredit(this.sessionKey);

            this.credit = Convert.ToDouble(resultString);

            return this.credit;
        }

        #endregion

        #region 4.2 刪除簡訊預約

        public ERASE_BOOKING_RESULT EraseBooking(string batchID)
        {
            EnsureSessionKey();

            string resultString = this.smsService.eraseBooking(this.sessionKey, batchID);
            /* 
             * 傳送成功 回傳字串內容格式為：CREDIT,SENDED,COST,UNSEND,BATCH_ID，各值中間以逗號分隔。
             * CREDIT：刪除的筆數
             * SENDED：回補的點數。
             */
            string[] tokens = resultString.Split(',');
            if (tokens.Length != 2)
                throw new Exception(string.Format("解析錯誤 {0}", resultString));

            ERASE_BOOKING_RESULT result = new ERASE_BOOKING_RESULT();
            result.DELETED = Convert.ToInt32(tokens[0]);
            result.CLAW_BACK = Convert.ToDouble(tokens[1]);

            return result;
        }

        #endregion

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.CloseConnection();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void EnsureSessionKey()
        {
            if (string.IsNullOrEmpty(sessionKey))
                throw new Exception("尚未連線至 Every8d");
        }

        private T DeserializeXml<T>(string xmlDocumentText)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(xmlDocumentText))
            {
                return (T)(serializer.Deserialize(reader));
            }
        }
    }

    #endregion

    #region 連線結果
    /*
    <?xml version=\"1.0\" encoding=\"UTF-8\"?>
    <SMS>
        <GET_CONNECTION>
            <CODE>0</CODE>
            <SESSION_KEY>edc603f14336483ea71e4998f555b504</SESSION_KEY>
            <DESCRIPTION>取得連線成功。</DESCRIPTION>
        </GET_CONNECTION>
    </SMS>
     */

    [XmlRoot("SMS")]
    public class SMS_GET_CONNECTION
    {
        public GET_CONNECTION GET_CONNECTION { get; set; }
    }

    public class GET_CONNECTION
    {
        public int CODE { get; set; }
        public string SESSION_KEY { get; set; }
        public string DESCRIPTION { get; set; }
    }
    #endregion

    #region 發送簡訊的回傳值

    public class SEND_SMS_RESULT
    {
        /// <summary>
        /// 發送後剩餘點數
        /// 
        /// 負值 代表發送失敗，系統無法處理該命令
        /// </summary>
        public double CREDIT { get; set; }

        /// <summary>
        /// 發送通數。
        /// </summary>
        public int SENDED { get; set; }

        /// <summary>
        /// 本次發送扣除點數
        /// </summary>
        public double COST { get; set; }

        /// <summary>
        /// 無額度時發送的通數
        /// 
        /// 當該值大於0，而剩餘點數等於0時，表示有部份的簡訊因無額度而法被發送。
        /// </summary>
        public int UNSEND { get; set; }

        /// <summary>
        /// 發送識別碼，可藉由此識別碼查詢本次發送狀態。
        /// </summary>
        public string BATCH_ID { get; set; }

        public override string ToString()
        {
            return string.Format("SEND_SMS_RESULT(CREDIT: {0}，SENDED: {1}，COST: {2}，UNSEND: {3}，BATCH_ID: {4})",
                CREDIT,
                SENDED,
                COST,
                UNSEND,
                BATCH_ID);
        }
    }

    #endregion

    #region 派送結果

    // 請使用 專案 EFunTech.Sms.Schema 中的 Every8d_DeliveryReportStatus
    //public enum DELIVERY_STATUS
    //{
    //    Unknown = -1,

    //    /// <summary>
    //    /// 已發送
    //    /// </summary>
    //    Sent = 0,

    //    /// <summary>
    //    /// 發送成功
    //    /// </summary>
    //    DeliveredToTerminal = 100,

    //    /// <summary>
    //    /// 手機端因素未能送達
    //    /// </summary>
    //    TerminalUncertain = 101,

    //    /// <summary>
    //    /// 無此手機號碼
    //    /// </summary>
    //    WrongPhoneNumber = 103,

    //    /// <summary>
    //    /// 電信終端設備異常未能送達
    //    /// </summary>
    //    NetworkUncertain102 = 102,
    //    NetworkUncertain104 = 104,
    //    NetworkUncertain105 = 105,
    //    NetworkUncertain106 = 106,

    //    /// <summary>
    //    /// 逾時收訊
    //    /// </summary>
    //    Timeout = 107,
    //}

    /*
    <?xml version="1.0" encoding="UTF-8"?>
    <SMS_LOG>
	    <CODE>0</CODE>
	    <DESCRIPTION>發送狀態查詢成功</DESCRIPTION>
	    <GET_DELIVERY_STATUS COUNT="1">
		    <SMS>
			    <NAME></NAME>
			    <MOBILE>+886921859698       </MOBILE>
			    <SENT_TIME>2015/09/23 15:37:05</SENT_TIME>
			    <COST>1.00</COST>
			    <STATUS>100</STATUS>
		    </SMS>
	    </GET_DELIVERY_STATUS>
    </SMS_LOG>
    */
    public class SMS_LOG
    {
        public int CODE { get; set; }

        public string DESCRIPTION { get; set; }

        public GET_DELIVERY_STATUS GET_DELIVERY_STATUS { get; set; }

        public override string ToString()
        {
            return string.Format("SMS_LOG(CODE：{0}，DESCRIPTION：{1}，GET_DELIVERY_STATUS：{2})",
                CODE,
                DESCRIPTION,
                GET_DELIVERY_STATUS.ToString());
        }
    }

    public class GET_DELIVERY_STATUS
    {
        [XmlAttribute]
        public int COUNT { get; set; }

        [XmlElement("SMS")]
        public List<SMS_CONTENT> SMS_LIST { get; set; }

        public override string ToString()
        {
            return string.Format("GET_DELIVERY_STATUS(COUNT：{0}，SMS_LIST：[{1}])",
                COUNT,
                string.Join("、", SMS_LIST.Select(p => p.ToString())));
        }
    }

    public class SMS_CONTENT
    {
        public string NAME { get; set; }
        public string MOBILE { get; set; }
        public string SENT_TIME { get; set; }
        public string COST { get; set; }
        public string STATUS { get; set; }

        public override string ToString()
        {
            return string.Format("SMS_CONTENT(NAME：{0}，MOBILE：{1}，SENT_TIME：{2}，COST：{3}，STATUS：{4})",
                NAME,
                MOBILE,
                SENT_TIME,
                COST,
                STATUS);
        }
    }

    public class Every8d_MessageReceiver
    {
        public string NAME { get; set; }
        public string MOBILE { get; set; }
        public string EMAIL { get; set; }
        public DateTime? SENDTIME { get; set; }

        public string CONTENT { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("<USER");

            sb.AppendFormat(" NAME=\"{0}\"", NAME);
            sb.AppendFormat(" MOBILE=\"{0}\"", MOBILE);
            sb.AppendFormat(" EMAIL=\"{0}\"", EMAIL);
            sb.AppendFormat(" SENDTIME=\"{0}\"", SENDTIME.HasValue ? SENDTIME.Value.ToString(SMSClient.SendTimeFormat) : string.Empty);
            
            sb.AppendFormat(">{0}", CONTENT);

            sb.AppendFormat("</USER>");

            return sb.ToString();
        }
    }
    #endregion

    #region 刪除簡訊預約的回傳值

    public class ERASE_BOOKING_RESULT
    {
        /// <summary>
        /// 刪除的筆數。
        /// </summary>
        public int DELETED { get; set; }

        /// <summary>
        /// 回補的點數
        /// </summary>
        public double CLAW_BACK { get; set; }
    }

    #endregion
}
