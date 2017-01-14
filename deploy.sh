for f in artifacts/*.nupkg
do
    curl -X POST https://www.myget.org/F/kasbah/api/v2/package -H "X-NuGet-ApiKey: 93888c8f-2ab2-4ee6-b9fa-4c99f47f0bb9" -F "data=@artifacts/$f"
done