using Mikos.SI.Fiscal.Datastore.Dao;
using Mikos.SI.Fiscal.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikos.SI.Fiscal.Datastore.Mapper
{
    public class BuyerInfoMapper
    {
        public static BuyerInfo ToBuyerInfo(BuyerInfoInput buyerInfoInput, BuyerInfo buyerInfo)
        {
            if (buyerInfo == null)
            {
                return new BuyerInfo()
                {
                    name = buyerInfoInput.name,
                    vatId = buyerInfoInput.vatId,
                    address = buyerInfoInput.address,
                    town = buyerInfoInput.town,
                    zip = buyerInfoInput.zip,
                    country = buyerInfoInput.country,
                    active = buyerInfoInput.active
                };
            }

            buyerInfo.name = buyerInfoInput.name;
            buyerInfo.vatId = buyerInfoInput.vatId;
            buyerInfo.address = buyerInfoInput.address;
            buyerInfo.town = buyerInfoInput.town;
            buyerInfo.zip = buyerInfoInput.zip;
            buyerInfo.country = buyerInfoInput.country;
            buyerInfo.active = buyerInfoInput.active;

            return buyerInfo;
        }
    }
}
