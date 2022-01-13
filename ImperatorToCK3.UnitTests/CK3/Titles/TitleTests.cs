﻿using commonItems;
using ImperatorToCK3.CK3.Characters;
using ImperatorToCK3.CK3.Titles;
using ImperatorToCK3.Imperator.Countries;
using ImperatorToCK3.Mappers.CoA;
using ImperatorToCK3.Mappers.Culture;
using ImperatorToCK3.Mappers.Government;
using ImperatorToCK3.Mappers.Localization;
using ImperatorToCK3.Mappers.Nickname;
using ImperatorToCK3.Mappers.Province;
using ImperatorToCK3.Mappers.Religion;
using ImperatorToCK3.Mappers.SuccessionLaw;
using ImperatorToCK3.Mappers.TagTitle;
using System.Linq;
using Xunit;

namespace ImperatorToCK3.UnitTests.CK3.Titles {
	[Collection("Sequential")]
	[CollectionDefinition("Sequential", DisableParallelization = true)]
	public class TitleTests {
		private class TitleBuilder {
			private Country country = new(0);
			private CountryCollection imperatorCountries = new();
			private LocalizationMapper localizationMapper = new();
			private readonly Title.LandedTitles landedTitles = new();
			private ProvinceMapper provinceMapper = new();
			private CoaMapper coaMapper = new("TestFiles/CoatsOfArms.txt");
			private TagTitleMapper tagTitleMapper = new("TestFiles/configurables/title_map.txt", "TestFiles/configurables/governorMappings.txt");
			private GovernmentMapper governmentMapper = new();
			private SuccessionLawMapper successionLawMapper = new("TestFiles/configurables/succession_law_map.txt");
			private DefiniteFormMapper definiteFormMapper = new("TestFiles/configurables/definite_form_names.txt");

			private readonly ReligionMapper religionMapper = new();
			private readonly CultureMapper cultureMapper = new();
			private readonly NicknameMapper nicknameMapper = new("TestFiles/configurables/nickname_map.txt");
			private readonly CharacterCollection characters = new();

			public Title BuildFromTag() {
				return landedTitles.Add(
					country,
					imperatorCountries,
					localizationMapper,
					provinceMapper,
					coaMapper,
					tagTitleMapper,
					governmentMapper,
					successionLawMapper,
					definiteFormMapper,
					religionMapper,
					cultureMapper,
					nicknameMapper,
					characters
				);
			}
			public TitleBuilder WithCountry(Country country) {
				this.country = country;
				return this;
			}
			public TitleBuilder WithImperatorCountries(CountryCollection imperatorCountries) {
				this.imperatorCountries = imperatorCountries;
				return this;
			}
			public TitleBuilder WithLocalizationMapper(LocalizationMapper localizationMapper) {
				this.localizationMapper = localizationMapper;
				return this;
			}
			public TitleBuilder WithProvinceMapper(ProvinceMapper provinceMapper) {
				this.provinceMapper = provinceMapper;
				return this;
			}
			public TitleBuilder WithCoatsOfArmsMapper(CoaMapper coaMapper) {
				this.coaMapper = coaMapper;
				return this;
			}
			public TitleBuilder WithTagTitleMapper(TagTitleMapper tagTitleMapper) {
				this.tagTitleMapper = tagTitleMapper;
				return this;
			}
			public TitleBuilder WithGovernmentMapper(GovernmentMapper governmentMapper) {
				this.governmentMapper = governmentMapper;
				return this;
			}
			public TitleBuilder WithSuccessionLawMapper(SuccessionLawMapper successionLawMapper) {
				this.successionLawMapper = successionLawMapper;
				return this;
			}
			public TitleBuilder WithDefiniteFormMapper(DefiniteFormMapper definiteFormMapper) {
				this.definiteFormMapper = definiteFormMapper;
				return this;
			}
		}

		private readonly TitleBuilder builder = new();

		[Fact]
		public void TitlePrimitivesDefaultToBlank() {
			var reader = new BufferedReader(string.Empty);
			var landedTitles = new Title.LandedTitles();
			var title = landedTitles.Add("k_testtitle");
			title.LoadTitles(reader);

			Assert.False(title.HasDefiniteForm);
			Assert.False(title.Landless);
			Assert.Null(title.Color1);
			Assert.Null(title.Color2);
			Assert.Null(title.CapitalCounty);
			Assert.Null(title.Province);
			Assert.False(title.PlayerCountry);
		}

