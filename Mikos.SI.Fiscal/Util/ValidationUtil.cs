using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikos.SI.Fiscal.Util
{
    public class ValidationUtil
    {
        public static bool ValidateTaxNumber(string taxNumber)
        {
            if (taxNumber.Length != 11)
            {
                return false;
            }
            if (!long.TryParse(taxNumber, out var _))
            {
                return false;
            }
            int num = 10;
            for (int i = 0; i < 10; i++)
            {
                num += Convert.ToInt32(taxNumber.Substring(i, 1));
                num %= 10;
                if (num == 0)
                {
                    num = 10;
                }
                num *= 2;
                num %= 11;
            }
            int num2 = 11 - num;
            if (num2 == 10)
            {
                num2 = 0;
            }
            return num2 == Convert.ToInt32(taxNumber.Substring(10, 1));
        }
    }
}
