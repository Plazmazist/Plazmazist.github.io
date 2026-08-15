using System;
using System.Linq;
using System.Reflection;

var asmPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages", "aspnetstaticcontrib", "1.2.1", "lib", "net10.0", "AspNetStaticContrib.dll");
var asm = Assembly.LoadFrom(asmPath);
foreach (var t in asm.GetTypes().OrderBy(t => t.FullName))
{
    if (t.FullName!.Contains("Static") || t.FullName!.Contains("Resource") || t.FullName!.Contains("Page"))
    {
        Console.WriteLine(t.FullName);
        foreach (var m in t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly).OrderBy(m => m.Name))
        {
            Console.WriteLine($"  {m.Name}({string.Join(", ", m.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name))})");
        }
    }
}
