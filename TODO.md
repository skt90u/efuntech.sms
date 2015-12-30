Mapper.Map(model, entity);

modalForm 指定欄位不存在 - NewPasswordConfirmed

### 20151229
SectorStatistics Bug



### 20151225
簡訊發送
參數簡訊發送
發送查詢
(OK)聯絡人管理
(OK)系統設定
預約/週期簡訊維護
子帳號管理
子帳號點數管理
報表管理
(OK)其他功能

### 20151120 todo
https://github.com/Textalk/angular-schema-form-datepicker

### 發送與接收都需要更新History
public void UpdateSendMessageHistory(int sendMessageQueueId)
public void CreateSendMessageHistory(int sendMessageQueueId)


entity.SendMessageResultCreatedTime = SendMessageResultCreatedTime;

### 20151109
陽明山

0922338896
0918519051

"   at EFunTech.Sms.Portal.InfobipSmsProvider.CreateSendMessageHistory(Int32 SendMessageQueueId) in c:\Project\efuntech.sms\EFunTech.Sms.Portal\Services\MessageServices\SmsProviders\InfobipSmsProvider.cs:line 382
   at EFunTech.Sms.Portal.InfobipSmsProvider.UpdateDb(String requestId, DeliveryReportList deliveryReportList) in c:\Project\efuntech.sms\EFunTech.Sms.Portal\Services\MessageServices\SmsProviders\InfobipSmsProvider.cs:line 292
   at EFunTech.Sms.Portal.InfobipSmsProvider.GetDeliveryReport(String requestId) in c:\Project\efuntech.sms\EFunTech.Sms.Portal\Services\MessageServices\SmsProviders\InfobipSmsProvider.cs:line 252
   at EFunTech.Sms.Portal.CommonSmsService.GetDeliveryReport(String requestId) in c:\Project\efuntech.sms\EFunTech.Sms.Portal\Services\MessageServices\CommonSmsService.cs:line 284"

GetDeliveryReport不可全部包在TransScope裡面

Client 驗證電話號碼

發送時間小於現在的最大一筆，並且判斷這筆資料是否已經發送了，如果沒有發送，就發送這則簡訊規則

為了避免 SmsApi 還要傳送多餘 TimeZoneOffset, 因此將想辦法去除此參數

<td height="80" background="/Content/images/menu.jpg">

### BlockUI
Loading ...

https://msdn.microsoft.com/en-us/library/system.datetime.specifykind.aspx?cs-save-lang=1&cs-lang=cpp#code-snippet-1
https://msdn.microsoft.com/en-us/library/shx7s921.aspx

http://www.aaroncoleman.net/post/2011/06/16/Forcing-Entity-Framework-to-mark-DateTime-fields-at-UTC.aspx

http://stackoverflow.com/questions/4648540/entity-framework-datetime-and-utc

避免重複的ContactsId


DeliveryStatusChineseString // TODO: DeliveryStatus 中文說明
 



HangFire retry survey, is it necessary to use retry attribute

API - For External SendSMS




2015/09/26
11:14:48

                {
                    name: 'SendTime',
                    displayName: '發送時間',
                    cellTemplate: '<div class="ui-grid-cell-contents">{{ row.entity.SendTime | LocalTimeFilter: "YYYY/MM/DD HH:mm:ss" }}</div>',
                },
				
Thread.CurrentThread.CurrentCulture = new CultureInfo(Request.UserLanguages[1]);				

                entity.SentDate = Converter.ToDateTime(DeliveryReport.SENT_TIME, Converter.Every8d_SentTime).Value; // 2010/03/23 12:05:29
                entity.DoneDate = Converter.ToDateTime(DeliveryReport.SENT_TIME, Converter.Every8d_SentTime).Value; // 2010/03/23 12:05:29，簡訊供應商沒有提供此資訊，因此設定與SentDate一致


http://stackoverflow.com/questions/19241815/how-to-change-the-default-timezone-of-my-asp-net-website-in-web-config-file


columnEditor.js -> columnSelector.js

