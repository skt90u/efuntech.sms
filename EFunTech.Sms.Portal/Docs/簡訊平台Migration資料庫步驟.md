簡訊平台 Migration 資料庫步驟 
=====

簡介
--------

簡訊平台，當在開發新的功能時，如果有異動資料庫 schema，必須先確保新的 schema 能夠正常運作，因此需要將 Azure 上面目前的資料庫匯入本機, 測試整個 Migration 過程是否正常，整個測試步驟如下：
	1. 匯出 Azure 資料庫
	2. 將 Azure 資料庫匯入本機的 LocalDB
	3. 對本機 LocalDB 執行 Migration，並記錄 Migration 步驟
	4. 依據步驟 3，對 Azure 資料庫執行 Migration

----------

匯出 Azure 資料庫
--------

![](images/ExportAzureDatabase01.png "選擇 Export Data-tier Application")
![](images/ExportAzureDatabase02.png "Save to local disk -> 選擇你要的路徑, 副檔名必須為 .bacpac")
![](images/ExportAzureDatabase03.png "點選 Finish")
![](images/ExportAzureDatabase04.png "匯出作業進行中")
![](images/ExportAzureDatabase05.png "匯出作業完成，點選 Close")

- - - -

將 Azure 資料庫匯入本機的 LocalDB
----------------------

![](images/ImportAzureDatabase01.png "選擇 Backup ...")
![](images/ImportAzureDatabase02.png "選擇要備份的路徑，副檔名必須為 .bak")
![](images/ImportAzureDatabase03.png "刪除 LocalDB 目前的 EFunTechSms 資料庫")
![](images/ImportAzureDatabase04.png "確定 Close existing connections，有被勾選")
![](images/ImportAzureDatabase05.png "匯入剛剛備份的資料庫，選擇 Import Data-tier Application")
![](images/ImportAzureDatabase07.png "指定要匯入的資料庫")
![](images/ImportAzureDatabase08.png "將 New database name 修改成 EFunTechSms")
![](images/ImportAzureDatabase09.png "點選 Finish")
![](images/ImportAzureDatabase10.png "匯入作業完成，點選 Close")

----------

對本機 LocalDB 執行 Migration，並記錄 Migration 步驟
-------

TODO

----------


依據步驟 3，對 Azure 資料庫執行 Migration
-------

TODO  

