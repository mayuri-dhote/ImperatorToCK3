#include "Provinces.h"
#include "Province.h"
#include "Pops.h"
#include "Log.h"
#include "ParserHelpers.h"

ImperatorWorld::Provinces::Provinces(std::istream& theStream)
{
	registerKeys();
	parseStream(theStream);
	clearRegisteredKeywords();
}

void ImperatorWorld::Provinces::registerKeys()
{
	registerRegex(R"(\d+)", [this](const std::string& provID, std::istream& theStream) {
		auto newProvince = std::make_shared<Province>(theStream, std::stoi(provID));
		provinces.insert(std::pair(newProvince->getID(), newProvince));
	});
	registerRegex(commonItems::catchallRegex, commonItems::ignoreItem);
}

void ImperatorWorld::Provinces::linkPops(const Pops& thePops)
{
	auto counter = 0;
	const auto& pops = thePops.getPops();
	for (const auto& [provinceID, province] : provinces)
	{
		if (!province->getPops().empty())
		{
			std::map<int, std::shared_ptr<Pop>> newPops;
			for (const auto& [popID, pop] : province->getPops())
			{
				const auto& popItr = pops.find(popID);
				if (popItr != pops.end())
				{
					newPops.insert(std::pair(popItr->first, popItr->second));
					counter++;
				}
				else
				{
					Log(LogLevel::Warning) << "Pop ID: " << popID << " has no definition!";
				}
			}
			province->setPops(newPops);
		}
	}
	Log(LogLevel::Info) << "<> " << counter << " pops linked to provinces.";
}