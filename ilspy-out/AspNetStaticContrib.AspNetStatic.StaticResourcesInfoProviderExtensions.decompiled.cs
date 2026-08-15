using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AspNetStatic;
using AspNetStatic.Optimizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using ThrowGuard;

namespace AspNetStaticContrib.AspNetStatic;

public static class StaticResourcesInfoProviderExtensions
{
	public static StaticResourcesInfoProvider AddAllProjectRazorPages(this StaticResourcesInfoProvider provider, IWebHostEnvironment env)
	{
		Throw.IfNull<StaticResourcesInfoProvider>(provider, (string)null, "provider", (Func<string, Exception>)null);
		Throw.IfNull<IWebHostEnvironment>(env, (string)null, "env", (Func<string, Exception>)null);
		string sharedFolder = string.Format("{0}Shared{0}", Path.DirectorySeparatorChar);
		string pagesFolderPath = Path.Combine(env.ContentRootPath, "Pages");
		IEnumerable<string> source = from f in Directory.GetFiles(pagesFolderPath, "*.cshtml", SearchOption.AllDirectories)
			where !f.Contains($"{Path.DirectorySeparatorChar}_", StringComparison.Ordinal) && !f.Contains(sharedFolder, StringComparison.Ordinal)
			select f.Replace(pagesFolderPath, string.Empty).Replace(".cshtml", string.Empty).Replace(Path.DirectorySeparatorChar, '/');
		provider.Add(source.Select((Func<string, PageResource>)delegate(string route)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			return new PageResource(route);
		}));
		return provider;
	}

	public static StaticResourcesInfoProvider AddAllWebRootContent(this StaticResourcesInfoProvider provider, IWebHostEnvironment env)
	{
		Throw.IfNull<StaticResourcesInfoProvider>(provider, (string)null, "provider", (Func<string, Exception>)null);
		Throw.IfNull<IWebHostEnvironment>(env, (string)null, "env", (Func<string, Exception>)null);
		string webRootPath = env.WebRootPath;
		IEnumerable<string> source = from f in Directory.GetFiles(webRootPath, "*.*", SearchOption.AllDirectories)
			select f.Replace(webRootPath, string.Empty).Replace(Path.DirectorySeparatorChar, '/');
		string[] cssExts = new string[2] { ".css", ".scss" };
		string[] jsExts = new string[2] { ".js", ".json" };
		StringComparer comparer = StringComparer.OrdinalIgnoreCase;
		provider.Add(source.Where((string r) => cssExts.Contains<string>(Path.GetExtension(r), comparer)).Select((Func<string, CssResource>)delegate(string r)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			return new CssResource(r);
		}));
		provider.Add(source.Where((string r) => jsExts.Contains<string>(Path.GetExtension(r), comparer)).Select((Func<string, JsResource>)delegate(string r)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			return new JsResource(r);
		}));
		provider.Add(source.Where((string r) => !cssExts.Contains<string>(Path.GetExtension(r), comparer) && !jsExts.Contains<string>(Path.GetExtension(r), comparer)).Select((Func<string, BinResource>)delegate(string r)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			return new BinResource(r);
		}));
		return provider;
	}

	public static IEnumerable<CssResource> GetWebRootCssResources(this IWebHostEnvironment hostEnvironment, string[]? include = null, string[]? exclude = null, bool dontOptimize = false)
	{
		Throw.IfNull<IWebHostEnvironment>(hostEnvironment, (string)null, "hostEnvironment", (Func<string, Exception>)null);
		string[] resourceRoutes = GetResourceRoutes(hostEnvironment.WebRootPath, include ?? new string[1] { "**/*.css" }, exclude ?? Array.Empty<string>());
		if (resourceRoutes.Length == 0)
		{
			return Array.Empty<CssResource>();
		}
		return ((IEnumerable<string>)resourceRoutes).Select((Func<string, CssResource>)delegate(string x)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			CssResource val = new CssResource(x);
			((ResourceInfoBase)val).set_OptimizationType((OptimizationType)(dontOptimize ? 1 : 0));
			return val;
		});
	}

	public static IEnumerable<JsResource> GetWebRootJsResources(this IWebHostEnvironment hostEnvironment, string[]? include = null, string[]? exclude = null, bool dontOptimize = false)
	{
		Throw.IfNull<IWebHostEnvironment>(hostEnvironment, (string)null, "hostEnvironment", (Func<string, Exception>)null);
		string[] resourceRoutes = GetResourceRoutes(hostEnvironment.WebRootPath, include ?? new string[1] { "**/*.js" }, exclude ?? Array.Empty<string>());
		if (resourceRoutes.Length == 0)
		{
			return Array.Empty<JsResource>();
		}
		return ((IEnumerable<string>)resourceRoutes).Select((Func<string, JsResource>)delegate(string x)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			JsResource val = new JsResource(x);
			((ResourceInfoBase)val).set_OptimizationType((OptimizationType)(dontOptimize ? 1 : 0));
			return val;
		});
	}

	public static IEnumerable<BinResource> GetWebRootBinResources(this IWebHostEnvironment hostEnvironment, string[] include, string[]? exclude = null, bool dontOptimize = false)
	{
		Throw.IfNull<IWebHostEnvironment>(hostEnvironment, (string)null, "hostEnvironment", (Func<string, Exception>)null);
		Throw.InvalidOpWhen((Func<bool>)(() => include.Length == 0), "Inclusion filter is empty when adding binary static resources.");
		string[] resourceRoutes = GetResourceRoutes(hostEnvironment.WebRootPath, include, exclude ?? Array.Empty<string>());
		if (resourceRoutes.Length == 0)
		{
			return Array.Empty<BinResource>();
		}
		return ((IEnumerable<string>)resourceRoutes).Select((Func<string, BinResource>)delegate(string x)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			BinResource val = new BinResource(x);
			((ResourceInfoBase)val).set_OptimizationType((OptimizationType)(dontOptimize ? 1 : 0));
			return val;
		});
	}

	public static string[] GetResourceRoutes(string rootPath, string[] includeFilter, string[] excludeFilter)
	{
		Throw.IfNullOrWhitespace(rootPath, (string)null, "rootPath", (Func<string, Exception>)null);
		Matcher matcher = new Matcher();
		matcher.AddIncludePatterns(includeFilter);
		matcher.AddExcludePatterns(excludeFilter);
		PatternMatchingResult patternMatchingResult = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(rootPath)));
		if (!patternMatchingResult.HasMatches)
		{
			return Array.Empty<string>();
		}
		return (from f in patternMatchingResult.Files
			select f.Path.Replace(Path.DirectorySeparatorChar, '/') into f
			select f.Replace(rootPath, string.Empty)).ToArray();
	}
}
