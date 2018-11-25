set -e

if [ -z "$VERSION_SUFFIX" ]; then
    COMMIT_COUNT=$(git rev-list --all --count)
    COMMIT_COUNT_PADDED=$(printf "%04d" $COMMIT_COUNT)
    VERSION_SUFFIX="build$COMMIT_COUNT_PADDED"
fi

DOTNET_PACK_OPTS="-c Release -o $(pwd)/artifacts --version-suffix=$VERSION_SUFFIX"

dotnet restore

dotnet pack $DOTNET_PACK_OPTS
