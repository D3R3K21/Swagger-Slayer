using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nancy;
using Nancy.Extensions;
using Nancy.Routing;

namespace Swagger_Slayer
{
    public class DocumentationProvider : IRouteMetadataProvider
    {
        private static readonly List<Type> Types;
        static DocumentationProvider()
        {
            Types = Assembly.GetEntryAssembly().GetTypes()
                .Where(p => p.IsInterface && p.GetCustomAttributes().Any(a => a.GetType() == typeof(DocumentationDescriptorAttribute))).ToList();
            var nancyModeuleType = typeof(NancyModule);
            var declaredModules = Assembly.GetEntryAssembly().GetTypes().Where(p => nancyModeuleType != p && nancyModeuleType.IsAssignableFrom(p)).ToList();
            if (declaredModules.Count != Types.Count)
            {
                //TODO:(drose) throw exception to enforce usage?
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Out.WriteLine($"Warning, {declaredModules.Count} Nancy module(s) defined, and {Types.Count} documentation interface(s) defined");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public Type GetMetadataType(INancyModule module, RouteDescription routeDescription)
        {
            return typeof(DocumentationObject);
        }

        public object GetMetadata(INancyModule module, RouteDescription routeDescription)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (module.GetType().Assembly != entryAssembly) return null;

            DocumentationObject val = null;
            var eps = BuildDocumentation(Types);
            string path;
            var basePath = "/";
            if (module.ModulePath.Length > 1)
            {
                basePath = module.ModulePath;
                path = routeDescription.Path.Replace(module.ModulePath, "");
                if (string.IsNullOrEmpty(path)) path = "/";
            }
            else
            {
                path = routeDescription.Path;
            }
            val = eps.FirstOrDefault(p => string.Equals(routeDescription.Method, p.HttpMethod.ToString(), StringComparison.CurrentCultureIgnoreCase) && p.BasePath == basePath && path == p.Path);
            if (val == null)
            {
                //TODO:(drose) throw exception to enfore usage?
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Out.WriteLine($"No Documentation Found For {module.GetModuleName()}Module : {routeDescription.Method} {path}");
                Console.ForegroundColor = ConsoleColor.White;
                return null;
            }
            val.BasePath = basePath;
            val.ModuleName = module.GetModuleName().ToLower();

            return val;
        }

        private List<DocumentationObject> BuildDocumentation(List<Type> moduleInterfaces)
        {
            var endpointDocs = new List<DocumentationObject>();
            foreach (var moduleInterface in moduleInterfaces)
            {
                var descriptor = (DocumentationDescriptorAttribute)moduleInterface.GetCustomAttributes().First(a => a.GetType() == typeof(DocumentationDescriptorAttribute));
                var declaredMethods = moduleInterface.GetMethods();
                foreach (var method in declaredMethods)
                {
                    var doc = new DocumentationObject();
                    doc.BasePath = descriptor.BasePath;
                    var descriptionAttribute =
                        method.GetCustomAttributes().FirstOrDefault(p => p.GetType() == typeof(RouteDescriptorAttribute)) as RouteDescriptorAttribute;
                    if (descriptionAttribute == null) throw new Exception("All methods declared on the interface must have a description attribute");
                    doc.Path = descriptionAttribute.Path;
                    doc.HttpMethod = descriptionAttribute.HttpMethodType.ToString();
                    doc.Description = descriptionAttribute.Description;
                    var responseType = method.ReturnType;
                    doc.ResponseModel = GenerateObjectDictionary(responseType);
                    var parameters = method.GetParameters();
                    if (parameters.Length != 0)
                    {
                        if (parameters.Length > 1) throw new Exception("interface for module should have one arg (view model) or 0");
                        var requestParam = parameters.First();
                        var requestType = requestParam.ParameterType;
                        doc.RequestModel = GenerateObjectDictionary(requestType);
                    }
                    endpointDocs.Add(doc);
                }

            }
            return endpointDocs;
        }
        private static Dictionary<string, object> GenerateObjectDictionary(Type modelType)
        {
            var returnVal = new Dictionary<string, object>();
            var properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var requiredProperties = properties.Where(p => p.GetCustomAttributes().Any(x => x.GetType() == typeof(PropertyRequiredAttribute))).Select(c => c.Name).ToList();
            if (requiredProperties.Any())
            {
                returnVal.Add("Required Properties", string.Join(", ", requiredProperties));
            }
            foreach (var property in properties.Where(p => p.GetCustomAttributes().All(x => x.GetType() != typeof(IgnorePropertyAttribute))))
            {
                if (property.PropertyType.GetInterfaces().Contains(typeof(ICollection)) && !property.PropertyType.IsArray && property.PropertyType.GetGenericArguments().Length > 0)
                {
                    var args = property.PropertyType.GetGenericArguments();
                    switch (args.Length)
                    {
                        case 1:
                            {
                                var genericType = property.PropertyType.GetGenericArguments().First();
                                if (genericType == modelType)
                                {
                                    returnVal.Add(property.Name, Array.CreateInstance(genericType, 0).GetType().Name);
                                }
                                else if (genericType.Assembly.GetName().Name != "mscorlib")
                                {
                                    returnVal.Add(property.Name, new[] { GenerateObjectDictionary(genericType) });
                                }
                                else
                                {
                                    returnVal.Add(property.Name, Array.CreateInstance(genericType, 0).GetType().Name);
                                }
                                break;
                            }
                        case 2:
                            {
                                var genericType1 = property.PropertyType.GetGenericArguments().First();
                                object key;
                                object val;
                                if (genericType1 == modelType)
                                {
                                    key = genericType1.Name;
                                }
                                else if (genericType1.Assembly.GetName().Name != "mscorlib")
                                {
                                    key = GenerateObjectDictionary(genericType1);
                                }
                                else
                                {
                                    key = genericType1.Name;
                                }
                                var genericType2 = property.PropertyType.GetGenericArguments().Last();
                                if (genericType2 == modelType)
                                {
                                    val = genericType2.Name;
                                }
                                else if (genericType2.Assembly.GetName().Name != "mscorlib")
                                {
                                    val = GenerateObjectDictionary(genericType2);
                                }
                                else
                                {
                                    val = genericType2.Name;
                                }
                                returnVal.Add(property.Name, new[] { new { Key = key, Value = val } });
                                break;
                            }
                        default:
                            {
                                Console.Out.WriteLine($"Error Deserializing Model For Documentation : {property.PropertyType.Name}");
                                break;
                            }
                    }
                }
                else if (property.PropertyType.Assembly.GetName().Name == "mscorlib" || property.PropertyType == modelType)
                {
                    returnVal.Add(property.Name, property.PropertyType.Name);
                }
                else
                {
                    returnVal.Add(property.Name, GenerateObjectDictionary(property.PropertyType));
                }

                (returnVal.LastOrDefault().Value as Dictionary<string, object>)?.Add("ModelType", property.PropertyType.Name);
            }

            return returnVal;
        }
    }
}
