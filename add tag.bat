#add tag:
git log --pretty=oneline
git tag -a tagName -m "tagDescription" commithash
git push origin tagName

#delete tag:
git push --delete origin tagName 
git tag -d tagName 