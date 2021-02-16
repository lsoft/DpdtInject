#add tag:
git log --pretty=oneline
git tag -a 0.2.1.3 -m "0.2.1.3" 3c1d046
git push origin 0.2.1.2

#delete tag:
git push --delete origin tagName 
git tag -d tagName 