# Core concepts

One of the main motivation points of Kasbah was to provide developers with the power and flexibility to provide quality solutions to their clients without the platform or framework getting in their way.

A lot of existing content management platforms place the power of configuration - such as content data modeling - in the hands of content authors where it should belong* to the developers.

## Data modeling

When building solutions in Kasbah, all data modeling is done in code using C# interfaces to provide the flexibility of multiple inheritence.

Although the data storage layer is built in a flexible manner where any style of database can be used the Kasbah core is built to leverage the power of the PostgreSQL RDBMS.

## Data access

Querying data in Kasbah from code is done using a simple LINQ interface.  To provide developers ultimate power, there are no intermediary "Kasbah" data types, you deal with your own classes and interfaces.

## Web applications

The core of Kasbah was built in an abstract format where it could be applied to any implementation, however the main intended use in in web applications.

The `Kasbah.Web` series of packages extends on the core of Kasbah to provide this functionality.

Projects based on the Kasbah.Web implementation are offered modern React-based SPA functionality by default.

Developers start by modeling their data, defining what websites are included in their solution (Kasbah.Web offers multi-site by default), then specifying what components are available.  Components are then written in React and are available to be placed in placeholders in predefined layouts.  See the [getting started](/quickstart.md) section for a more detailed explanation of creating web applications with Kasbah.

### Segregated roles

Kasbah web applications run in two modes to offer a greater level of security.

* __Content delivery__ is the mode run to serve the public website, this serves as a read-only view to the data stored in the Kasbah content tree while feeding analytical data back to the system
* __Content management__ is the mode where authors are able to update content and publish changes when they are ready to go live
