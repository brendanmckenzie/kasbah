for f in artifacts/*.nupkg
do
    curl -X POST https://www.myget.org/F/kasbah/api/v2/package -H "X-NuGet-ApiKey: $MYGET_KEY" -F "data=@artifacts/$f"
done