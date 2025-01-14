﻿using commonItems;
using ImperatorToCK3.Mappers.Region;
using System.Collections.Generic;

namespace ImperatorToCK3.Mappers.Culture;

public class CultureMapper {
	public CultureMapper() {
		Logger.Info("Parsing culture mappings...");
		var parser = new Parser();
		RegisterKeys(parser);
		parser.ParseFile("configurables/culture_map.txt");
		Logger.Info($"Loaded {cultureMappingRules.Count} cultural links.");
	}
	public CultureMapper(BufferedReader reader) {
		var parser = new Parser();
		RegisterKeys(parser);
		parser.ParseStream(reader);
	}

	public void LoadRegionMappers(ImperatorRegionMapper imperatorRegionMapper, CK3RegionMapper ck3RegionMapper) {
		foreach (var mapping in cultureMappingRules) {
			mapping.ImperatorRegionMapper = imperatorRegionMapper;
			mapping.CK3RegionMapper = ck3RegionMapper;
		}
	}

	private void RegisterKeys(Parser parser) {
		parser.RegisterKeyword("link", reader => cultureMappingRules.Add(CultureMappingRule.Parse(reader)));
		parser.RegisterRegex(CommonRegexes.Catchall, ParserHelpers.IgnoreAndLogItem);
	}
	public string? Match(
		string impCulture,
		string ck3Religion,
		ulong ck3ProvinceId,
		ulong impProvinceId,
		string ck3OwnerTitle
	) {
		foreach (var cultureMappingRule in cultureMappingRules) {
			var possibleMatch = cultureMappingRule.Match(impCulture, ck3Religion, ck3ProvinceId, impProvinceId, ck3OwnerTitle);
			if (possibleMatch is not null) {
				return possibleMatch;
			}
		}
		return null;
	}

	public string? NonReligiousMatch(
		string impCulture,
		string ck3Religion,
		ulong ck3ProvinceId,
		ulong impProvinceId,
		string ck3OwnerTitle
	) {
		foreach (var cultureMappingRule in cultureMappingRules) {
			var possibleMatch = cultureMappingRule.NonReligiousMatch(impCulture, ck3Religion, ck3ProvinceId, impProvinceId, ck3OwnerTitle);
			if (possibleMatch is not null) {
				return possibleMatch;
			}
		}
		return null;
	}

	private readonly List<CultureMappingRule> cultureMappingRules = new();
}