		[Fact]
		public void TitlePrimitivesCanBeLoaded() {
			var reader = new BufferedReader(
				"definite_form = yes\n" +
				"landless = yes\n" +
				"color = { 23 23 23 }\n" +
				"capital = c_roma\n" +
				"province = 345\n" +
				"c_roma = {}"
			);

			var titles = new Title.LandedTitles();
			var title = titles.Add("k_testtitle");
			title.LoadTitles(reader);

			Assert.True(title.HasDefiniteForm);
			Assert.True(title.Landless);
			Assert.NotNull(title.Color1);
			Assert.Equal("rgb { 23 23 23 }", title.Color1.OutputRgb());
			Assert.Equal((ulong)345, title.Province);

			Assert.NotNull(title.CapitalCounty);
			Assert.Equal("c_roma", title.CapitalCountyId);
		}

		[Fact]
		public void LocalizationCanBeSet() {
			var titles = new Title.LandedTitles();
			var title = titles.Add("k_testtitle");
			var locBlock = new LocBlock {
				english = "engloc",
				french = "frloc",
				german = "germloc",
				russian = "rusloc",
				spanish = "spaloc"
			};

			title.SetNameLoc(locBlock);
			Assert.Equal(1, title.Localizations.Count);
		}

		[Fact]
		public void MembersDefaultToBlank() {
			var titles = new Title.LandedTitles();
			var title = titles.Add("k_testtitle");

			Assert.Empty(title.Localizations);
			Assert.Null(title.CoA);
			Assert.Null(title.CapitalCounty);
			Assert.Null(title.ImperatorCountry);
		}

		[Fact]
		public void HolderIdDefaultsTo0String() {
			var titles = new Title.LandedTitles();
			var title = titles.Add("k_testtitle");

			Assert.Equal("0", title.GetHolderId(new Date(867, 1, 1)));
		}

		[Fact]
		public void CapitalBaronyDefaultsToNull() {
			var titles = new Title.LandedTitles();
			var title = titles.Add("k_testtitle");

			Assert.Null(title.CapitalBaronyProvince);
		}

		[Fact]
		public void HistoryCanBeAdded() {
			var titlesHistory = new TitlesHistory("TestFiles/title_history", new Date(867, 1, 1));
			var history = titlesHistory.PopTitleHistory("k_greece");
			var titles = new Title.LandedTitles();
			var title = titles.Add("k_testtitle");
			title.AddHistory(history);

			Assert.Equal("420", title.GetHolderId(new Date(867, 1, 1)));
			Assert.Equal(20, title.DevelopmentLevel);
		}

		[Fact]
		public void DevelopmentLevelCanBeInherited() {
			var titles = new Title.LandedTitles();
			var vassal = titles.Add("c_vassal");
			vassal.DeJureLiege = titles.Add("d_liege");
			vassal.DeJureLiege.DevelopmentLevel = 8;

			Assert.Equal(8, vassal.OwnOrInheritedDevelopmentLevel);
		}

		[Fact]
		public void InheritedDevelopmentCanBeNull() {
			var titles = new Title.LandedTitles();
			var vassal = titles.Add("c_vassal");
			vassal.DeJureLiege = titles.Add("d_liege");

			Assert.Null(vassal.OwnOrInheritedDevelopmentLevel);
		}

