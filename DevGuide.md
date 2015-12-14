### Client 驗證電話號碼
http://www.phoneformat.com/

### Sublime
1. [Emmet cheat-sheet](http://docs.emmet.io/cheat-sheet/)
2. [Sublime Useful Hotkey](https://gist.github.com/eteanga/1736542)
https://www.nuget.org/packages/SimpleBasicAuthenticationModule/


### 遠端發布
http://www.asp.net/web-forms/overview/deployment/configuring-server-environments-for-web-deployment/configuring-a-web-server-for-web-deploy-publishing-(remote-agent)
http://stackoverflow.com/questions/5867392/can-the-web-deploy-agent-run-on-a-port-other-than-80-on-iis6

If you want to change the security stamp after adding to a role use this:

UserManager.UpdateSecurityStampAsync(User.Id)


http://yslow.org/

Administrator

Administrator / Administratorpass 

".AspNet.ApplicationCookie"  FormsAuthentication.FormsCookieName
"__RequestVerificationToken"


elmad enable remote

write a log viewer

http://programmers.stackexchange.com/questions/185184/one-login-amongst-multiple-asp-net-mvc-applications-questions

change administrator to admini
change password to 1234
default to 100
static files caches

### Caching
1. [初探 HTTP 1.1 Cache 機制](http://blog.toright.com/posts/3414/%E5%88%9D%E6%8E%A2-http-1-1-cache-%E6%A9%9F%E5%88%B6.html)
2. [asp.net 客戶端瀏覽器緩存的Http頭介紹](http://www.loliman3000.com/tech/2fe1515db1b0c49283085984.php)
3. [CacheCow](http://www.hanselman.com/blogNuGetPackageOfTheWeekASPNETWebAPICachingWithCacheCowAndCacheOutput.aspx)
4. [HTTP 快取](https://developers.google.com/web/fundamentals/performance/optimizing-content-efficiency/http-caching)

https://developer.yahoo.com/performance/rules.html

http://compass-style.org/help/tutorials/spriting/
http://alistapart.com/article/sprites
https://developer.yahoo.com/performance/rules.html#favicon=

寶哥說的快取
http://blog.miniasp.com/post/2009/04/12/How-to-setup-Client-side-Cache-in-IIS.aspx
http://blog.miniasp.com/post/2008/02/03/Avoid-browser-cache-problem-on-css-or-javascript-file.aspx

### Chrome Reload 差別
http://stackoverflow.com/questions/14969315/whats-the-difference-between-normal-reload-hard-reload-and-empty-cache-a

附帶一提, 檢測用的相關工具:
Firefox 的 YSlow 會查那些靜態檔案沒設 expire time。
Firefox 的 Live HTTP Headers 會秀出目前連線的所有 http connection 的 request 和 response headers。可以用來查看 Expire 是否有被設對, 還有 client 是否真的沒發出 request。
Chrome 的 Speed Tracer 可以秀出頁面載入所有檔案的開始時間, 包含 http request、response 開始和結束的時間。也有 header 內的訊息。多種願望, 一次滿足!!


https://developer.yahoo.com/performance/rules.html

### 壓縮
http://dotnetmentors.com/aspnet/how-to-enable-http-compression-in-asp-net-website.aspx

### Cache
http://dejanstojanovic.net/aspnet/2015/april/microsoft-iis-and-aspnet-mvc-caching-techniques/

### 開發環境設定
1. [SCSS](http://sass-lang.com/install)
2. [SassyStudio](https://visualstudiogallery.msdn.microsoft.com/85fa99a6-e4c6-4a1c-9f00-e6a8129b6f4d)
3. [fire.app](https://github.com/KKBOX/FireApp/releases)
4. Sublime3 [破解](https://www.shuax.com/archives/SublimeText3Crack.html)
	- PackageControl
	- SCSS
	- SASS
	- EMMET
	- AUTOFILENAME

```
----- BEGIN LICENSE -----
Michael Barnes
Single User License
EA7E-821385
8A353C41 872A0D5C​​ DF9B2950 AFF6F667
C458EA6D 8EA3C286 98D1D650 131A97AB
AA919AEC EF20E143 B361B1E7 4C8B7F04
B085E65E 2F5F5360 8489D422 FB8FC1AA
93F6323C FD7F7544 3F39C318 D95E6480
FCCC7561 8A4A1741 68FA4223 ADCEDE07
200C25BE DBBC4855 C4CFB774 C5EC138C
0FEC1CEF D9DCECEC D3A5DAD1 01316C36
------ END LICENSE ------
```

5. Commands 
	- [SASS 安裝流程](https://www.youtube.com/watch?v=2rM5KP5X_UI)
	- cd C:\Project\efuntech.sms
	- compass watch EFunTech.Sms.Portal

### Angular 中文
1. https://github.com/angular/angular.js/tree/master/src/ngLocale
2. https://github.com/angular/bower-angular-i18n

### WebMarkupMin 
1. https://webmarkupmin.codeplex.com/wikipage?title=WebMarkupMin%201.0.0

### Character Entities
1. http://brajeshwar.github.io/entities/

### CSS pseudo-element
1. http://blog.mukispace.com/pseudo-elements-10-examples/

### Linq Lamba Join
http://stackoverflow.com/questions/9720225/how-to-perform-join-between-multiple-tables-in-linq-lambda

### modal 置中
http://codepen.io/dimbslmh/pen/mKfCc?editors=001
###  form-horizontal
<div class="container-fluid">
	<div class="row">
		<div class="col-md-12">
			<form class="form-horizontal" role="form">
				<div class="form-group">
					 
					<label for="inputEmail3" class="col-sm-2 control-label">
						Email
					</label>
					<div class="col-sm-10">
						<input type="email" class="form-control" id="inputEmail3" />
					</div>
				</div>
				<div class="form-group">
					 
					<label for="inputPassword3" class="col-sm-2 control-label">
						Password
					</label>
					<div class="col-sm-10">
						<input type="password" class="form-control" id="inputPassword3" />
					</div>
				</div>
				<div class="form-group">
					<div class="col-sm-offset-2 col-sm-10">
						<div class="checkbox">
							 
							<label>
								<input type="checkbox" /> Remember me
							</label>
						</div>
					</div>
				</div>
				<div class="form-group">
					<div class="col-sm-offset-2 col-sm-10">
						 
						<button type="submit" class="btn btn-default">
							Sign in
						</button>
					</div>
				</div>
			</form>
		</div>
	</div>
</div>

### 20150903 ANGULAR LOGGER MAX
1. http://ngmodules.org/modules/angular-logger-max
2. [Console.Re](http://console.re/)

### 20150902 TAB VERTICAL
1. https://gist.github.com/hugsbrugs/c85a6b1c10d03e8cb0da
2. http://stackoverflow.com/questions/26394640/angular-ui-bootstrap-vertical-tabs

### 20150902
### translate
1. https://angular-translate.github.io/
2. https://github.com/angular-translate/angular-translate/wiki/Multi-Language


### fancy website
1. http://yearofmoo-articles.github.io/angularjs-2nd-animation-article/app/#/
2. [$animate](https://medium.com/angularjs-meetup-south-london/angular-new-features-in-angular-1-4-b9b47077a8b2)

### Cast Anonymous Type
1. http://blogs.msmvps.com/jonskeet/2009/01/09/horrible-grotty-hack-returning-an-anonymous-type-instance/
2. http://stackoverflow.com/questions/10073319/returning-anonymous-type-in-c-sharp

### MvcMailer
1. https://github.com/smsohan/MvcMailer/wiki/MvcMailer-Step-by-Step-Guide

## Background Task in asp.net
1. [HowToRunBackgroundTasksInASPNET](http://www.hanselman.com/blog/HowToRunBackgroundTasksInASPNET.aspx)
2. [Hangfire](http://hangfire.io/)
3. [CRON](https://en.wikipedia.org/wiki/Cron#CRON_expression)
4. [使用 Hangfire 來處理非同步的工作](http://www.dotblogs.com.tw/rainmaker/archive/2015/08/19/153169.aspx)
5. [How To: Install Hangfire without ASP.NET MVC](http://frankouimette.com/tutorial-installing-hangfire-without-asp-net-mvc/)
6. https://github.com/phenixdotnet/Hangfire.Unity
7. http://docs.hangfire.io/en/latest/background-methods/using-ioc-containers.html
8. [解決 IOC 問題](http://stackoverflow.com/questions/27961210/hangfire-dependency-injection-lifetime-scope)

http://www.albahari.com/threading/part2.aspx#_Signaling_with_Event_Wait_Handles

## Cache
https://www.mnot.net/cache_docs/

## Speed up HTML
[加速前端網頁效能的14條規則](http://blog.miniasp.com/post/2007/11/24/14-rules-for-faster-front-end-performance-notes.aspx)

## Response.Filter
http://blog.miniasp.com/post/2009/02/21/Introduce-ASPNET-ResponseFilter-Property.aspx

## GZIP
http://www.iis.net/configreference/system.webserver/httpcompression
http://betterexplained.com/articles/how-to-optimize-your-site-with-gzip-compression/

## Browser Link
http://blogs.msdn.com/b/webdev/archive/2013/06/28/browser-link-feature-in-visual-studio-preview-2013.aspx

## 加速VisualStudio偵錯速度
1. [uncheck the checkbox for "Enable IntelliTrace](http://stackoverflow.com/questions/12567984/visual-studio-debugging-loading-very-slow)

## LocalDb in IIS 設定
1. http://vmiv.blogspot.tw/2012/03/q-localdb-iis.html

## Loading ngcloak
http://plnkr.co/edit/twGP7gUe9uraYXSr6kQG?p=preview

## CSS FAQ
1. [radio 按鈕與文字的對齊](http://www.phpernote.com/div-css/1085.html)
## Angular ng-cloak
1. [要額外加上css才有作用](http://blog.miniasp.com/post/2013/06/04/AngularJS-Expression-and-ngBind-directive-tips.aspx)

## Angular Debug
1. http://odetocode.com/blogs/scott/archive/2014/07/29/debugging-angularjs-in-the-console.aspx
2. http://blog.ionic.io/angularjs-console/

angular.element(targetNode).scope()
angular.element(targetNode).isolateScope()
angular.element(targetNode).injector().get('MyService')
angular.element(targetNode).controller('directiveName')


## EntityFramework
1. Tools
	- [Entity Framework Profiler 3.0](http://www.hibernatingrhinos.com/products/EFProf)
2. Performance
	- [Query Optimizations](http://www.farreachinc.com/blog/far-reach/2013/09/26/entity-framework-query-optimizations)
	- [Tips to improve Entity Framework Performance](http://www.dotnet-tricks.com/Tutorial/entityframework/J8bO140912-Tips-to-improve-Entity-Framework-Performance.html)
	- [IEnumerable VS IQueryable](http://www.dotnet-tricks.com/Tutorial/linq/I8SY160612-IEnumerable-VS-IQueryable.html)
	
## 資料庫中文亂碼
ALTER DATABASE [afi] COLLATE French_CI_AI

## 有叉叉的按鈕
http://demo.optimalbpm.se/angular-schema-form-dynamic-select/
https://material.angularjs.org/latest/

## Bootstrap
1. [theme](http://getbootstrap.com/examples/theme/)

## Generic Unit of work
https://genericunitofworkandrepositories.codeplex.com/wikipage?title=Find%20Object%20by%20Primary%20Key%28s%29&referringTitle=Documentation

## glimpse
1. [Glimpse Core](https://www.nuget.org/packages/Glimpse)
2. [ASP.NET MVC 使用Glimpse監測網站的一舉一動 2](http://kevintsengtw.blogspot.tw/2011/10/aspnet-mvc-glimpse-2.html)


## Angular 學習資源
1. [學習 AngularJS](https://github.com/jmcunningham/AngularJS-Learning/blob/master/ZH-TW.md)

## LinqKit
1. [使用 LINQKit PredicateBuilder 解決動態OR條件查詢窘境](http://www.dotblogs.com.tw/wasichris/archive/2014/12/20/147734.aspx)
2. [效能大戰番外編 - 性格乖僻扭曲的傢伙 為了摘一朵花 翻越整個青藏高原 之 使用 Expression Trees (運算式樹狀架構) 動態 操作 LINQ 消除重複程式片段 這真令人心情莫名興奮愉悅](http://weisnote.blogspot.tw/2013/08/expression-trees-linq.html)
3. [LINQ – LINQKit(PredicateBuilder)](http://www.dotblogs.com.tw/alonstar/archive/2010/06/30/16274.aspx)
4. [LINQ 動態查詢 LINQKit PredicateBuilder + Glimpse 效能監測](http://weisnote.blogspot.tw/2014/06/linq-linqkit-predicatebuilder-glimpse.html)
5. [PredicateBuilder True | False](http://leaflet-t-h.blogspot.tw/2011/11/linq-predicatebuilder.html)

## angular promise 用法
https://www.airpair.com/angularjs/posts/angularjs-promises

## ngClass 用法
```
ng:class="{true:'selected', false:''}[$index==selectedIndex]"
ng-class="{selected: $index==selectedIndex}"
ng-class="{admin:'enabled', moderator:'disabled', '':'hidden'}[user.role]"
ng-class="($index==selectedIndex) ? 'selected' : ''"
ng-class="condition ? 'trueClass' : 'falseClass'"
```
1. https://scotch.io/tutorials/the-many-ways-to-use-ngclass
2. 
## Bootstrap
1. http://getbootstrap.com/2.3.2/base-css.html#tables
2. 
## WebApi 設定
1. 設定 WebApiConfig.cs
2. Application_Start
GlobalConfiguration.Configure(WebApiConfig.Register);
3. UnityMvcActivator.cs
GlobalConfiguration.Configuration.DependencyResolver = new WebApiDependencyResolver(container);

## Table

1. [tutorial](http://ui-grid.info/docs/#/tutorial)
2. [grid selection] (http://ui-grid.info/docs/#/tutorial/210_selection)

http://lorenzofox3.github.io/smart-table-website/

ui-grid 與 bootstrap 一起用好像有問題

Brian
Github Twitter
http://brianhann.com/6-ways-to-take-control-of-how-your-ui-grid-data-is-displayed/
http://brianhann.com/create-a-modal-row-editor-for-ui-grid-in-minutes/#more-162
http://ui-grid.info/docs/#/tutorial/210_selection

## ui-grid
[文件](http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions)

## Angular Schema Form

官方網站 http://schemaform.io/
程式碼
文件
參考資料

### Loading Indicator
1. http://stackoverflow.com/questions/18743656/angular-ui-grid-how-to-show-a-loader
2. http://codingsmackdown.tv/blog/2013/04/20/using-response-interceptors-to-show-and-hide-a-loading-widget-redux/
3. https://github.com/lavinjj/angularjs-spinner



// 路徑說明
    // http://blog.miniasp.com/post/2008/10/19/URL-URI-Description-and-usage-tips.aspx

### tabs + ui-grid
http://plnkr.co/edit/73k0n2MzVgZpfUez6VTx?p=preview

### ui-grid height
Its quite crude but here you are.

This gets called whenever the rows in the grid change.

function setHeight(extra) {
      $scope.height = (($scope.gridOptions.data.length * 30) + 30);
      if (extra) {
        $scope.height += extra;
      }
      $scope.api.grid.gridHeight = $scope.height;
    }
And this is on the div ui-grid-auto-resize ng-style="{ 'height': height }"

I basically just resize the container based on the number of rows (plus the header) and allow auto-resize to handle the rest.


uiGridCtrl.focus is not a function
remove ui-grid-cellnav
https://github.com/angular-ui/ui-grid/issues/3589

### Component
1. http://www.jquerynewsticker.com/
2. https://github.com/Textalk/angular-schema-form
3. [AngualrMotion](http://mgcrea.github.io/angular-motion/)
4. http://ngmodules.org/popular

### ui-grid
1. [Create a Modal Row Editor for UI-Grid in Minutes](http://brianhann.com/create-a-modal-row-editor-for-ui-grid-in-minutes/)
2. [Filter](http://ui-grid.info/docs/#/tutorial/103_filtering)
3. 
### angular schema Form
1. [Extending Schema Form](https://github.com/Textalk/angular-schema-form/blob/master/docs/extending.md)
2. [SourceCode](https://github.com/Textalk/angular-schema-form)
3. [Documentation](https://github.com/Textalk/angular-schema-form/blob/development/docs/index.md)
4. [Examples](http://schemaform.io/examples/bootstrap-example.html)
5. CommonControllers.js

### 多國語系
1. [Jed](https://github.com/SlexAxton/Jed)

### Angular
1. [The Top 10 Mistakes AngularJS Developers Make](https://www.airpair.com/angularjs/posts/top-10-mistakes-angularjs-developers-make#5-service-vs-factory)
2. [AngularJS Service, Factory, Provider 差別](http://roxannera.blogspot.tw/2014/01/angularjs-service-factory-provider.html)
3. [Service，Factory 傻傻分不清楚](http://ithelp.ithome.com.tw/question/10161278)

Bootstrap v3.3.4 (http://getbootstrap.com)

### ASP.NET MVC 效能
1. http://stackoverflow.com/questions/2246251/how-to-improve-asp-net-mvc-application-performance
2. http://www.dotnetcurry.com/aspnet/1058/aspnet-website-performance-optimization
3. http://www.asp.net/mvc/overview/performance/bundling-and-minification
4. http://www.slideshare.net/SarveshKushwaha/tips-and-tricks-for-aspnet
5. http://codeclimber.net.nz/archive/2009/04/17/how-to-improve-the-performances-of-asp.net-mvc-web-applications.aspx

### File Upload
1. http://stackoverflow.com/questions/18571001/file-upload-using-angularjs
2. http://jsfiddle.net/JeJenny/ZG9re/
3. https://www.npmjs.com/package/angular-file-upload
4. http://ngmodules.org/modules/angular-file-upload


 <form class="form-horizontal">
        <h4>Tutorial</h4>
        <hr />
        <div class="form-group">
            <label for="title" class="col-md-2 control-label">Title</label>
            <div class="col-md-10">
                <input type="text" data-ng-model="tutorial.title" name="title" class="form-control" />
            </div>
        </div>

        <div class="form-group">
            <label for="description" class="col-md-2 control-label">Description</label>
            <div class="col-md-10">
                <textarea data-ng-model="tutorial.description" name="description" class="form-control"></textarea>
            </div>
        </div>

        <div class="form-group">
            <label for="attachment" class="col-md-2 control-label">Attachment</label>
            <div class="col-md-10">
                <input type="file" name="attachment" class="form-control" data-ak-file-model="tutorial.attachment" />
            </div>
        </div>

        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <input type="button" class="btn btn-primary" value="Save" data-ng-click="saveTutorial(tutorial)" />
            </div>
        </div>
    </form>

### System.ComponentModel.DataAnnotations
ValidationAttribute

