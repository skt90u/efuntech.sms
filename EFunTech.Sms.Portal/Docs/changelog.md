## Change Log
All notable changes to this project will be documented in this file.
This project adheres to [Semantic Versioning](http://semver.org/).

## Fixed - 2016-06-30
- 修正【系統設定】按鈕在IE下無法顯示的問題
- 尚未解決客戶說的重複發送的問題
select * from LogItems  where (Message like '%972038199%' or Message like '%960624158]%') and Message like '%3121%'
select * from LogItems  where Method = 'RetrySMS' and (Message like '%972038199%' or Message like '%960624158]%')

## Fixed - 2016-05-09
- 修復無法刪除使用者的問題(Workaround)

## Fixed - 2016-03-31
- 檢核簡訊規則的重要參數 UseParam 判斷錯誤，目前改成以下方式進行判斷。
	UseParam = -1 != window.location.hash.trim().toUpperCase().indexOf('SendParamMessage'.toUpperCase());
- angular router 改成路徑不區分大小寫

## Fixed - 2016-03-31
- 簡訊平台變更事項
	* 高品質線路對應帳號 -> ENVOTIONS
	* 一般線路對應帳號 -> ZU2SMSC
- 改成使用 GMAIL 發送 EMAIL
- 暫時禁止操作者刪除子帳號，等待刪除帳號功能修復為止
- 參數簡訊提示字樣

## Add - 2016-02-21
- 新增重試機制

## Fixed - 2015-12-13
- 修正相對路徑設定
	* (ServerSide) 將 /SendMessage 改成 SendMessage
	* (ClientSide) 將 /api/AllContact 改成 api/AllContact
- 解決 GroupContact 設定常用聯絡人錯誤

## Fixed - 2015-11-30
- 加入英文API說明文件
- 解決使用API發送簡訊在LOG中無法顯示使用者的問題

## Fixed - 2015-11-29
- 調整 SectorStatistics 查詢效能

## Fixed - 2015-11-27
- 調整 LogItems 查詢效能
- BasicAuthApi 加上 Log
 
## Fixed - 2015-11-27
- 解決 SearchMemberSend 依照狀態查詢無作用的問題
- 調整 SearchMemberSend 查詢效能

## Fixed - 2015-11-26
- [解決 MemberSendMessageStatisticController 使用 LinqKit StackOverFlow 問題](http://kalcik.net/2014/01/05/joining-data-in-memory-with-data-in-database-table/)

## Fixed - 2015-11-23
- Eric 要求發送失敗不回補點數
 
## Fixed - 2015-11-21
- 修正 **查詢發送狀態** 不使用接收狀態查詢與使用接收狀態(全選)查詢，兩者之間筆數不一致的問題
- 修正 **查詢發送狀態** 即使發送成功，點選明細看到的仍然是 Timeout 的問題

## Add - 2015-11-20
- 加入 系統公告管理

## Add - 2015-11-19
- 取得派送結果邏輯修改成，每分鐘取得派送結果，直到所有派送結果都取完為止，但只持續一天(若一天之內尚未取完所有派送結果，就不再取，並回補點數)
- 對於一天之內尚未取完所有派送結果的 SendMessageHistory 執行扣點
- 加入 HouseKeep 機制 (僅保留一個月以內的LogItems)
- footer 暫時只保留 **Copyright © 2015 資餘科技股份有限公司版權所有，轉載必究**
- 修改 wording 
	1. 逾期收訊 -> 傳送失敗
	2. 額度類別 -> 類別
	3. 點數購買與匯轉明細 -> 查詢點數明細
- 修正登入頁登入失敗出現奇怪現象，移除
	1. @Html.ValidationMessageFor(m => m.Password, "", new { @class = "text-danger" })
	2. @Html.ValidationMessageFor(m => m.UserName, "", new { @class = "text-danger" })
 
## Add - 2015-11-13
- 允許手動輸入門號，可使用全形或者半形的逗號。
- 如果發送失敗，就回補點數。

## Fixed - 2015-11-10
- 修正 entity.SentDate = DeliveryReport.SentDate; NullReferenceException 的問題，發生原因：Infobip 在取得GetDeliveryReport 的時候，可能不會一次性的將所有派送結果都回傳，因此有可能在寫入 History 時，還沒找到對應的派送結果。 

## Fixed - 2015-11-06
- 修正查詢發送紀錄查不到資料的問題
- 修正同時執行多個 **相同參數的** Background Job

## Fixed - 2015-11-01
- 將 LogLevel 改成使用 Flag 之後，Hangfire 仍然使用舊的方式(Debug = 0, Info = 1, Warn = 2, Error = 3)，找不出發現原因，目前沒有解決辦法，只好改回來

## Fixed - 2015-10-30
- 修正 [GMAIL 在 Azure 發送問題](http://webstackoflove.com/use-gmail-to-deliver-email-from-azure-website/)
- 在 MessageReceiver 新增 E164Mobile, Region 以方便偵錯

----------------------------------------
以下為 changelog 範例
----------------------------------------

### Added
- RU translation from @aishek.
- pt-BR translation from @tallesl.

## [0.2.0] - 2015-10-06
### Changed
- Remove exclusionary mentions of "open source" since this project can benefit
both "open" and "closed" source projects equally.

## [0.1.0] - 2015-10-06
### Added
- Answer "Should you ever rewrite a change log?".

### Changed
- Improve argument against commit logs.
- Start following [SemVer](http://semver.org) properly.

## [0.0.8] - 2015-02-17
### Changed
- Update year to match in every README example.
- Reluctantly stop making fun of Brits only, since most of the world
  writes dates in a strange way.

### Fixed
- Fix typos in recent README changes.
- Update outdated unreleased diff link.

## [0.0.7] - 2015-02-16
### Added
- Link, and make it obvious that date format is ISO 8601.

### Changed
- Clarified the section on "Is there a standard change log format?".

### Fixed
- Fix Markdown links to tag comparison URL with footnote-style links.

## [0.0.6] - 2014-12-12
### Added
- README section on "yanked" releases.

## [0.0.5] - 2014-08-09
### Added
- Markdown links to version tags on release headings.
- Unreleased section to gather unreleased changes and encourage note
keeping prior to releases.

## [0.0.4] - 2014-08-09
### Added
- Better explanation of the difference between the file ("CHANGELOG")
and its function "the change log".

### Changed
- Refer to a "change log" instead of a "CHANGELOG" throughout the site
to differentiate between the file and the purpose of the file — the
logging of changes.

### Removed
- Remove empty sections from CHANGELOG, they occupy too much space and
create too much noise in the file. People will have to assume that the
missing sections were intentionally left out because they contained no
notable changes.

## [0.0.3] - 2014-08-09
### Added
- "Why should I care?" section mentioning The Changelog podcast.

## [0.0.2] - 2014-07-10
### Added
- Explanation of the recommended reverse chronological release ordering.

## 0.0.1 - 2014-05-31
### Added
- This CHANGELOG file to hopefully serve as an evolving example of a standardized open source project CHANGELOG.
- CNAME file to enable GitHub Pages custom domain
- README now contains answers to common questions about CHANGELOGs
- Good examples and basic guidelines, including proper date formatting.
- Counter-examples: "What makes unicorns cry?"

[Unreleased]: https://github.com/olivierlacan/keep-a-changelog/compare/v0.2.0...HEAD
[0.2.0]: https://github.com/olivierlacan/keep-a-changelog/compare/v0.1.0...v0.2.0
[0.1.0]: https://github.com/olivierlacan/keep-a-changelog/compare/v0.0.8...v0.1.0
[0.0.8]: https://github.com/olivierlacan/keep-a-changelog/compare/v0.0.7...v0.0.8
[0.0.7]: https://github.com/olivierlacan/keep-a-changelog/compare/v0.0.6...v0.0.7
[0.0.6]: https://github.com/olivierlacan/keep-a-changelog/compare/v0.0.5...v0.0.6
[0.0.5]: https://github.com/olivierlacan/keep-a-changelog/compare/v0.0.4...v0.0.5
[0.0.4]: https://github.com/olivierlacan/keep-a-changelog/compare/v0.0.3...v0.0.4
[0.0.3]: https://github.com/olivierlacan/keep-a-changelog/compare/v0.0.2...v0.0.3
[0.0.2]: https://github.com/olivierlacan/keep-a-changelog/compare/v0.0.1...v0.0.2