### 20151001
1. 刪除子帳號，確定這個子帳號沒有建立這個子帳號下面沒有任何子帳號
2. 低於下限撥點於每天凌晨執行
3. 開立 帳號提供了管理者針對公司或部門進行會員-
 角色之「管理層級」若為企業，可開立公司內所有人員帳號 角色之「管理層級」若為企業，可開立公司內所有人員帳號
 角色之「管理層級」若為部門，可開立該所有人員帳號 角色之「管理層級」若為部門，可開立該所有人員帳號
4. (DONE)部門-新增刪除修改
5. (DONE)發送前過濾黑名單
6. (DONE)datepicker 要使用中文顯示 (GOOGLE 'angular bootstrap datepicker 中文')

### PlaceHolder
1. http://stackoverflow.com/questions/7312623/insert-line-break-inside-placeholder-attribute-of-a-textarea
2. https://gist.github.com/antsa/2170024
3. https://github.com/michaelsacca/Compass-Placeholder-Text-Mixin
4. http://stackoverflow.com/questions/17181849/placeholder-mixin-scss-css
5. http://compass-style.org/examples/compass/css3/input-placeholder/

### Grid CSS
https://github.com/angular-ui/ui-grid/issues/3369

### 20150924
立即發送

SendMessageStatistic 統計表
SendMessageHistory 發送歷史紀錄

(2) BackgroundService.GetDeliveryReports()
在建立SendMessageRule的時候，
在產生完成MessageReciever後，
把有指定時間的找出來(同一個SendTime設為同一個SendMessageRule)，設定成為預約發送，
其他的才是這一次，這一次發送的內容的內容，如果沒有任何內容，就將這筆SendMessageRule刪除

(2.PS)
移除SendMessageQueue欄位，未來統計數量不會寫在這個資料表

(3) BackgroundService.CreateSendMessageStatistic(SendMessageQueueId)
產生大表(SendMessageQueueId)
CreatedUser + SendMessageQueue + SendMessageResultItem + DeliveryReport + Every8d欄位

(4) BackgroundService.CheckRetryMobiles(SendMessageQueueId)
大表產生完成，檢查是否有需要重新發送的簡訊，如果有，則根據Administrator設定的RetryTimeInterval，設定重送時間

(5) 每分鐘檢查 SendMessageStatistic 有設定重送時間，且已到達重送時間，但是尚未重送的資料進行重送

(6) 重新傳送，並更新資料

