using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ITNSBOCustomization.Lib.Utils
{
    public class AssemblyHelper
    {
        public static IEnumerable<Type> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class
        {
            Console.WriteLine("Runtime Assemblies of events" +" " + JsonConvert.SerializeObject(Assembly.GetEntryAssembly()));
            List<Type> objects = new List<Type>();
            var types = Assembly.GetEntryAssembly().GetTypes();
            //Console.WriteLine("entry assembly types : " + JsonConvert.SerializeObject(types));
            foreach (Type type in
                types
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                objects.Add(type);
            }
            //objects.Sort(); commented out due to issue
            Console.WriteLine("Inherited classes"+JsonConvert.SerializeObject(objects));
            return objects;
        }

        public static string GetEmbeddedResource(string resourceName)
        {
            Console.WriteLine(" assembly data {0}", JsonConvert.SerializeObject(Assembly.GetCallingAssembly()));
            var data = GetEmbeddedResource(resourceName, Assembly.GetCallingAssembly());
            Console.WriteLine("embedded resource data : {0} {1}", resourceName,  data);
            return data;
        }

        public static byte[] GetEmbeddedResourceAsBytes(string resourceName)
        {
            return GetEmbeddedResourceAsBytes(resourceName, Assembly.GetCallingAssembly());
        }

        public static string GetEmbeddedResource(string resourceName, Assembly assembly)
        {
            resourceName = FormatResourceName(assembly, resourceName);
            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                    return null;

                using (StreamReader reader = new StreamReader(resourceStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static byte[] GetEmbeddedResourceAsBytes(string resourceName, Assembly assembly)
        {
            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                byte[] content = new byte[resourceStream.Length];
                resourceStream.Read(content, 0, content.Length);

                return content;
            }
        }

        private static string FormatResourceName(Assembly assembly, string resourceName)
        {
            return assembly.GetName().Name + "." + resourceName.Replace(" ", "_").Replace("\\", ".").Replace("/", ".");
        }
    }
}
