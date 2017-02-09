# Kasbah

Version 20 (incarnation 5).

# Getting started

Docker is the easiest way to run the services required to power Kasbah.

## Requirements

 * Kasbah uses ElasticSearch as its datastore, index and store behind the analytic/personalisation engine.
    * Run a local development instance using docker by executing: `docker run -d -p '9200:9200' elasticsearch`
 * Kasbah uses Redis for its caching layer.
    * Run a local development instance using docker by executing: `docker run -d -p '6379:6379' redis`

The fastest way to get started with Kasbah is to use the Yeoman generator.

    npm install -g yeoman generator-kasbah
    yo kasbah

Follow the prompts and you'll have the code ready to build a Kasbah based web application.

This generator assumes that you have ElasticSearch and Redis running on the local host.  If this isn't the case then simple modify the generatored `.vscode/launch.json` file to point to the correct instances.

Run the administration site by executing `dotnet run` in the src/*.ContentManagement folder.  The site will be available at http://localhost:5000/

## Development

To debug the Kasbah framework simply clone the Kasbah repository to the same location your generated website is located and your project will use the local version of Kasbah instead of the NuGet packages.  For example, if your website is located in the folder `/projects/my-website`, clone the Kasbah source code to `/projects/kasbah`.

See the generated .csproj files for how this reference is hooked up, or to customise the location of the Kasbah source code.

Then using Visual Studio Code (or Visual Studio 2017+) you will be able to debug your website and the underlying Kasbah framework at the same time.

# Authors

 * Brendan McKenzie - *Lead Developer* - [https://www.brendanmckenzie.com/](https://www.brendanmckenzie.com/)

# License

The licensing status of this project is still under consideration.  I (Brendan) am not ready to open source the project as I do see some commercial viability in it.