		[Fact]
		public void DeJureVassalsAndBelowAreCorrectlyReturned() {
			var titles = new Title.LandedTitles();
			var empire = titles.Add("e_empire");

			var kingdom1 = titles.Add("k_kingdom1");
			kingdom1.DeJureLiege = empire;

			var kingdom2 = titles.Add("k_kingdom2");
			kingdom2.DeJureLiege = empire;

			var duchy = titles.Add("d_duchy");
			duchy.DeJureLiege = kingdom2;

			var county = titles.Add("c_county");
			county.DeJureLiege = duchy;

			var vassals = empire.GetDeJureVassalsAndBelow();
			var sortedVassals = from entry in vassals orderby entry.Key ascending select entry;
			Assert.Collection(sortedVassals,
				item1 => Assert.Equal("c_county", item1.Value.Id),
				item2 => Assert.Equal("d_duchy", item2.Value.Id),
				item3 => Assert.Equal("k_kingdom1", item3.Value.Id),
				item4 => Assert.Equal("k_kingdom2", item4.Value.Id)
			);
		}
		[Fact]
		public void DeJureVassalsAndBelowCanBeFilteredByRank() {
			var titles = new Title.LandedTitles();
			var empire = titles.Add("e_empire");

			var kingdom1 = titles.Add("k_kingdom1");
			kingdom1.DeJureLiege = empire;

			var kingdom2 = titles.Add("k_kingdom2");
			kingdom2.DeJureLiege = empire;

			var duchy = titles.Add("d_duchy");
			duchy.DeJureLiege = kingdom2;

			var county = titles.Add("c_county");
			county.DeJureLiege = duchy;

			var vassals = empire.GetDeJureVassalsAndBelow(rankFilter: "ck");
			var sortedVassals = from entry in vassals orderby entry.Key ascending select entry;
			Assert.Collection(sortedVassals,
				// only counties and kingdoms go through the filter
				item1 => Assert.Equal("c_county", item1.Value.Id),
				item2 => Assert.Equal("k_kingdom1", item2.Value.Id),
				item3 => Assert.Equal("k_kingdom2", item3.Value.Id)
			);
		}

		[Fact]
		public void DeFactoVassalsAndBelowAreCorrectlyReturned() {
			var titles = new Title.LandedTitles();
			var empire = titles.Add("e_empire");

			var kingdom1 = titles.Add("k_kingdom1");
			kingdom1.DeFactoLiege = empire;

			var kingdom2 = titles.Add("k_kingdom2");
			kingdom2.DeFactoLiege = empire;

			var duchy = titles.Add("d_duchy");
			duchy.DeFactoLiege = kingdom2;

			var county = titles.Add("c_county");
			county.DeFactoLiege = duchy;

			var vassals = empire.GetDeFactoVassalsAndBelow();
			var sortedVassals = from entry in vassals orderby entry.Key select entry;
			Assert.Collection(sortedVassals,
				item1 => Assert.Equal("c_county", item1.Value.Id),
				item2 => Assert.Equal("d_duchy", item2.Value.Id),
				item3 => Assert.Equal("k_kingdom1", item3.Value.Id),
				item4 => Assert.Equal("k_kingdom2", item4.Value.Id)
			);
		}
		[Fact]
		public void DeFactoVassalsAndBelowCanBeFilteredByRank() {
			var titles = new Title.LandedTitles();
			var empire = titles.Add("e_empire");

			var kingdom1 = titles.Add("k_kingdom1");
			kingdom1.DeFactoLiege = empire;

			var kingdom2 = titles.Add("k_kingdom2");
			kingdom2.DeFactoLiege = empire;

			var duchy = titles.Add("d_duchy");
			duchy.DeFactoLiege = kingdom2;

			var county = titles.Add("c_county");
			county.DeFactoLiege = duchy;

			var vassals = empire.GetDeFactoVassalsAndBelow(rankFilter: "ck");
			var sortedVassals = from entry in vassals orderby entry.Key ascending select entry;
			Assert.Collection(sortedVassals,
				// only counties and kingdoms go through the filter
				item1 => Assert.Equal("c_county", item1.Value.Id),
				item2 => Assert.Equal("k_kingdom1", item2.Value.Id),
				item3 => Assert.Equal("k_kingdom2", item3.Value.Id)
			);
		}

		[Fact]
		public void DeFactoLiegeChangeRemovesTitleFromVassalsOfPreviousLiege() {
			var titles = new Title.LandedTitles();
			var vassal = titles.Add("d_vassal");
			var oldLiege = titles.Add("k_old_liege");
			vassal.DeFactoLiege = oldLiege;
			Assert.Equal("k_old_liege", vassal.DeFactoLiege.Id);
			Assert.True(oldLiege.DeFactoVassals.ContainsKey("d_vassal"));

			var newLiege = titles.Add("k_new_liege");
			vassal.DeFactoLiege = newLiege;
			Assert.Equal("k_new_liege", vassal.DeFactoLiege.Id);
			Assert.False(oldLiege.DeFactoVassals.ContainsKey("d_vassal"));
			Assert.True(newLiege.DeFactoVassals.ContainsKey("d_vassal"));
		}

