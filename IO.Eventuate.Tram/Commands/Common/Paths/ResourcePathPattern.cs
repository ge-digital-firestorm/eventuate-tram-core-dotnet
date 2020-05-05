using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace IO.Eventuate.Tram.Commands.Common.Paths
{
	public class ResourcePathPattern
	{
		private readonly string[] _splits;

		public ResourcePathPattern(string pathPattern)
		{
			Debug.Assert(pathPattern.StartsWith("/"), "Should start with / " + pathPattern);
			_splits = SplitPath(pathPattern);
		}

		private static string[] SplitPath(string path)
		{
			return path.Split("/");
		}

		public static ResourcePathPattern Parse(string pathPattern)
		{
			return new ResourcePathPattern(pathPattern);
		}

		public int Length => _splits.Length;

		public bool IsSatisfiedBy(ResourcePath mr)
		{
			if (_splits.Length != mr.Splits.Length)
			{
				return false;
			}

			for (int i = 0; i < mr.Splits.Length; i++)
			{
				if (!PathSegmentMatches(_splits[i], mr.Splits[i]))
				{
					return false;
				}
			}

			return true;
		}

		private bool PathSegmentMatches(string patternSegment, string pathSegment)
		{
			return IsPlaceholder(patternSegment) || patternSegment.Equals(pathSegment);
		}

		private static bool IsPlaceholder(string patternSegment)
		{
			return patternSegment.StartsWith("{");
		}

		public IDictionary<string, string> GetPathVariableValues(ResourcePath mr) {
			IDictionary<string, string> result = new Dictionary<string, string>();
			for (int i = 0 ; i < mr.Splits.Length ; i++)
			{
				string name = _splits[i];
				if (IsPlaceholder(name))
				{
					 result[PlaceholderName(name)] = mr.Splits[i];
				}
			}
			return result;
		}

		private static string PlaceholderName(string name)
		{
			return name.Substring(1, name.Length - 1);
		}

		// TODO
		// public ResourcePath ReplacePlaceholders(PlaceholderValueProvider placeholderValueProvider)
		// {
		// 	return new ResourcePath(Arrays.stream(_splits).map(s -> isPlaceholder(s) ? placeholderValueProvider.get(placeholderName(s)).orElseGet(() -> {
		// 		throw new RuntimeException("Placeholder not found: " + placeholderName(s) + " in " + s + ", params=" + placeholderValueProvider.getParams());
		// 	}) : s).collect(toList()).toArray(new string[_splits.length]));
		// }
		//
		// public ResourcePath ReplacePlaceholders(object[] pathParams)
		// {
		// 	AtomicInteger idx = new AtomicInteger(0);
		// 	return new ResourcePath(Arrays.stream(_splits).map(s -> isPlaceholder(s) ? getPlaceholderValue(pathParams, idx.getAndIncrement()).orElseGet(() -> {
		// 		throw new RuntimeException("Placeholder not found: " + placeholderName(s) + " in " + s + ", params=" + Arrays.asList(pathParams));
		// 	}) : s).collect(toList()).toArray(new String[_splits.length]));
		// }

		private string GetPlaceholderValue(object[] pathParams, int idx)
		{
			return idx < pathParams.Length ? pathParams[idx].ToString() : null;
		}
	}
}