set -e

rm -rf src/Kasbah.Web.ContentManagement/wwwroot
cp -r src/Kasbah.Web.ContentManagement.UI/dist src/Kasbah.Web.ContentManagement/wwwroot

if [ -z "$VERSION_SUFFIX" ]; then
    COMMIT_COUNT=$(git rev-list --all --count)
    COMMIT_COUNT_PADDED=$(printf "%04d" $COMMIT_COUNT)
    VERSION_SUFFIX="build$COMMIT_COUNT_PADDED"
fi

DOTNET_PACK_OPTS="-c Release -o $(pwd)/artifacts --version-suffix=$VERSION_SUFFIX"

dotnet restore

dotnet pack src/Kasbah $DOTNET_PACK_OPTS
dotnet pack src/Kasbah.Media $DOTNET_PACK_OPTS
dotnet pack src/Kasbah.Provider.Npgsql $DOTNET_PACK_OPTS
dotnet pack src/Kasbah.Provider.Aws $DOTNET_PACK_OPTS
dotnet pack src/Kasbah.Web $DOTNET_PACK_OPTS
dotnet pack src/Kasbah.Web.ContentDelivery $DOTNET_PACK_OPTS
dotnet pack src/Kasbah.Web.ContentManagement $DOTNET_PACK_OPTS
