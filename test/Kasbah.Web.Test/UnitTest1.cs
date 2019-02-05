using System;
using System.Collections;
using System.Linq;
using Xunit;
using Newtonsoft.Json;
using Kasbah.Web.Models;

namespace Kasbah.Web.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var list = new ComponentCollection();
            list.Add("test", new [] { new Component { Control = "control", DataSource = null } });

            var json = JsonConvert.SerializeObject(list);

            Console.WriteLine(json);

            Assert.NotNull(json);
        }

        [Fact]
        public void Test2()
        {
            var json = "{\"test\":[{\"Control\":\"control\",\"DataSource\":null}]}";

            var list = JsonConvert.DeserializeObject<ComponentCollection>(json);

            Console.WriteLine(JsonConvert.SerializeObject(list));

            Assert.NotNull(list);
            Assert.Equal(json, JsonConvert.SerializeObject(list));
        }
    }
}
