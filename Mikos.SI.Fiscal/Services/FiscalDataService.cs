using Micros.Ops;
using Mikos.SI.Fiscal.Datastore.Dao;
using Mikos.SI.Fiscal.Datastore.Mapper;
using Mikos.SI.Fiscal.Datastore.Repository;
using Mikos.SI.Fiscal.Dtos;
using Mikos.SI.Fiscal.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikos.SI.Fiscal.Services
{
    public class FiscalDataService
    {
        public void SaveInvoiceResponseToDb(
            FiscalResponseData result,
            OpsContext opsContext,
            FiscalData fiscalData,
            bool isVoid,
            CisConfiguration cisConfiguration,
            string employeeVatId,
            string paymentMethod)
        {
            try
            {
                using (FiscalContext fiscalContext = new FiscalContext())
                {
                    FiscalInvoiceRepository fiscalInvoiceRepository = new FiscalInvoiceRepository(fiscalContext);
                    bool update = false;
                    FiscalInvoice existingFiscalInvoice = fiscalInvoiceRepository.GetByCheckGuid(opsContext.Check.Guid);

                    if (existingFiscalInvoice != null)
                    {
                        update = true;
                    }
                    FiscalInvoice fiscalInvoice = InvoiceMapper.ToFiscalInvoice(opsContext, fiscalData, result, existingFiscalInvoice, isVoid, cisConfiguration, employeeVatId, paymentMethod);

                    if (update && !isVoid)
                    {
                        fiscalInvoiceRepository.Update(fiscalInvoice);
                    }
                    else
                    {
                        fiscalInvoiceRepository.Insert(fiscalInvoice);
                    }
                    fiscalInvoiceRepository.Save();
                }
            }
            catch (Exception ex)
            {
                opsContext.ShowError("Failed saving an invoice to a local database: [" + ex.Message + "]");
            }
        }

        public void SaveBuyerInfoToDb(OpsContext opsContext, BuyerInfoInput buyerInfoInput)
        {
            try
            {
                using (FiscalContext fiscalContext = new FiscalContext())
                {
                    BuyerInfoRepository buyerInfoRepository = new BuyerInfoRepository(fiscalContext);
                    bool update = false;

                    BuyerInfo existingBuyerInfo = buyerInfoRepository.GetByVatId(buyerInfoInput.vatId);

                    if (existingBuyerInfo != null)
                    {
                        update = true;
                    }

                    BuyerInfo buyerInfo = BuyerInfoMapper.ToBuyerInfo(buyerInfoInput, existingBuyerInfo);

                    if (update)
                    {
                        buyerInfoRepository.Update(buyerInfo);
                    }
                    else
                    {
                        buyerInfoRepository.Insert(buyerInfo);
                    }

                    buyerInfoRepository.Save();
                }
            }
            catch (Exception ex)
            {
                opsContext.ShowError("Failed to save the buyer info to local db : [" + ex.Message + "]");
            }
        }

        public BuyerInfo retrieveBuyerInfoById(OpsContext opsContext, long id)
        {
            try
            {
                using (FiscalContext fiscalContext = new FiscalContext())
                {
                    BuyerInfoRepository buyerInfoRepository = new BuyerInfoRepository(fiscalContext);
                    return buyerInfoRepository.GetById(id);
                }
            }
            catch (Exception ex)
            {
                opsContext.ShowError("An error occured while trying to retreieve the buyer info from local db: [" + ex.Message + "]");
                return null;
            }
        }

        public FiscalInvoice retrieveFiscalInvoiceByChkGUID(OpsContext opsContext, string guid)
        {
            try
            {
                using (FiscalContext fiscalContext = new FiscalContext())
                {
                    FiscalInvoiceRepository fiscalInvoiceRepository = new FiscalInvoiceRepository(fiscalContext);
                    return fiscalInvoiceRepository.GetByCheckGuid(guid);
                }
            }
            catch (Exception ex)
            {
                opsContext.ShowError("An error occured while trying to retreieve the fiscal invoice from local db: [" + ex.Message + "]");
                return null;
            }
        }
    }
}
