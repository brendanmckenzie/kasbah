set -e

pushd src/Kasbah.Web.ContentManagement.UI
npm install
npm run deploy:prod
popd