### 20150923
0. [basic auth](https://github.com/jamiekurtz/BasicAuthForWebAPI)
0. 不要cookies儲存帳號密碼

1. 手機發送不成功的問題
	* [OneAPI 支援格式](http://www.gsma.com/oneapi/faq-messaging-api-questions/)
	* [International phone number parsing, validation and formatting for Smart Office](https://thibaudatwork.wordpress.com/2014/04/24/international-phone-number-parsing-validation-and-formatting-for-smart-office/)
	* [USING GOOGLE’S LIBPHONENUMBER IN MICROSOFT.NET WITH C#](https://bkiener.wordpress.com/2011/06/06/using-googles-libphonenumber-in-microsoft-net-with-c/)
	* [Why does the libphonenumber website return a different result from the libphonenumber library](http://stackoverflow.com/questions/24148732/why-does-the-libphonenumber-website-return-a-different-result-from-the-libphonen)
	* [Parsing Phone Numbers to their parts](http://stackoverflow.com/questions/12540319/parsing-phone-numbers-to-their-parts)
	* [erezak/libphonenumber-csharp](https://github.com/erezak/libphonenumber-csharp)
	* [C# 使用範例](https://libphonenumber.codeplex.com/)
	* [javascript](https://www.npmjs.com/package/google-libphonenumber)
	* [C# source code](https://libphonenumber.codeplex.com/)
	* [javascript demo](https://libphonenumber.appspot.com/)
	* [js demo2](https://rawgit.com/googlei18n/libphonenumber/master/javascript/i18n/phonenumbers/demo-compiled.html)
	* [phone format](http://blogs.chenan.tw/blog/2015/02/05/libphonenumberfor-ci的phonenumberformat種類/#more-1126)
	* [nuget](https://www.nuget.org/packages/libphonenumber-csharp/)
	* [How To Dial Internationally](https://countrycode.org/how-to-call?fromCountry=us&toCountry=tw&phoneNumber=011+886+)
	* [GlobalPhone](https://github.com/GlobalPhone/GlobalPhone)
	* 所有手機門號在進入資料庫之前都轉換成886的格式 [how-to-convert-mobile-number-to-international-format-in-c-sharp](http://stackoverflow.com/questions/30434836/how-to-convert-mobile-number-to-international-format-in-c-sharp)

2. Infobip_DeliveryReport.MessageId 與 Infobip_SendMessageResultItem.MessageId 之間 ForeignKey 的問題
   在 Hangfire 每分鐘取得 DeliveryReport 資料，並存入 Infobip_DeliveryReport，有可能發生在長榮發送簡訊，但是在家裡的電腦先收到DeliveryReport，會發生 Infobip_DeliveryReport.MessageId 在 Infobip_SendMessageResultItem 沒有對應的 MessageId 
的問題，解決方式：NotifyUrl, GetDeliveryReportByRequestId -> 每次發送簡訊得到的requestId, 都存放到DeliveryReportRequestQueue, 如果撈到資料，就在Queue刪除對應資料
	
3. HangFire Dashboard
	* 修改存取路徑 (URL)
	* 修改存取權限 (AllowAllAuthorizationFilter)

4. 簡訊發送最後送出號碼轉為 Infobip 支援格式


5. 資料庫待移除欄位(Contact)
	* HomePhoneArea
	* CompanyPhoneArea	
	* CompanyPhoneExt

6. 調整網站效能
	* 在 AutoMapper Profile 中減少數量，看看是否能夠加快查詢速度
	* 調整 EntityFramework 效能
	* 判斷網站是否有MemoryLeak (DbContext)
	* 使用GZIP压缩
		- [ASP.NET Web API中使用GZIP 或 Deflate压缩](http://www.cnblogs.com/yxlblogs/p/4058888.html)
		- [WEB API USING GZIP COMPRESSION](https://damienbod.wordpress.com/2014/07/16/web-api-using-gzip-compression/)
		- http://benfoster.io/blog/aspnet-web-api-compression
		- https://github.com/azzlack/Microsoft.AspNet.WebApi.MessageHandlers.Compression
		- https://www.nuget.org/packages/Microsoft.AspNet.WebApi.MessageHandlers.Compression/

7. 發送任務，處理上傳資料有帶發送時間的資料，詢問Dino如何處理，當一個使用者選擇週期發送，但是上傳檔案又有指定時間的問題

8. 動態顯示隱藏Grid
	- CommonContactManager.selectColumns
	- SharedContactManager.editColumn
	- 參考資料
		* [113_adding_and_removing_columns](http://ui-grid.info/docs/#/tutorial/113_adding_and_removing_columns)  
```
$scope.remove = function() {
    $scope.columns.splice($scope.columns.length-1, 1);
  }
  
  $scope.add = function() {
    $scope.columns.push({ field: 'company', enableSorting: false });
  }
 
  $scope.splice = function() {
    $scope.columns.splice(1, 0, { field: 'company', enableSorting: false });
  }
 
  $scope.unsplice = function() {
    $scope.columns.splice(1, 1);
  }
  
  $scope.toggleDisplayName = function() {
    if( $scope.columns[1].displayName === 'GENDER' ){
      $scope.columns[1].displayName = 'Gender';
    } else {
      $scope.columns[1].displayName = 'GENDER';
    }
    $scope.gridApi.core.notifyDataChange( uiGridConstants.dataChange.COLUMN );
  }
  
  $scope.toggleVisible = function() {
    $scope.columns[0].visible = !($scope.columns[0].visible || $scope.columns[0].visible === undefined);
    $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.COLUMN);
  }
```

9. [跨頁保存選取項目](http://ui-grid.info/docs/#/tutorial/208_save_state)

10. 網域資訊如下 : Zutech.info，屆時如需設定DNS，請再提供相關資料給我設定。

     
### 後台功能需求
1.     管理者帳號可以設立子管理帳號，並做權限控管
2.     客戶帳號新增修改刪除查詢功能
3.     可設定發送失敗後Retry 的時間(0-24Hrs)與次數
4.     客戶主、子帳號明細查詢
5.     點數之撥發與刪除


### 20150901
### BlockUI
1. https://github.com/McNull/angular-block-ui
2. http://angular-block-ui.nullest.com/#!/
3. http://angular-block-ui.nullest.com/#!/examples/documentation
4. http://angular-block-ui.nullest.com/#!/examples/manual-blocking-examples
5. http://ngmodules.org/modules/angular-block-ui
6. http://embed.plnkr.co/NqyunD/preview
7. http://www.fredmastro.com/?p=155
### Modal
1. http://mgcrea.github.io/angular-motion/
### Angular Confirm
1. http://schlogen.github.io/angular-confirm/
2. http://codepen.io/m-e-conroy/pen/ALsdF
3. http://ngmodules.org/modules/angular-dialog-service

### Code Gen Sample
http://comdan66.github.io/OA-ElasticaSearch/guide/create.html

### Issues
1. https://github.com/angular-ui/bootstrap/issues/3429
2. 找不到
 http://localhost:20155/Scripts/bower_components/bootstrap/dist/fonts/glyphicons-halflings-regular.woff2
glyphicons-halflings-regular.woff2 Failed to load resource: the server responded with a status of 404 (Not Found)
3. 測試修改
5. [ui-grid loading example](https://cwiki.apache.org/confluence/dashboard.action) 

### TODO(20150814)
0. Generate Controller & SchemaForm File (DONE)
1. treeview
2. async http://stackoverflow.com/questions/9343594/how-to-call-asynchronous-method-from-synchronous-method-in-c
1. 資料庫中文問題
2. 過濾文字去除空格
6. RecurringSMS.aspx
7. DepartmentManager.aspx
8. DepartmentPointManager.aspx
9. SMS_Setting.aspx
10. SearchMemberSend.aspx
11. [使用metrial's tabs](https://material.angularjs.org/latest/#/demo/material.components.tabs)
12. [AccountController DI Error](http://stackoverflow.com/questions/26947371/dependency-injecting-signinmanager-does-not-work-with-unity-works-when-using-o)

	<script type="text/javascript">


		$(function() {
			$("a.tooltip").tooltip({
				track: true,
				delay: 0,
				showURL: false,
				showBody: " - ",
				fade: 250
			});
			
			var NameListUpload_Upload = new AjaxUpload('#NameListUpload_UploadInput', {
				action: 'BlackListAjaxHandler.ashx', // I disabled uploads in this example for security reaaons
				name: 'userfile',
				data: {
					action: 'importBlackList'
				},
				onSubmit: function(file, ext) {
					if (!(ext && /^(xls|xlsx|txt|zip)$/i.test(ext))) {
						alert('上傳檔案格式不正確，建議使用範例檔案！');
						return false;
					}
					//$("#NameListUpload_UploadResultDiv").slideUp("slow");
					this.disable();
					$("#NameListUpload_UploadInput").removeClass("NameListUpload_UploadInputClass");
					$("#NameListUpload_UploadInput").addClass("NameListUpload_Uploading");
				},
				onComplete: function(file, response) {
					//file:上傳檔案名稱
					//response:回傳資訊
					this.enable();
					$("#NameListUpload_UploadInput").removeClass("NameListUpload_Uploading");
					$("#NameListUpload_UploadInput").addClass("NameListUpload_UploadInputClass");
					if (response != '') {
						var splitIndex = response.search(",");
						var reCode = response.substring(0, splitIndex)
						var reMsg = response.substring(splitIndex + 1, response.length);
						if (reCode != '0000') {
							alert('' + response);
						} else {
							setParentInfoReload();
							closeWindow();
						}
					} else {
						alert('上傳資料過程發生錯誤，請重新上傳');
					}
				}
			});
		});

### SURVEY
1. Grid in Modal
2. Model Flip 效果


### Model 致中
http://jsfiddle.net/vCL9P/2/

[bower]
%SystemRoot%\system32;%SystemRoot%;%SystemRoot%\System32\Wbem;C:\Program Files (x86)\Windows Kits\8.1\Windows Performance Toolkit\;C:\Program Files\Microsoft SQL Server\110\Tools\Binn\;C:\Program Files (x86)\Microsoft SDKs\TypeScript\1.0\;C:\Program Files\Microsoft SQL Server\120\Tools\Binn\;C:\Program Files\Microsoft\Web Platform Installer\;C:\Program Files (x86)\Microsoft SDKs\Azure\CLI\wbin;%systemroot%\System32\WindowsPowerShell\v1.0\;C:\Program Files (x86)\Git\bin;C:\Program Files\nodejs\

WebApi
Auth

https://gilesey.wordpress.com/2013/04/21/allowing-remote-access-to-your-iis-express-service/

context.SharedGroupContacts.AddRange(SharedGroupContacts);

[偵錯MVC]
http://blog.miniasp.com/post/2013/03/16/ASPNET-MVC-4-Source-Code-with-YOUR-projects.aspx

[ASP.NET SOURCE CDOE]
https://github.com/aspnet?page=2

[VS DEBUG]
 private void Log(string methodName, RouteData routeData)
          {
               var controllerName = routeData.Values["controller"];
               var actionName = routeData.Values["action"];
               var message = String.Format("{0} controller:{1} action:{2}", methodName, controllerName, actionName);
               Debug.WriteLine(message, "Action Filter Log");
          }


[ASP.NET MVC Action Filters]
https://mvcactionfilter.codeplex.com/

實作購物網站
http://maidot.blogspot.tw/2014/10/aspnet-mvc-session.html

ActionFilter範例(mvcactionfilter-70298)
https://mvcactionfilter.codeplex.com/SourceControl/latest#Mvc/NerdDinner 1.0/NerdDinner/Controllers/AccountController.cs

http://encosia.com/asp-net-web-api-vs-asp-net-mvc-apis/

[如何處理沒有認證的頁面]
http://stackoverflow.com/questions/977071/redirecting-unauthorized-controller-in-asp-net-mvc

[Action Result]
https://msdn.microsoft.com/en-us/library/dd410269(v=vs.100).aspx

[AuthorizationPrivilegeFilter]
http://blog.falafel.com/custom-filter-asp-net-mvc-5/

[404 NOT FOUND處理方式參考範例]
http://www.codeproject.com/Articles/422572/Exception-Handling-in-ASP-NET-MVC

[HandleError WebConfig Setting]
https://msdn.microsoft.com/en-us/library/system.web.mvc.handleerrorattribute(v=vs.118).aspx

ViewBag

ValidateAntiForgeryToken

[filter]
https://msdn.microsoft.com/en-us/library/gg416513(VS.98).aspx
http://arthurmvc.blogspot.tw/2012/11/aspnet-mvcfilter.html

@Html.ValidationSummary("", new { @class = "text-danger" })


examples
http://www.asp.net/aspnet/samples/aspnet-mvc#mvc5

            // 設定 OAuth 基礎流程的應用程式
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
            };

            // 讓應用程式使用 Bearer 權杖驗證使用者
            app.UseOAuthBearerTokens(OAuthOptions);


filters.Add(new RequireHttpsAttribute());

https://portal.azure.com/#


[效能調整]
使用BundleTable壓縮Script
@section Scripts {
    @Scripts.Render("~/bundles/app")
}

BundleTable.EnableOptimizations = true;

http://www.asp.net/mvc/overview/security/create-an-aspnet-mvc-5-web-app-with-email-confirmation-and-password-reset

RegisterGlobalFilters
還不會用

RouteCollection
還不會用


[Enable SSL]
https://azure.microsoft.com/en-us/documentation/articles/web-sites-dotnet-deploy-aspnet-mvc-app-membership-oauth-sql-database/#bkmk_createmvc4app

[cross domain api]
http://devproconnections.com/aspnet/aspnet-web-api-tip-cors-cross-domain-requests
http://blogs.msdn.com/b/msdntaiwan/archive/2013/09/10/asp-net-web-api-2.aspx
http://www.asp.net/web-api/overview/security/enabling-cross-origin-requests-in-web-api


https://manage.windowsazure.com/@jelly11223344hotmail.onmicrosoft.com#Workspaces/WebsiteExtension/websites

{5.2.2.0}
https://aspnetwebstack.codeplex.com/SourceControl/list/changesets

[Add WebAPI in ASP.NET MVC 5]
http://blog.miniasp.com/post/2015/02/19/How-to-Add-Web-API-to-ASPNET-MVC.aspx

[Email Skill]
http://blog.miniasp.com/post/2015/04/10/Office-Outlook-Email-writing-tips.aspx

[Kudu]
http://blog.miniasp.com/post/2015/05/05/Intro-Azure-Web-App-Kudu-engine.aspx
http://efuntechsmsportal.scm.azurewebsites.net/
http://efuntechsmsportal.scm.azurewebsites.net/basicauth

Azure時間調整
http://blog.miniasp.com/post/2013/04/10/How-to-convert-Times-between-Time-Zones-in-Windows-Azure-Cloud-Platform.aspx

所有Service使用 IDispose

### IDisposable & GC.SuppressFinalize & Finalize
	* // TODO: YourToDoItem
	* 快速鍵: [Ctrl+\] + [CtrlT]
	* 工作清單選擇註解

### IDisposable & GC.SuppressFinalize & Finalize
- [正確呼叫 GC.SuppressFinalize](https://msdn.microsoft.com/zh-tw/library/ms182269.aspx)
	* 如果方法是 Dispose 的實作，請將呼叫加入至 GC.SuppressFinalize。
	* 如果方法不是 Dispose 的實作，請移除對 GC.SuppressFinalize 的呼叫，或將其移至型別的 Dispose 實作。
	* 變更 GC.SuppressFinalize 的所有呼叫以傳遞 this (Visual Basic 中的 Me)。
- [實現標準Dispose模式](http://www.dotblogs.com.tw/larrynung/archive/2011/03/10/21774.aspx)
- [When to Use IDisposable: Three easy rules](http://www.codeproject.com/Articles/319826/IDisposable-Finalizer-and-SuppressFinalize-in-Csha)
	* Don’t do it (unless you need to).
	* For a class owning managed resources, implement IDisposable (but not a finalizer)
	* For a class owning at least one unmanaged resource, implement both IDisposable and a finalizer
- [Dispose & Finalize 方法的用法方針](https://msdn.microsoft.com/zh-tw/library/vstudio/b1yfkh5e(v=vs.100).aspx)

### IOC
- [Enterprise Library DLL](http://entlib.codeplex.com/)
- [Enterprise Library SourceCode][https://unity.codeplex.com/SourceControl/latest]
- IUnityContainer.RegisterType
- IUnityContainer.RegisterInstance
- InjectionConstructor (InjectionMember)
- LifetimeManager
	* [Understanding Lifetime Managers](https://msdn.microsoft.com/en-us/library/ff660872(v=pandp.20).aspx)
	* [各種LifetimeManager說明](http://stackoverflow.com/questions/5129789/unity-2-0-and-handling-idisposable-types-especially-with-perthreadlifetimemanag)
	* Default: ContainerControlledLifetimeManager
	* 表列各種LifetimeManager  
	    參考資料: C:\Projects\efuntech.sms\reference\source\unity\source
		1. ExternallyControlledLifetimeManager
		2. PerResolveLifetimeManager
		3. PerThreadLifetimeManager
		4. SynchronizedLifetimeManager
		5. TransientLifetimeManager
		6. ContainerLifetimeManager
		7. PerRequestLifetimeManager(ASP.NET MVC靠他，Console專案請使用USING)    
- ServiceLocator.Current.GetInstance<ILogService>()
- ServiceLocator.Current.GetService(typeof(ILogService))






### 程式碼
- git clone https://skt90u@bitbucket.org/skt90u/efuntech.sms.git

### CQRS

IService
 
### 列出工作項目
- MVC登入機制(TOKEN)
- 跨網域存取API
- MVC帳號管理機制
- MVC IOC使用方式
- MVC DBCONTEXT記憶體釋放問題
- every8d每個頁面所需要實作的函式
- EXCEL讀取，寫入
- 自動測試

### LocalDb
(localdb)\v11.0
connectionString="Server=(localdb)\\v11.0;Integrated Security=true;"

    <add name="LocalDbConnection" connectionString="Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\SmsLocalDb.mdf;Integrated Security=True"
      providerName="System.Data.SqlClient" />

http://www.asp.net/mvc/overview/getting-started/introduction/creating-a-connection-string





[Create an ASP.NET MVC app with auth and SQL DB and deploy to Azure App Service](https://azure.microsoft.com/en-us/documentation/articles/web-sites-dotnet-deploy-aspnet-mvc-app-membership-oauth-sql-database/)

https://github.com/floydsoft/angular-underscore