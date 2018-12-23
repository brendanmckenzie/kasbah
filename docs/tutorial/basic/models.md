# Data modeling

The next step is to setup your data models.

Create a file called `WebRoot.cs` in a new folder called `Models` within your new project.

Add the following. The comments can be omitted, they are included in this file only to serve as annotations to help you understand the reason behind each line.

```csharp
using Kasbah.Content.Attributes; // for `IconAttribute`
using Kasbah.Content.Models; // for `Item`

namespace KasbahDemo.Blog.Models
{
    [Icon("badge", "#FF3860")] // this is optional, it provides developers the ability to customise the appearance of these items in the content tree
    public class WebRoot : Item // all content types in Kasbah must implement the `IItem` interface.  the `Item` class includes a basic implementation of the interface to save code duplicity
    {
    }
}
```

Create another file called `HomePage.cs` also in the `Models` folders with the following content.

```csharp
using Kasbah.Content.Attributes;
using Kasbah.Web;
using Kasbah.Web.Models;

namespace KasbahDemo.Blog.Models
{
    [Icon("home", "#FA6900")]
    public class HomePage : BasePresentable, IPresentable
    {
        public string Title { get; set; }
    }
}
```

_n.b. Data types that implement the `IPresentable` interface are "routable" by Kasbah - if a request comes in and it resolves to content implementing the interface, it will be processed, rendered and returned to the HTTP request. If a request comes in for content that does not implement the interface, an error will be returned. See the [quickstart](/quickstart.md#Modelling%20data) for more information._

The final model required is the blog post - create a file called `BlogPost.cs` in the `Models` folder with the following content.

```csharp
using Kasbah.Content.Attributes;
using Kasbah.Web;
using Kasbah.Web.Models;

namespace KasbahDemo.Blog.Models
{
    [Icon("file-alt", "#FACA66")]
    public class BlogPost : BasePresentable, IPresentable
    {
        public string Title { get; set; }

        public string Content { get; set; }
        
        public string Summary { get; set; }
    }
}
```
