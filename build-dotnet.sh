set -e

cp -r src/Kasbah.Web.ContentManagement.UI/dist src/Kasbah.Web.ContentManagement/wwwroot

COMMIT_COUNT=$(git rev-list --all --count)
DOTNET_PACK_OPTS="-c Release -o ./artifacts --version-suffix=build$COMMIT_COUNT"

dotnet restore src/**/project.json

dotnet pack src/Kasbah $DOTNET_PACK_OPTS
dotnet pack src/Kasbah.DataAccess.ElasticSearch $DOTNET_PACK_OPTS
dotnet pack src/Kasbah.Web.ContentDelivery $DOTNET_PACK_OPTS
dotnet pack src/Kasbah.Web.ContentManagement $DOTNET_PACK_OPTS