		[Fact]
		public void DeJureLiegeChangeRemovesTitleFromVassalsOfPreviousLiege() {
			var titles = new Title.LandedTitles();
			var vassal = titles.Add("d_vassal");
			var oldLiege = titles.Add("k_old_liege");
			vassal.DeJureLiege = oldLiege;
			Assert.Equal("k_old_liege", vassal.DeJureLiege.Id);
			Assert.True(oldLiege.DeJureVassals.ContainsKey("d_vassal"));

			var newLiege = titles.Add("k_new_liege");
			vassal.DeJureLiege = newLiege;
			Assert.Equal("k_new_liege", vassal.DeJureLiege.Id);
			Assert.False(oldLiege.DeJureVassals.ContainsKey("d_vassal"));
			Assert.True(newLiege.DeJureVassals.ContainsKey("d_vassal"));
		}

		[Fact]
		public void DuchyContainsProvinceReturnsFalseWhenTitleIsNotDuchy() {
			var titles = new Title.LandedTitles();
			var countyReader = new BufferedReader("b_barony = { province=1}");
			var county = titles.Add("c_county");
			county.LoadTitles(countyReader, null);
			Assert.False(county.DuchyContainsProvince(1));
		}
		[Fact]
		public void DuchyContainsProvinceCorrectlyReturnsTrue() {
			var titles = new Title.LandedTitles();
			var countyReader = new BufferedReader("b_barony = { province=1}");
			var county = titles.Add("c_county");
			county.LoadTitles(countyReader, null);
			var duchy = titles.Add("d_duchy");
			county.DeJureLiege = duchy;
			Assert.True(duchy.DuchyContainsProvince(1));
		}
		[Fact]
		public void DuchyContainsProvinceCorrectlyReturnsFalse() {
			var titles = new Title.LandedTitles();
			var countyReader = new BufferedReader("b_barony = { province=1}");
			var county = titles.Add("c_county");
			county.LoadTitles(countyReader, null);
			var duchy = titles.Add("d_duchy");
			county.DeJureLiege = duchy;
			Assert.False(duchy.DuchyContainsProvince(2)); // wrong id
		}

		[Fact]
		public void KingdomContainsProvinceWhenTitleIsNotKingdom() {
			var titles = new Title.LandedTitles();
			var countyReader = new BufferedReader("b_barony = { province=1}");
			var county = titles.Add("c_county");
			county.LoadTitles(countyReader, null);
			Assert.False(county.KingdomContainsProvince(1));
		}
		[Fact]
		public void KingdomContainsProvinceCorrectlyReturnsTrue() {
			var titles = new Title.LandedTitles();
			var countyReader = new BufferedReader("b_barony = { province=1}");
			var county = titles.Add("c_county");
			county.LoadTitles(countyReader, null);
			var duchy = titles.Add("d_duchy");
			county.DeJureLiege = duchy;
			var kingdom = titles.Add("k_kingdom");
			duchy.DeJureLiege = kingdom;
			Assert.True(kingdom.KingdomContainsProvince(1));
		}
		[Fact]
		public void KingdomContainsProvinceCorrectlyReturnsFalse() {
			var titles = new Title.LandedTitles();
			var countyReader = new BufferedReader("b_barony = { province=1}");
			var county = titles.Add("c_county");
			county.LoadTitles(countyReader, null);
			var duchy = titles.Add("d_duchy");
			county.DeJureLiege = duchy;
			var kingdom = titles.Add("k_kingdom");
			duchy.DeJureLiege = kingdom;
			Assert.False(kingdom.KingdomContainsProvince(2));
		}

		[Fact]
		public void TitleCanBeConstructedFromCountry() {
			var countryReader = new BufferedReader("tag = HRE");
			var country = Country.Parse(countryReader, 666);

			var title = builder
				.WithCountry(country)
				.BuildFromTag();
			Assert.Equal("d_IMPTOCK3_HRE", title.Id);
		}
	}
}
