for f in artifacts/*.nupkg
do
    echo "Deploying $f"
    curl -X POST https://www.myget.org/F/kasbah/api/v2/package -H "X-NuGet-ApiKey: $MYGET_KEY" -F "data=@$f"
done