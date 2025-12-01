using Micros.Ops;
using Mikos.SI.Fiscal.Datastore.Dao;
using Mikos.SI.Fiscal.Datastore.Repository;
using Mikos.SI.Fiscal.Dtos;
using Mikos.SI.Fiscal.Model;
using Mikos.SI.Fiscal.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Micros.Ops.OpsAskYesNoCancelRequest;
using static System.Net.Mime.MediaTypeNames;

namespace Mikos.SI.Fiscal.Datastore.Mapper
{
    public class InvoiceMapper
    {
        public static FiscalInvoice ToFiscalInvoice(
            OpsContext opsContext,
            FiscalData fiscalData,
            FiscalResponseData result,
            FiscalInvoice fiscalInvoice,
            bool isVoid,
            CisConfiguration cisConfiguration,
            string employeeVatId,
            string paymentMethod)
        {
            var nonTaxableAmount = isVoid ? decimal.Negate(Mikos.SI.Fiscal.TaxesByTaxRates.NonTaxable) : Mikos.SI.Fiscal.TaxesByTaxRates.NonTaxable;
            if (fiscalInvoice == null || isVoid)
            {
                FiscalInvoice toSave = new FiscalInvoice()
                {
                    TaxNumber = cisConfiguration.PropertyTaxNumber,
                    EmployeeVatId = employeeVatId,
                    SeqNum = !string.IsNullOrEmpty(fiscalData.SpecialId) ? long.Parse(fiscalData.SpecialId.Split('/').FirstOrDefault()) : 0,
                    CheckNumber = opsContext.CheckNumber,
                    SpecialId = !string.IsNullOrEmpty(fiscalData.SpecialId) ? fiscalData.SpecialId.Replace("/", "-") : null,
                    ChkClosedDateTime = DateTime.Now,
                    RvcNumber = opsContext.RvcNumber.ToString(),
                    WorkstationNumber = opsContext.WorkstationNumber.ToString(),
                    ZOI = fiscalData.Doc1,
                    EOR = fiscalData.Doc2,
                    VerifyInvoiceCode = fiscalData.QrCode,
                    QRImage = string.IsNullOrEmpty(fiscalData.QrCode) ? null : ReceiptPrintUtil.GenerateQrCode(fiscalData.QrCode),
                    Queued = string.IsNullOrEmpty(fiscalData.Doc2),
                    Error = string.IsNullOrEmpty(fiscalData.Doc1),
                    SyncDateTime = DateTime.Now,
                    UpdateDateTime = DateTime.Now,
                    ResponseMessage = string.IsNullOrEmpty(fiscalData.Doc2) ? null : result.StatusMessages.Messages[0].Description,
                    PaymentSubtotal = isVoid ? decimal.Negate(opsContext.Check.SubTotal) : opsContext.Check.SubTotal,
                    PaymentTotal = isVoid ? decimal.Negate(opsContext.Check.Payment) : opsContext.Check.Payment,
                    TaxExemptTtl = isVoid ? decimal.Negate(nonTaxableAmount) : nonTaxableAmount,
                    ChkGUID = opsContext.Check.Guid,
                    PaymentMethod = paymentMethod
                };

                if (result != null)
                {
                    toSave.ErrorDesc = string.IsNullOrEmpty(fiscalData.Doc2) ? result.StatusMessages.Messages[0].Description : null;
                    toSave.ResponseMessage = string.IsNullOrEmpty(fiscalData.Doc2) ? null : result.StatusMessages.Messages[0].Description;
                }
                else
                {
                    toSave.ErrorDesc = "Zahtevek ni bil poslan davčni službi.";
                    toSave.ResponseMessage = "Brez odgovora.";
                }

                if (Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Vat != 0.0m)
                {
                    toSave.Tax01Name = Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Name;
                    toSave.Tax01Vat = isVoid ? decimal.Negate(Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Vat) : Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Vat;
                    toSave.Tax01Net = isVoid ? decimal.Negate(Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Net) : Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Net;
                }
                if (Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Vat != 0.0m)
                {
                    toSave.Tax02Name = Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Name;
                    toSave.Tax02Vat = isVoid ? decimal.Negate(Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Vat) : Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Vat;
                    toSave.Tax02Net = isVoid ? decimal.Negate(Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Net) : Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Net;
                }
                if (Mikos.SI.Fiscal.TaxesByTaxRates.Tax3Vat != 0.0m)
                {
                    toSave.Tax03Name = Mikos.SI.Fiscal.TaxesByTaxRates.Tax3Name;
                    toSave.Tax03Vat = isVoid ? decimal.Negate(Mikos.SI.Fiscal.TaxesByTaxRates.Tax3Vat) : Mikos.SI.Fiscal.TaxesByTaxRates.Tax3Vat;
                    toSave.Tax03Net = isVoid ? decimal.Negate(Mikos.SI.Fiscal.TaxesByTaxRates.Tax3Net) : Mikos.SI.Fiscal.TaxesByTaxRates.Tax3Net;
                }
                if (Mikos.SI.Fiscal.TaxesByTaxRates.Tax4Vat != 0.0m)
                {
                    toSave.Tax04Name = Mikos.SI.Fiscal.TaxesByTaxRates.Tax4Name;
                    toSave.Tax04Vat = isVoid ? decimal.Negate(Mikos.SI.Fiscal.TaxesByTaxRates.Tax4Vat) : Mikos.SI.Fiscal.TaxesByTaxRates.Tax4Vat;
                    toSave.Tax04Net = isVoid ? decimal.Negate(Mikos.SI.Fiscal.TaxesByTaxRates.Tax4Net) : Mikos.SI.Fiscal.TaxesByTaxRates.Tax4Net;
                }

                return toSave;
            } 
            else
            {
                fiscalInvoice.SyncDateTime = DateTime.Now;
                fiscalInvoice.SpecialId = !string.IsNullOrEmpty(fiscalData.SpecialId) ? fiscalData.SpecialId.Replace("/", "-") : null;
                fiscalInvoice.ZOI = fiscalData.Doc1;
                fiscalInvoice.EOR = fiscalData.Doc2;
                fiscalInvoice.VerifyInvoiceCode = fiscalData.QrCode;
                fiscalInvoice.QRImage = string.IsNullOrEmpty(fiscalData.QrCode) ? null : ReceiptPrintUtil.GenerateQrCode(fiscalData.QrCode);
                fiscalInvoice.UpdateDateTime = DateTime.Now;
                fiscalInvoice.Queued = string.IsNullOrEmpty(fiscalData.Doc2);
                fiscalInvoice.Error = string.IsNullOrEmpty(fiscalData.Doc1);
                fiscalInvoice.ResponseMessage = string.IsNullOrEmpty(fiscalData.Doc2) ? null : result.StatusMessages.Messages[0].Description;

                if (result != null)
                {
                    fiscalInvoice.ErrorDesc = string.IsNullOrEmpty(fiscalData.Doc2) ? result.StatusMessages.Messages[0].Description : null;
                    fiscalInvoice.ResponseMessage = string.IsNullOrEmpty(fiscalData.Doc2) ? null : result.StatusMessages.Messages[0].Description;
                }
                else
                {
                    fiscalInvoice.ErrorDesc = "Zahtevek ni bil poslan davčni službi.";
                    fiscalInvoice.ResponseMessage = "Brez odgovora.";
                }

                return fiscalInvoice;
            }
        }

        private static string formatErrorDescription(string errorDescription)
        {
            if (!string.IsNullOrEmpty(errorDescription))
            {
                if (errorDescription.Length > 100)
                return errorDescription.Substring(0, 100);
            }
            return errorDescription;
        }
    }
}
