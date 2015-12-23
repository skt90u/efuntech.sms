## Branch 操作

1. 建立 local branch

	```
	git checkout -b branchName
	```

2. 發送 local branch 至 remote

	```
	git push -u origin branchName
	```
3. 抓取 remote branch 至 local branch

	```
	git branch branchName origin/branchName 
	git checkout branchName
	``` 
4. merge branch to master

	```
	TODO
	```
5. 刪除 local branch

	```
	git branch -D branchName
	```
6. 刪除 remote branch

	```
	git push origin --delete branchName
	```
