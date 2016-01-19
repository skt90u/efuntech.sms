HTTP API
=====

簡介
--------

為企業用戶提供透過 ZU Tech Msg API，進行簡訊發送、取得派送紀錄、使用者帳戶餘額等相關操作。

<!--
為企業用戶提供透過 [REST API](https://zh.wikipedia.org/wiki/REST) 的方式，進行簡訊發送、取得派送紀錄、使用者帳戶餘額等相關操作。
-->

Note: 若您尚無本平台之帳號，請至[註冊](http://www.zutech.info/)功能頁申請帳號

----------

API 基本網址
--------

**基本網址**:   
```
http://www.zutech.info/docs/
```

----------

請求標頭 (Request Headers)
------------

**Content-Type**: `application/json`.

**Accept header**: `application/json`.

**TimezoneOffset**: 本地時間與世界標準時間(UTC)之間的差距. 範例： `+08:00`

**Authorization**: 此平台所有API都是使用 basic authorization 的方式進行任何， 範例：`Basic dGVzdDoxMjM0` 

authorization header建構步驟:

1. 將帳號密碼結合成一個 `帳號:密碼` 型式的字串.
2. 將結合成的字串使用 **[RFC2045-MIME](https://webnet77.net/cgi-bin/helpers/base-64.pl)** 進行編碼.
3. 將編碼結果寫入 authorization header.

範例說明:   
**Username**: `test`
**Password**: `1234`  
**Base64 encoded string**: `dGVzdDoxMjM0`  
**Authorization header**: `Basic dGVzdDoxMjM0`  

- - - -

發送門號格式
----------------------

發送門號格式必須為國際標準手機格式(E.164)，範例：`+886921123456`

----------

取得帳戶餘額
-------

**定義**:   
```
http://www.zutech.info/docs/balance
```

**Request**  
```json
GET /docs/balance HTTP/1.1
Host: www.zutech.info
Authorization: Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==
Accept: application/json
TimezoneOffset: +08:00
```

**Response**  
```json
HTTP/1.1 200 OK
Content-Type: application/json

{
  "balance": 47.79
}
```

<!--
** 使用 Postman 範例**

![](images/GetAccountBalance.png)
-->

----------


發送簡訊
-------

**定義**:  
```
http://www.zutech.info/docs/sendsms
```

**參數**  

- **sendTime:** 預定發送時間，其格式必須為 [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601), 例如：`2015-10-16T10:34:27.8680576+08:00`
- **subject:** 簡訊類別描述，發送紀錄查詢時參考用，可不填
- **content:** 簡訊內容，其內容可以包含取代關鍵字 `@space1@`、 `@space2@`、 `@space3@`、 `@space4@`、 `@space5@`，這些關鍵字將被收訊者資訊定義的 `param1`、 `param2`、 `param3`、 `param4`、 `param5` 所取代
- **mobiles:** 收訊門號，其格式必須為國際標準手機格式(E.164)，範例：`+886921123456`

**Request**  
```json
POST /docs/sendsms HTTP/1.1
Host: www.zutech.info
Authorization: Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==
Content-Type: application/json
Accept: application/json
TimezoneOffset: +08:00

{
    "sendTime":"2015-10-16T10:34:27.8680576+08:00",
    "subject":"My first SMS message",
    "content":"Hello world",
    "mobiles":[
        "+886921123456",
        "+886921123457",
        "+886921123458",
    ]
}
```

**Response**   
```json
HTTP/1.1 200 OK
Content-Type: application/json

[
  {
    "balance": 997,
    "smsCount": 3,
    "price": 3,
    "bulkId": 12
  }
]
```

** Response 參數說明 **   

若呼叫此API成功, 則回應的狀態碼將為 200 OK.

| 參數     | 型別    | 說明         |
| -------- | ------- |--------------|
| balance  | double  | 使用者餘額.  |
| smsCount | integer | 發送簡訊筆數 |
| price    | double  | 花費點數     |
| bulkId   | integer | 簡訊識別碼   |

<!--
** 使用 Postman 範例**

![](images/SendSMS.png)
-->

----------


發送參數簡訊
-------

**定義**:  
```
http://www.zutech.info/docs/sendparamsms
```

**參數**  

- **sendTime:** 預定發送時間，其格式必須為 [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601), 例如：`2015-10-16T10:34:27.8680576+08:00`
- **subject:** 簡訊類別描述，發送紀錄查詢時參考用，可不填
- **content:** 簡訊內容，其內容可以包含取代關鍵字 `@space1@`、 `@space2@`、 `@space3@`、 `@space4@`、 `@space5@`，這些關鍵字將被收訊者資訊定義的 param1、 param2、 param3、 param4、 param5
- **messageReceivers:** 收訊者資訊
	- **name:** 收訊者姓名
	- **mobile:** 收訊者門號，其格式必須為國際標準手機格式(E.164)，範例：`+886921123456`
	- **email:** 收訊者電子信箱，若有指定此參數，在發送簡訊時，會一併將簡訊內容發送至指定的電子信箱
	- **sendTime:** 預定發送時間，其格式必須為 [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601), 例如：`2015-10-16T10:34:27.8680576+08:00`
	- **param1:** 簡訊內容參數一，此參數指定文字將會取代簡訊內容的關鍵字 `@space1@`
	- **param2:** 簡訊內容參數二，此參數指定文字將會取代簡訊內容的關鍵字 `@space2@`
	- **param3:** 簡訊內容參數三，此參數指定文字將會取代簡訊內容的關鍵字 `@space3@`
	- **param4:** 簡訊內容參數四，此參數指定文字將會取代簡訊內容的關鍵字 `@space4@`
	- **param5:** 簡訊內容參數五，此參數指定文字將會取代簡訊內容的關鍵字 `@space5@`


**Request**  
```json
POST /docs/sendparamsms HTTP/1.1
Host: www.zutech.info
Authorization: Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==
Content-Type: application/json
Accept: application/json
TimezoneOffset: +08:00

{  
   "sendTime":"2045-10-20T11:58:13.0385009+08:00",
   "subject":"My first SMS message with parameters",
   "content":"Hello @space1@",
   "messageReceivers":[  
      {  
         "name":"Andy",
         "mobile":"+886921123456",
         "email":"andy@gmail.com",
         "sendTime":"2045-10-16T11:58:13.0385009+08:00",
         "param1":"Andy",
         "param2":"",
         "param3":"",
         "param4":"",
         "param5":"",
      },
      {  
         "name":"John",
         "mobile":"+886921654321",
         "email":"john@gmail.com",
         "sendTime":null,
         "param1":"John",
         "param2":"",
         "param3":"",
         "param4":"",
         "param5":"",
      }
   ]
}
```

**Response**   
```json
HTTP/1.1 200 OK
Content-Type: application/json

[
  result: [{
    "balance": 997,
    "smsCount": 3,
    "price": 3,
    "bulkId": 12
  }],
  errors: []
]
```

** Response 參數說明 **   

若呼叫此API成功, 則回應的狀態碼將為 200 OK.

| 參數     | 型別    | 說明         |
| -------- | ------- |--------------|
| balance  | double  | 使用者餘額.  |
| smsCount | integer | 發送簡訊筆數 |
| price    | double  | 花費點數     |
| bulkId   | integer | 簡訊識別碼   |

<!--
** 使用 Postman 範例**

![](images/SendParamSMS.png)
-->

----------

取得派送結果
-------

**定義**:  
```
http://www.zutech.info/docs/report/{blukId}
```

**參數**  

- **bulkId:** 簡訊識別碼，用來指定要取得哪一筆預約簡訊規則對應的派送結果

**Request**  
```json
GET /docs/report/12 HTTP/1.1
Host: www.zutech.info
Authorization: Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==
Accept: application/json
TimezoneOffset: +08:00
```

**Response** 
```json
HTTP/1.1 200 OK
Content-Type: application/json

[
  {
    "sendTime": "2015-10-16 02:34:00",
    "sendTitle": "My first SMS message",
    "sendBody": "Hello world",
    "messageStatus": "MessageAccepted",
    "destinationAddress": "+886921123456",
    "createdTime": "2015-10-17 07:20:13",
    "sentDate": "2015-10-17 07:20:16",
    "doneDate": "2015-10-17 07:24:22",
    "deliveryStatus": "DeliveredToTerminal",
    "price": 1,
    "delivered": true,
    "name": ""
  },
  {
    "sendTime": "2015-10-16 02:34:00",
    "sendTitle": "My first SMS message",
    "sendBody": "Hello world",
    "messageStatus": "MessageAccepted",
    "destinationAddress": "+886921123457",
    "createdTime": "2015-10-17 07:20:13",
    "sentDate": "2015-10-17 07:20:16",
    "doneDate": "2015-10-17 07:24:22",
    "deliveryStatus": "DeliveredToTerminal",
    "price": 1,
    "delivered": true,
    "name": ""
  },
  {
    "sendTime": "2015-10-16 02:34:00",
    "sendTitle": "My first SMS message",
    "sendBody": "Hello world",
    "messageStatus": "MessageAccepted",
    "destinationAddress": "+886921123458",
    "createdTime": "2015-10-17 07:20:13",
    "sentDate": "2015-10-17 07:20:16",
    "doneDate": "2015-10-17 07:24:22",
    "deliveryStatus": "DeliveredToTerminal",
    "price": 1,
    "delivered": true,
    "name": ""
  }
]
```

** Response 參數說明 **   

若呼叫此API成功, 則回應的狀態碼將為 200 OK. 

| 參數               | 型別    | 說明               |
| ------------------ | ------- |------------------- |
| sendTime           | string  | 預定發送時間       |
| sendTitle          | string  | 發送主旨           |
| sendBody           | string  | 發送內容           |
| messageStatus      | string  | 電信商回傳訂閱狀態 |
| destinationAddress | string  | 收訊人門號         |
| createdTime        | string  | 建立預約簡訊時間   |
| sendDate           | string  | 實際發送時間       |
| doneDate           | string  | 手機接收時間       |
| deliveryStatus     | string  | 派送結果           |
| price              | double  | 簡訊發送費用       |
| delivered          | boolean | 是否派送成功       |
| name               | string  | 收訊人姓名         |

<!--
** 使用 Postman 範例**

![](images/Report.png)
-->

----------

取消預約簡訊 
-------

**定義**:  
```
http://www.zutech.info/docs/erasebooking/{blukId}
```

**Parameters**  

- **bulkId:** 簡訊識別碼，用來指定要刪除的預約簡訊規則

**Request**  
```json
DELETE /docs/report/10 HTTP/1.1
Host: www.zutech.info
Authorization: Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==
Accept: application/json
TimezoneOffset: +08:00
```

**Response** 
```json
HTTP/1.1 200 OK
Content-Type: application/json

{
	"smsCount": "3",
	"price": "3",
}
```

** Response 參數說明 **   

若呼叫此API成功, 則回應的狀態碼將為 200 OK.

| 參數      | 型別   | 說明            |
| -------- | ------ | --------------- |
| smsCount | double | 取消簡訊的筆數   |
| price    | double | 回補點數        |

<!--
** 使用 Postman 範例**

![](images/EraseBooking.png)
-->


