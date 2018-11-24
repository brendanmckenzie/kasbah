set -e

cd src/Kasbah.Web.Management.UI
yarn
yarn run build

cd ../../

rm -rf src/Kasbah.Web/Management/Ui
mkdir -p src/Kasbah.Web/Management/Ui
cp -r src/Kasbah.Web.Management.UI/dist/* src/Kasbah.Web/Management/Ui
