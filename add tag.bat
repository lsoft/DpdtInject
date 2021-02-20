#add tag:
git log --pretty=oneline
git tag -a 0.2.2.1 -m "0.2.2.1" commit_hash
git push origin 0.2.2.1

#delete tag:
git push --delete origin tagName 
git tag -d tagName 