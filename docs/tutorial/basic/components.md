# Creating the components

All components in Kasbah web applications are React components. The dotnet command used to initialise the project includes a basic layout and filesystem structure for you to start creating components.

Create a C# class in the folder called `BlogListing` with the following content.

```
using System.Threading.Tasks;
using Kasbah.Web;

namespace KasbahDemo.Blog.Models
{
    public class BlogListing
    {
        public async Task<object> GetModelAsync(object properties, KasbahWebContext context)
        {
            var posts = Enumerable.Empty<object>();

            return new
            {
                posts
            };
        }
    }
}
```

Create a file called `BlogListing.js` in the `ClientApp` folder with the following content.

```jsx
import React from 'react'
import Markdown from 'react-markdown'

const BlogListing = ({ posts }) => (
  <div>
    {posts.map(ent => (
      <div key={ent.id}>
        <strong>{ent.title}</strong>
        <Markdown source={ent.summary} />
      </div>
    ))}
  </div>
)

export default BlogListing
```

__TODO: add code to populate `items`__
