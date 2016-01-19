HTTP API
=====

Introduction
--------

This page will help you get started with ZU Tech Msg API. You'll be up and running in a jiffy!

Note: If you don't have an account yet, please visit [our Sign-up page](http://www.zutech.info/) and create your free account.

----------

API Base URL
--------

**Base URL**:   
```
http://www.zutech.info/docs/
```

----------

Request Headers
------------

**Content-Type**: `application/json`.

**Accept header**: `application/json`.

**TimezoneOffset**: timezone offset is a timezone difference between UTC and Local Time. e.g. `+08:00`

**Authorization**: We support basic authorization using a username and password with Base64 encoding variation **[RFC2045-MIME](https://webnet77.net/cgi-bin/helpers/base-64.pl)**.

The authorization header is constructed as follows:

1. Username and password are combined into a string `username:password`.
2. The resulting string is encoded using the **[RFC2045-MIME](https://webnet77.net/cgi-bin/helpers/base-64.pl)** variant of Base64.
3. Example:   
	**Username**: `test`
	**Password**: `1234`  
	**Base64 encoded string**: `dGVzdDoxMjM0`  
	**Authorization header**: `Basic dGVzdDoxMjM0`  

- - - -

Phone numbers format
----------------------

We strongly recommend using the E.164 number formatting. E.164 numbers are internationally standardized to a fifteen digit maximum length. Phone numbers are usually prefixed with + (plus sign), followed by a country code, network code and the subscriber number. Phone numbers that are not E.164 formatted may work, depending on the handset or network, e.g. `+886921123456`

----------

Get account balance
-------

**Definition**:   
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


Send SMS
-------

**Definition**:  
```
http://www.zutech.info/docs/sendsms
```

**Parameters**  

- **sendTime:** It's a datetime string with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) format, e.g. `2015-10-16T10:34:27.8680576+08:00`
- **subject:** It will not be sent into SMS, input `subject` can help you memory why you send this SMS message 
- **content:** The parameter will send as SMS text 
- **mobiles:** Specify SMS recipients, which are using the E.164 number formatting, e.g. `+886921123456`

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

**Response Format**   

If everything went well, it should provide an 200 OK response code

| Parameter  | Type    | Description                   |
| ---------  | ------- |------------------------------ |
| balance    | double  | Account balance.              |
| smsCount   | integer | Represents number of messages |
| price      | double  | Represents cost of messages   |
| bulkId     | integer | The ID that uniquely identifies the request. Bulk ID will be received only when you send a message           |

<!--
** 使用 Postman 範例**

![](images/SendSMS.png)
-->

----------


Send SMS with parameters
-------

**Definition**:  
```
http://www.zutech.info/docs/sendparamsms
```

**Parameters**  

- **sendTime:** It's a datetime string with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) format, e.g. `2015-10-16T10:34:27.8680576+08:00`
- **subject:** It will not be sent into SMS, input `subject` can help you memory why you send this SMS message 
- **content:** The parameter will send as SMS text, it can contain text replacements `%filed1%`, `%filed2%`, `%filed3%`, `%filed4%`, `%filed5%`, they will be replaced by messageReceiver's parameters `param1`, `param2`, `param3`, `param4`, `param5`.
- **messageReceivers:** Specify SMS recipients, which contains following parameters
	- **name:** Recipient's name 
	- **mobile:** Recipient's mobile number, which is using the E.164 number formatting, e.g. `+886921123456`
	- **email:** Recipient's email
	- **sendTime:** It's a datetime string with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) format, e.g. `2015-10-16T10:34:27.8680576+08:00`
	- **param1:** Replace `@space1@` in SMS content to this string
	- **param2:** Replace `@space2@` in SMS content to this string
	- **param3:** Replace `@space3@` in SMS content to this string
	- **param4:** Replace `@space4@` in SMS content to this string
	- **param5:** Replace `@space5@` in SMS content to this string


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

**Response Format**    

If everything went well, it should provide an 200 OK response code

| Parameter  | Type    | Description                   |
| ---------  | ------- |------------------------------ |
| balance    | double  | Account balance.              |
| smsCount   | integer | Represents number of messages |
| price      | double  | Represents cost of messages   |
| bulkId     | integer | The ID that uniquely identifies the request. Bulk ID will be received only when you send a message           |

<!--
** 使用 Postman 範例**

![](images/SendParamSMS.png)
-->

----------

Get delivery report 
-------

**Definition**:  
```
http://www.zutech.info/docs/report/{blukId}
```

**Parameters**  

- **bulkId:** The ID that uniquely identifies the request. Bulk ID will be received only when you send a message

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

**Response Format**  

If everything went well, it should provide an 200 OK response code 

| Parameter          | Type    | Description                   |
| -----------------  | ------- |------------------------------ |
| sendTime           | string  | Send message time expectly    |
| sendTitle          | string  | Send title |
| sendBody           | string  | SMS content you sent |
| messageStatus      | string  | Message status of server   |
| destinationAddress | string  | Recipient's mobile number   |
| createdTime        | string  | Booking time when you use a send message service |
| sendDate           | string  | Send message time actually |
| doneDate           | string  | Recipient's receive time   |
| deliveryStatus     | string  | Status of delivery   |
| price              | double  | Message cost   |
| delivered          | boolean | Determine message delivered or not |
| name               | string  | Recipient's name    |


<!--
** 使用 Postman 範例**

![](images/Report.png)
-->

----------

Erase booking SMS message 
-------

**Definition**:  
```
http://www.zutech.info/docs/erasebooking/{blukId}
```

**Parameters**  

- **bulkId:** The ID that uniquely identifies the request. Bulk ID will be received only when you send a message

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

**Response Format**   

If everything went well, it should provide an 200 OK response code

| Parameter          | Type    | Description                   |
| -----------------  | ------- |------------------------------ |
| smsCount           | double  | Number of message be canceled |
| price              | double  | Price of drawback  |

<!--
** 使用 Postman 範例**

![](images/EraseBooking.png)
-->


