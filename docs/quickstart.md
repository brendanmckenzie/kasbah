# Getting started

Kasbah was intentionally built to require a minimal infrastructure footprint.

The recommended approach to provisioning the infrastructure required for developing Kasbah projects is to leverage containerisation such as [Docker](https://docker.io).

## Development infrastructure

The only external software required to run Kasbah projects are -

- [PostgreSQL](https://www.postgres.org)
- [Redis](https://redis.io)

Using docker, you can initialise the required infrastructure with the following commands.

```sh
docker run -d --name kasbah_db -p '5432:5432' postgres
docker run -d --name kasbah_cache -p '6379:6379' redis
```

## Installation

Kasbah packages are distributed by NuGet. The simplest way to get started is by installing the `dotnet` CLI templates.

// TODO: provide documentation around using CLI templates

Alternatively you can create an empty web application and install the required NuGet packages for your use-case.

The footprint of a Kasbah project is kept as minimal as possible. Once the project has been installed using the CLI, you may need to update the `launch.json` configuration to point to the correct Redis and PostgreSQL instances.

Kasbah manages its own database schema including migrations, therefore it is safe to point kasbah to an empty database, provided there is sufficient access given to the database to make schema changes.

## Configuration

The default project comes with some base configuration and predefined models that can be customised.

If you have started an empty project and installed the NuGet packages manually you will need to update your `Startup` class with some basic configuration.

You will also need to create a class that inherits `KasbahWebRegistration` to configure sites, models and components.

In its most basic form a registration class would look similar to below -

```cs
public class WebRegistration : KasbahWebRegistration
{
    // this method is where developers define what sites are served by this solution
    public override void RegisterSites(SiteRegistry siteRegistry)
    {
        siteRegistry.RegisterSite(config =>
        {
            config.Alias = "kasbah"; // this provides a friendly short name for the site
            config.Domains = new[] { "localhost:5001", "www.kasbah.io" }; // when a request is processed by Kasbah these values are used to match the request to the site
            config.DefaultDomain = "www.kasbah.io"; // when resolving links within a site, this domain is used when a URL is requested to include the dmoain
            config.UseSsl = true; // similar to above, this tells the link resolved to provide `http` or `https` URLs
            config.ContentRoot = new[] { "sites", "kasbah", "home" }; // if the request matches the site, content is searched for at this location in the content tree - i.e., a request to https://www.kasbah.io/example-page will be resolved to content in the Kasbah tree at the path `/sites/kasbah/home/example-page`
        });
    }

    // this method declares what types of data are available within the solution
    public override void RegisterTypes(TypeRegistry typeRegistry)
    {
        typeRegistry.Register<WebRoot>();
        typeRegistry.Register<SiteRoot>();
        typeRegistry.Register<HomePage>(config =>
        {
            config
                .SetOption("view", nameof(HomePage));
        });
    }

    // this method registers the components available within the system
    public override void RegisterComponents(ComponentRegistry componentRegistry)
    {
        componentRegistry.RegisterComponent<Components.BodyContent>();
    }
```

## Modeling data

All data types that are to be used within a Kasbah project must inherit the `Kasbah.Content.Models.IItem` interface.

As there are a number of properties defined in the interface, to save developers time there is a class `Kasbah.Content.Models.Item` that implements these properties; as such in most cases you may wish to simply inherit your data classes from the `Item` class.

A simple data model would look something like the class below.

```cs
    public class BodyContent : IItem
    {
        public string Text { get; set; }
    }
```

For web applications, data models that inherit the `IPresentable` interface are considered renderable.  If a request comes in that resolves to an item not inheriting `IPresentable` and 404 HTTP response will be served.

## Components

Following on from the `BodyContent` example data model above, a possible implementation of the React component could be as below.

```js
import React from 'react'
import Markdown from 'react-markdown'

const BodyContent = ({ model }) => (
  <Markdown source={model.text} />
)

export default BodyContent
```
