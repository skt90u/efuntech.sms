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

### 修改最後一個commit訊息
local端修改
git commit --amend
發送至遠端修改
git pull
git push
## Git Merge
1. [fast-forward](http://lemonup.logdown.com/posts/166352-git-merge-fast-forward-difference)

## 設定
1. [diffmerge 下載](https://sourcegear.com/diffmerge/downloaded.php)
2. [diffmerge 設定](https://sourcegear.com/diffmerge/webhelp/sec__git__windows__msysgit.html)