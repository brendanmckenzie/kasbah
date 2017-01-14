set -e

COMMIT_COUNT=$(git log | wc -l | awk '{print $1}')
DOTNET_PACK_OPTS="-c Release -o ./artifacts --version-suffix=build$COMMIT_COUNT"

dotnet restore src/**/project.json

dotnet pack src/Kasbah $DOTNET_PACK_OPTS
dotnet pack src/Kasbah.DataAccess.ElasticSearch $DOTNET_PACK_OPTS
dotnet pack src/Kasbah.Web.ContentDelivery $DOTNET_PACK_OPTS
dotnet pack src/Kasbah.Web.ContentManagement $DOTNET_PACK_OPTS