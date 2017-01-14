set -e

# cd src/Kasbah.Web.ContentManagement.UI
# npm install
# npm run deploy:prod

# cd ../../

COMMIT_COUNT=$(git log | wc -l | awk '{print $1}')
DOTNET_PACK_OPTS="-c Release -o ./artifacts --version-suffix=build$COMMIT_COUNT"

dotnet pack src/Kasbah $DOTNET_PACK_OPTS
dotnet pack src/Kasbah.DataAccess.ElasticSearch $DOTNET_PACK_OPTS
dotnet pack src/Kasbah.Web.ContentDelivery $DOTNET_PACK_OPTS
dotnet pack src/Kasbah.Web.ContentManagement $DOTNET_PACK_OPTS