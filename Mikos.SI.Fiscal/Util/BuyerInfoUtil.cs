using Micros.Ops;
using Mikos.SI.Fiscal.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikos.SI.Fiscal.Util
{
    public class BuyerInfoUtil
    {
        public static bool ValidateBuyerData(BuyerInfoInput BuyerInfoInput, OpsContext opsContext)
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrEmpty(BuyerInfoInput.vatId))
            {
                errors.Add("Buyer info is missing the VAT ID!");
            }

            if (string.IsNullOrEmpty(BuyerInfoInput.address))
            {
                errors.Add("Buyer info is missing the address!");
            }

            if (string.IsNullOrEmpty(BuyerInfoInput.town))
            {
                errors.Add("Buyer info is missing the town/city!");
            }

            if (string.IsNullOrEmpty(BuyerInfoInput.zip))
            {
                errors.Add("Buyer info is missing the postal code!");
            }

            if (string.IsNullOrEmpty(BuyerInfoInput.country))
            {
                errors.Add("Buyer info is missing the country code!");
            }
            else if (!IsValidISO2Code(BuyerInfoInput.country) && !IsValidISO3Code(BuyerInfoInput.country))
            {
                errors.Add("Provided country code: " + BuyerInfoInput.country + " is not a valid ISO country code!");
            }
            else if (BuyerInfoInput.country.ToLower().Equals("al") || BuyerInfoInput.country.ToLower().Equals("alb"))
            {
                errors.Add("To add buyer information for a domestic buyer use the option to add a buyer with NUIS!");
            }

            if (errors.Any())
            {
                opsContext.ShowError(BuyerInfoValidationErrorMessage(errors));
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool IsValidISO2Code(string isoCode)
        {
            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            // Extract the unique ISO 2 country codes
            var iso2CountryCodes = cultures.Select(culture => new RegionInfo(culture.Name).TwoLetterISORegionName).Distinct();

            // Check if the provided code is in the list of valid ISO2 country codes
            return iso2CountryCodes.Contains(isoCode.ToUpper());
        }

        private static bool IsValidISO3Code(string isoCode)
        {
            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            // Extract the unique ISO 3 country codes
            var iso3CountryCodes = cultures.Select(culture => new RegionInfo(culture.Name).ThreeLetterISORegionName).Distinct();

            // Check if the provided code is in the list of valid ISO3 country codes
            return iso3CountryCodes.Contains(isoCode.ToUpper());
        }

        private static string BuyerInfoValidationErrorMessage(List<string> errors)
        {
            StringBuilder sb = new StringBuilder("Buyer info is containing following errors:");
            foreach (string error in errors)
            {
                sb.Append($"\n{error}");
            }
            return sb.ToString();
        }
    }
}
