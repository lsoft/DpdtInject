#add tag:
git log --pretty=oneline
git tag -a 0.2.1.4 -m "0.2.1.4" commit_hash
git push origin 0.2.1.4

#delete tag:
git push --delete origin tagName 
git tag -d tagName 