#add tag:
git log --pretty=oneline
git tag -a 0.2.2.3 -m "0.2.2.3" commit_hash
git push origin 0.2.2.3
git push --tags

#delete tag:
git push --delete origin tagName 
git tag -d tagName 

