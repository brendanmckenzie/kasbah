# Setup

To work with Kasbah projects locally you'll need to have your environment configured, follow the "Development infrastructure" steps in the [quickstart](/quickstart.md) guide to get up and running.

Once you have the infrastructure up and running, use the following command to initialise an empty Kasbah web project.

```sh
dotnet new kasbah-web KasbahDemo.Blog
```

This requires you to have the `kasbah-web` dotnet template installed; if you don't already have it, run the following command to install it.

```sh
dotnet new -i Kasbah.Web.Template
```

You will now have an empty Kasbah web application project ready to work with.

Navigate to the `KasbahDemo.Blog` and run `dotnet build` to restore the required NuGet packages and build the project.
