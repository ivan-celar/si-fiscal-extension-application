using Micros.Ops;
using Micros.Ops.Extensibility;
using Micros.Ops.IntegratedCashManagement;
using Micros.PosCore.Checks;
using Micros.PosCore.Common;
using Micros.PosCore.Common.Classes;
using Micros.PosCore.Extensibility;
using Micros.PosCore.Extensibility.DataStore.DbRecords;
using Micros.PosCore.Extensibility.Ops;
using Micros.PosCore.Extensibility.Printing;
using Mikos.SI.Fiscal.Datastore.Configuration;
using Mikos.SI.Fiscal.Datastore.Dao;
using Mikos.SI.Fiscal.Datastore.Mapper;
using Mikos.SI.Fiscal.Datastore.Repository;
using Mikos.SI.Fiscal.Dtos;
using Mikos.SI.Fiscal.Model;
using Mikos.SI.Fiscal.Services;
using Mikos.SI.Fiscal.Util;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Micros.Ops.OpsAskYesNoCancelRequest;
using static Micros.PosCore.Extensibility.DataStore.DbRecords.DbKey;
using static Micros.PosCore.Extensibility.Printing.PosPrinterData;
using static Micros.PosCore.Extensibility.Printing.PosPrinterDirective;

namespace Mikos.SI.Fiscal
{
    public class Application : OpsExtensibilityApplication
    {
        private const string validateInvoiceReferenceUrl = "/invoices/reference";
        private const string invoicesUrl = "/invoices/fiscalize";

        //string dllLocationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

        public class TenderMediaTotals
        {
            public int TenderMediaObjNum { get; set; }

            public string TenderMediaType { get; set; }

            public decimal TenderMediaTotal { get; set; }
        }
        internal class PaymentMethod
        {
            public String Description { get; set; }
            public int ObjectNumber { get; set; }

            public TenderMediaType Type { get; set; }

            public decimal Total { get; set; }
        }
        internal enum TenderMediaType
        {
            NONE,
            G,
            K,
            C,
            T,
            O,
            P
        }

        internal enum RetransmitType
        {
            Undefined = -1,
            FiscalInvoice
        }

        internal class itemsPlaceholderObject
        {
            public List<Posting> postings { get; set; }
            public List<RevenueBucketInfo> revenueBucketInfos { get; set; }
            public TotalInfo totalInfo { get; set; }
            public List<TrxInfo> TrxInfo { get; set; }
        }

        public PosPrinterDirective posPrinterDirective { get; set; }
        private CashDrawerSettings cashDrawerSettings = new CashDrawerSettings()
        {
            Enabled = false
        };
        private VoidCheckItems VoidCheckItems;
        private FiscalData fiscalData;
        private FiscalDataService fiscalDataService;
        private BuyerInfo buyerInfo;
        private bool isRetransmit;
        private string paymentMethod;
        private bool cancelVoid;
        private bool fiscalize = true;
        private bool hasBuyer = false;
        private bool openCashDrawer = false;
        /// <summary>
        /// Extension application constructor
        /// </summary>
        /// <param name="context">the execution context for the application</param>
        public Application(IExecutionContext context)
            : base(context)
        {
            //Add initialization code and hook up event handlers here
            base.OpsFinalTenderEvent += Application_FinalTenderEvent;
            base.OpsVoidClosedCheckEventPreview += Application_VoidClosedCheckEventPreview;
            base.OpsCustomReceiptEvent += Application_CustomReceiptEvent;
            this.OpsVoidReasonEvent += Application_VoidReasonEvent;
            this.fiscalDataService = new FiscalDataService();
            this.cashDrawerSettings = ReadOpenCashDrawerSetting();
        }

        private EventProcessingInstruction Application_FinalTenderEvent(object sender, OpsTmedEventArgs args)
        {
            try
            {
                if (!this.OpsContext.TrainingModeEnabled)
                {
                    FiscalResponseData result = null;
                    FiscalRequestData invoice = FillFiscalRequestInitData(false);
                    fiscalData = null;
                    VoidCheckItems = null;


                    if (invoice != null)
                    {

                        result = System.Threading.Tasks.Task.Run(async () => await sendRequestToRacunko(invoice)).Result;

                    }

                    ProcessResult(result, false);
                }
            }
            catch (Exception ex)
            {
                this.OpsContext.ShowMessage(ex.Message);
                return EventProcessingInstruction.Continue;
            }
            return EventProcessingInstruction.Continue;
        }

        private EventProcessingInstruction Application_VoidClosedCheckEventPreview(object sender, OpsVoidCheckEventArgs args)
        {
            try
            {
                if (!this.OpsContext.TrainingModeEnabled)
                {
                    FiscalResponseData result = null;
                    FiscalRequestData invoice = FillFiscalRequestInitData(true);
                    fiscalData = null;


                    if (invoice != null)
                    {

                        result = System.Threading.Tasks.Task.Run(async () => await sendRequestToRacunko(invoice)).Result;

                    }

                    ProcessResult(result, true);
                }
            }
            catch (Exception ex)
            {
                this.OpsContext.ShowMessage(ex.Message);
                return EventProcessingInstruction.Continue;
            }
            return EventProcessingInstruction.Continue;
        }

        private EventProcessingInstruction Application_VoidReasonEvent(object sender, OpsVoidReasonEventArgs args)
        {
            try
            {
                if (!this.OpsContext.TrainingModeEnabled)
                {
                    if (cancelVoid)
                    {
                        TransactionCancel();
                    }
                }
                cancelVoid = false;
            }
            catch (Exception ex)
            {
                this.OpsContext.ShowMessage(ex.Message);
                return EventProcessingInstruction.Continue;
            }
            return EventProcessingInstruction.Continue;
        }

        private void ProcessResult(FiscalResponseData result, bool isVoid)
        {
            CisConfiguration cisConfiguration = ReadCisConfiguration();
            string emplyeeVatId = GetEmployeeVatId();
            string verifyInvoiceValue = "";
            string doc1Value = "";
            string doc2Value = "";
            string specialIdValue = "";
            bool subsequentTipFailed = false;
            if (result != null)
            {
                if (result.StatusMessages.Messages[0].Type.Equals("Error"))
                {
                    base.OpsContext.ShowError(ShowMessages(result.StatusMessages.Messages));
                    if (isVoid)
                    {
                        cancelVoid = true;
                    }
                }
                else
                {
                    base.OpsContext.ShowMessage(ShowMessages(result.StatusMessages.Messages));
                }

                verifyInvoiceValue = result?.FiscalOutputs?.Output?.Find(qRCodeBase => qRCodeBase.Name == "VERIFY_INVOICE")?.Value;
                doc1Value = result?.FiscalOutputs?.Output?.Find(qRCodeBase => qRCodeBase.Name == "DOCUMENT_NO_1")?.Value;
                doc2Value = result?.FiscalOutputs?.Output?.Find(qRCodeBase => qRCodeBase.Name == "DOCUMENT_NO_2")?.Value;
                specialIdValue = result?.FiscalFolioNo;
            } 
            else if (fiscalize)
            {
                base.OpsContext.ShowError("Zahteva ni bila uspešno poslana za fiskalizacijo.\n"
                    + "Preverite internetno povezavo in poskusite znova poslati zahtevo.");
                cancelVoid = true;
            }

            fiscalData = new FiscalData
            {
                QrCode = verifyInvoiceValue,
                Doc1 = doc1Value,
                Doc2 = doc2Value,
                SpecialId = specialIdValue
            };


            if (fiscalize)
            {
                fiscalDataService.SaveInvoiceResponseToDb(result, this.OpsContext, fiscalData, isVoid, cisConfiguration, emplyeeVatId, paymentMethod);
            }
              
            isRetransmit = false;
            hasBuyer = false;
            fiscalize = true;
        }

        private bool IsEligibleToBeStored(FiscalResponseData result)
        {
            return ((this.OpsContext.Check.Payment != 0.0m && !cancelVoid)) 
                && !result.StatusMessages.Messages[0].Description.Contains("Nije potrebna fiskalizacija.");
        }

        private string ShowMessages(List<Message> Messages)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Message message in Messages)
            {
                sb.AppendLine(message.Description);
            }

            return sb.ToString();
        }

        private FiscalRequestData FillFiscalRequestInitData(bool isVoid)
        {
            FiscalRequestData invoice = new FiscalRequestData();

            if (!hasBuyer)
            {
                buyerInfo = null;
            }


            CisConfiguration cisConfiguration = ReadCisConfiguration();

            if (string.IsNullOrEmpty(cisConfiguration.PropertyTaxNumber))
            {
                this.OpsContext.ShowError("Property tax number is not configured or it's in the wrong format. Please contact customer support.");
            }

            itemsPlaceholderObject itemsList = fillInvoiceItemData(isVoid);

            if (itemsList == null)
            {
                return null;
            }

            string currentDateTimeFormat = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            string employeeVatId = GetEmployeeVatId();

            //if (!ValidationUtil.ValidateTaxNumber(cisConfiguration.PropertyTaxNumber))
            //{
            //    base.OpsContext.ShowError("Invalid Property Tax Number. Please check fiscal configuration.");
            //    TransactionCancel();
            //    return null;
            //}

            if (string.IsNullOrEmpty(employeeVatId))
            {
                base.OpsContext.ShowError("Invalid Employee Tax Number. Please check the system configuration.");
                TransactionCancel();
                return null;
            }

            invoice.DocumentInfo = new DocumentInfo()
            {
                HotelCode = cisConfiguration.PropertyCode,
                BillNo = this.OpsContext.CheckNumber.ToString(),
                FolioType = "FISCAL",
                TerminalId = this.OpsContext.Check.Guid,
                ProgramName = "0",
                FiscalFolioId = "0",
                OperaFiscalBillNo = null,
                Application = "Simphony",
                PropertyTaxNumber = cisConfiguration.PropertyTaxNumber,
                BankName = "",
                BankCode = "",
                BankIdType = "",
                BankIdCode = "",
                BusinessDate = DateTime.Now.ToString("yyyy-MM-dd"),
                BusinessDateTime = currentDateTimeFormat,
                CountryCode = "SI",
                CountryName = cisConfiguration.SellerCountry,
                Command = "INVOICE",
                FiscalTimeoutPeriod = "30"
            };
            invoice.UserDefinedFields = new UserDefinedFields()
            {
                CharacterUDFs = new List<CharacterUDF>()
                {
                   new CharacterUDF()
                   {
                       UDF =  new List<UDF>()
                       {
                           new UDF()
                           {
                               Name = "FLIP_PARTNER_TAX1",
                               Value = cisConfiguration.BusinessPremiseId
                           },
                           new UDF()
                           {
                                Name = "FLIP_PARTNER_TAX2",
                               Value = OpsContext.WorkstationNumber.ToString()
                           },
                           new UDF()
                           {
                               Name = "FLIP_PARTNER_TAX_LEVEL",
                               Value = "C"
                           }
                       }//FillUDFs()
                   }
                }
            };
            invoice.FolioInfo = new FolioInfo()
            {
                FolioHeaderInfo = new FolioHeaderInfo()
                {
                    BillGenerationDate = currentDateTimeFormat,
                    FolioType = "FISCAL",
                    CreditBill = false,
                    FolioNo = (this.OpsContext.CheckNumber + (isVoid ? 1 : 0)).ToString(),
                    BillNo = this.OpsContext.CheckNumber.ToString(),
                    InvoiceCurrencyCode = "EUR",
                    InvoiceCurrencyRate = "1",
                    Window = "4",
                    CashierNumber = this.OpsContext.WorkstationNumber.ToString(),
                    FiscalFolioStatus = "OK",
                    LocalBillGenerationDate = currentDateTimeFormat,
                    CollectingAgentTaxes = null,
                    FolioTypeUniqueCode = "0",
                    AssociatedFiscalTerminalInfo = null,
                    AssociatedFolioInfo = null
                },
                Postings = itemsList.postings,
                RevenueBucketInfo = itemsList.revenueBucketInfos,
                TotalInfo = itemsList.totalInfo,
                TrxInfo = itemsList.TrxInfo
            };
            invoice.HotelInfo = new HotelInfo()
            {
                HotelCode = cisConfiguration.PropertyCode,
                HotelName = cisConfiguration.SellerName,
                LegalOwner = cisConfiguration.SellerOwner,
                Address = new Addresses()
                {
                    Address = cisConfiguration.SellerAddr,
                    City = cisConfiguration.SellerCity,
                    Country = cisConfiguration.SellerCountry

                },
                LocalCurrency = cisConfiguration.SellerCurrencyCode,
                Decimals = "2",
                TimeZoneRegion = "Europe/Ljubljana",
                PhoneNo = "+386 800 8000 80",
                Email = "info@symp.si",
                WebPage = "www.symp.si",
                ExchangeRates = { },
                PropertyDateTime = currentDateTimeFormat,
                BusinessPremiseId1 = OpsContext.RvcNumber.ToString(),
                BusinessPremiseId2 = OpsContext.WorkstationNumber.ToString()
            };
            invoice.ReservationInfo = new ReservationInfo()
            {
                ResvNameID = OpsContext.Check.Guid,
            };
            invoice.FiscalFolioUserInfo = new FiscalFolioUserInfo()
            {
                AppUser = this.OpsContext.TransEmployeeFullName,
                AppUserId = this.OpsContext.TransEmployeeID.ToString(),
                EmployeeNumber = employeeVatId,
                CashierId = null
            };
            invoice.FiscalTerminalInfo = new FiscalTerminalInfo()
            {
                TerminalAddessAndPort = "",
                TerminalID = OpsContext.WorkstationNumber.ToString()
            };

            return invoice;
        }

        private itemsPlaceholderObject fillInvoiceItemData(bool isVoid)
        {
            decimal invoiceTotalNetAmount = 0;
            decimal invoiceTotalGross = 0;
            bool isSD = true;
            CisConfiguration cisConfiguration = ReadCisConfiguration();
            itemsPlaceholderObject tempItemsList = new itemsPlaceholderObject()
            {
                postings = new List<Posting>(),
                revenueBucketInfos = new List<RevenueBucketInfo>(),
                TrxInfo = new List<TrxInfo>(),
                totalInfo = new TotalInfo()
            };
            Posting postingItem = null;
            RevenueBucketInfo revenueBucketItem = null;
            TrxInfo trxInfoItem = null;
            Dtos.Taxes taxes = new Dtos.Taxes();
            taxes.Tax = calculateTaxes();
            List<PaymentMethod> tenderMediaSettings = GetTenderMediaSettings();

            if (isVoid)
            {
                VoidCheckItems = new VoidCheckItems()
                {
                    Items = new List<VoidCheckItem>(),
                    Total = 0,
                    TotalPaid = 0,
                    TotalTendered = 0
                };
                List<Tax> voidTax = new List<Tax>();
                foreach (Tax calculatedTax in taxes.Tax)
                {
                    Tax negatedTax = NegateTax(calculatedTax);
                    if (negatedTax != null)
                    {
                        voidTax.Add(negatedTax);
                    }
                }
                taxes.Tax = voidTax;
            }

            foreach (Micros.PosCore.Extensibility.Ops.CheckDetailItem item in base.OpsContext.CheckDetail.Where((Micros.PosCore.Extensibility.Ops.CheckDetailItem c) => !c.LineNumVoid))
            {
                if ((int)item.DetailType == 1 && isVoid)
                {
                    var itemDetails = item as OpsMenuItemDetail;

                    var existingItem = VoidCheckItems.Items.Find(voidCheckItem => ItemAlreadyAdded(item, voidCheckItem));

                    var miObjectNumber = item.GetType().GetProperty("MiObjNum");
                    var objectNumber = miObjectNumber.GetValue(item);

                    if (existingItem == null)
                    {
                        VoidCheckItems.Items.Add(new VoidCheckItem()
                        {
                            ObjectNumber = (int)objectNumber,
                            Name = item.Name,
                            Quantity = decimal.Negate(itemDetails.SalesCount),
                            Price = decimal.Negate(Math.Round(itemDetails.Total, 2))
                        });
                    }
                    else
                    {
                        existingItem.Quantity += decimal.Negate(itemDetails.SalesCount);
                        existingItem.Price += decimal.Negate(Math.Round(itemDetails.Total, 2));
                    }
                }
                if ((int)item.DetailType == 4)
                {
                    OpsTenderMediaDetail opsTenderMediaDetail = item as OpsTenderMediaDetail;
                    var parentDetailLink = opsTenderMediaDetail.ParentDetailLink;
                    var tenderGrossAmount = isVoid ? decimal.Negate(opsTenderMediaDetail.Total) : opsTenderMediaDetail.Total;
                    invoiceTotalGross += tenderGrossAmount;
                    isSD = false;
                    postingItem = new Posting()
                    {
                        TrxCode = opsTenderMediaDetail.ObjectNumber.ToString(),
                        TrxDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        TrxType = "FC",
                        UnitPrice = tenderGrossAmount * 1.00M,
                        Quantity = 1.0,
                        TrxDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                        LocalTrxDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                        GrossAmount = tenderGrossAmount * 1.00M,
                        GuestAccountDebit = tenderGrossAmount * 1.00M
                    };
                    tempItemsList.postings.Add(postingItem);

                    var type = tenderMediaSettings.Where((PaymentMethod paymentSetting) => opsTenderMediaDetail.ObjectNumber == paymentSetting.ObjectNumber);
                    string selectedPaymentMethod = type.Count() != 0 ? type.FirstOrDefault().Type.ToString() : "O";

                    if (selectedPaymentMethod == "NONE") //invoice does not require fiscalization
                    {
                        fiscalize = false;
                    }
                    else if (selectedPaymentMethod == "G")
                    {
                        openCashDrawer = true;
                    }

                    paymentMethod = string.Copy(selectedPaymentMethod);

                    revenueBucketItem = new RevenueBucketInfo()
                    {
                        BucketCode = selectedPaymentMethod,
                        BucketType = "FLIP_PAY_SUBTYPE",
                        BucketValue = selectedPaymentMethod,
                        Description = opsTenderMediaDetail.Name,
                        BucketCodeTotalGross = tenderGrossAmount * 1.00M,
                        TrxCode = new List<string>()
                        {
                            opsTenderMediaDetail.ObjectNumber.ToString()

                        }
                    };
                    tempItemsList.revenueBucketInfos.Add(revenueBucketItem);

                    trxInfoItem = new TrxInfo()
                    {
                        HotelCode = cisConfiguration.SellerName,
                        Group = "PAY",
                        SubGroup = opsTenderMediaDetail.Name,
                        Code = selectedPaymentMethod,
                        TrxType = "FC",
                        Description = opsTenderMediaDetail.ObjectNumber.ToString(),
                        Articles = { },
                        TranslatedDescriptions = { },
                        TrxCodeType = "O"
                    };

                    if (isVoid)
                    {
                        VoidCheckItems.TotalTendered = tenderGrossAmount;
                        VoidCheckItems.TenderName = opsTenderMediaDetail.Name;
                    }

                }
            }

            var nonTaxableAmount = isVoid ? decimal.Negate(Mikos.SI.Fiscal.TaxesByTaxRates.NonTaxable) : Mikos.SI.Fiscal.TaxesByTaxRates.NonTaxable;
            tempItemsList.totalInfo = new TotalInfo()
            {
                NetAmount = invoiceTotalNetAmount,
                GrossAmount = invoiceTotalGross,
                NonTaxableAmount = nonTaxableAmount,
                PaidOut = 0.0m,
                Taxes = taxes
            };

            if (isVoid)
            {
                VoidCheckItems.Total = Math.Round(invoiceTotalGross, 2) * 1.00m;
                VoidCheckItems.TotalPaid = Math.Round(invoiceTotalGross, 2) * 1.00m;
            }

            return fiscalize ? tempItemsList : null;
        }

        private Tax NegateTax(Tax calculatedTax)
        {
            if (calculatedTax.Value != 0)
            {
                return new Tax(
                    int.Parse(calculatedTax.Name),
                    decimal.Negate(calculatedTax.Value),
                    decimal.Negate(calculatedTax.NetAmount),
                    calculatedTax.Percent,
                    calculatedTax.Amount
                );
            }
            return null;
        }

        private bool ItemAlreadyAdded(Micros.PosCore.Extensibility.Ops.CheckDetailItem item, VoidCheckItem voidCheckItem)
        {
            OpsCheckDetailItem checkDetailItem = item as OpsCheckDetailItem;

            var miObjectNumber = item.GetType().GetProperty("MiObjNum");
            var value = miObjectNumber.GetValue(item);

            if (voidCheckItem.ObjectNumber == (int)value)
            {
                return true;
            }

            return false;
        }

        private List<Tax> calculateTaxes()
        {
            SimTaxCalc();
            List<Tax> taxList = new List<Tax>();

            if (Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Vat != 0.0m)
                taxList.Add(new Tax(1, Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Vat, Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Net, Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Name, ""));
            if (Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Vat != 0.0m)
                taxList.Add(new Tax(2, Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Vat, Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Net, Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Name, ""));
            if (Mikos.SI.Fiscal.TaxesByTaxRates.Tax3Vat != 0.0m)
                taxList.Add(new Tax(3, Mikos.SI.Fiscal.TaxesByTaxRates.Tax3Vat, Mikos.SI.Fiscal.TaxesByTaxRates.Tax3Net, Mikos.SI.Fiscal.TaxesByTaxRates.Tax3Name, ""));
            if (Mikos.SI.Fiscal.TaxesByTaxRates.Tax4Vat != 0.0m)
                taxList.Add(new Tax(4, Mikos.SI.Fiscal.TaxesByTaxRates.Tax4Vat, Mikos.SI.Fiscal.TaxesByTaxRates.Tax4Net, Mikos.SI.Fiscal.TaxesByTaxRates.Tax4Name, ""));

            return taxList;
        }

        private void SimTaxCalc()
        {
            OpsCommand command = new OpsCommand
            {
                Command = OpsCommandType.SimInquire,
                Arguments = "Mikos.SI.Fiscal:GetTaxTotals"
            };

            base.OpsContext.ProcessCommand(command);
        }

        private void TransactionCancel()
        {
            OpsCommand item = new OpsCommand(OpsCommandType.TransactionCancel);
            OpsCommand item2 = new OpsCommand(OpsCommandType.EnterKey);
            List<OpsCommand> data = new List<OpsCommand> { item, item2 };
            OpsCommand command = new OpsCommand(OpsCommandType.Macro)
            {
                Data = data
            };
            base.OpsContext.ProcessCommand(command);
        }

        public static void ReceiveTaxTotals(string tax1Name, decimal tax1Net, decimal tax1Vat, string tax2Name, decimal tax2Net, decimal tax2Vat, decimal nonTaxable)
        {
            Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Net = 0m;
            Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Vat = 0m;
            Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Net = 0m;
            Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Vat = 0m;
            Mikos.SI.Fiscal.TaxesByTaxRates.Tax3Net = 0m;
            Mikos.SI.Fiscal.TaxesByTaxRates.Tax3Vat = 0m;
            Mikos.SI.Fiscal.TaxesByTaxRates.Tax4Net = 0m;
            Mikos.SI.Fiscal.TaxesByTaxRates.Tax4Vat = 0m;
            Mikos.SI.Fiscal.TaxesByTaxRates.NonTaxable = 0m;
            if (!string.IsNullOrEmpty(tax1Name))
            {
                Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Name = tax1Name;
                Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Net = tax1Net;
                Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Vat = tax1Vat;
            }
            if (!string.IsNullOrEmpty(tax2Name))
            {
                Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Name = tax2Name;
                Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Net = tax2Net;
                Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Vat = tax2Vat;
            }
            if (tax1Vat != 0.0m || tax2Vat != 0.0m)
            {
                Mikos.SI.Fiscal.TaxesByTaxRates.hasTaxes = true;
            }
            else
            {
                Mikos.SI.Fiscal.TaxesByTaxRates.hasTaxes = false;
            }
            Mikos.SI.Fiscal.TaxesByTaxRates.NonTaxable = nonTaxable;
        }

        private EventProcessingInstruction Application_CustomReceiptEvent(object sender, OpsCustomReceiptEventArgs args)
        {

            CisConfiguration cisConfiguration = ReadCisConfiguration();
            bool isVoid = VoidCheckItems != null;

            if (!isVoid)
            {
                fiscalData = RetrieveCheckFiscalData(this.OpsContext.Check.Guid);
                SimTaxCalc();
            }

            if (string.IsNullOrEmpty(fiscalData.SpecialId) 
                && string.IsNullOrEmpty(fiscalData.Doc1) 
                && string.IsNullOrEmpty(fiscalData.Doc2) 
                && string.IsNullOrEmpty(fiscalData.QrCode))
            {
                return EventProcessingInstruction.AbortEventMonitorProcessingAndContinueEvent;
            } 

            args.HeaderAction = (CustomPrintType)4;

            

            if (fiscalData.SpecialId != null)
            {
                PosPrinterData header = new PosPrinterData()
                {
                    Attribute = (TextAttribute)0,
                    Alignment = (TextAlignment)2,
                    Size = (TextSize)0,
                    TextList = new ArrayList { ReceiptPrintUtil.addNumber(fiscalData.SpecialId, cisConfiguration.BusinessPremiseId, this.OpsContext) }
                };

                if (buyerInfo != null)
                {

                    header.TextList.AddRange(ReceiptPrintUtil.AddBuyerInfoHeader(buyerInfo));
                }

                byte[] cmd = new byte[] { 0x1B, 0x70, 0x00, 0x40, 0x50 }; //command to open cash drawer

                header.TextList.Add(cmd);

                args.CustomHeader = header.TextList;
            }


            args.TrailerAction = (CustomPrintType)3;

            PosPrinterData val = new PosPrinterData()
            {
                Attribute = (TextAttribute)0,
                Alignment = (TextAlignment)2,
                Size = (TextSize)0,
                TextList = new ArrayList { "----------------------------------------" }
            };

            if (isVoid)
            {
                val.TextList.AddRange(ReceiptPrintUtil.VoidCheckItemsPrint(VoidCheckItems));
                isVoid = true;
            }

            if (Mikos.SI.Fiscal.TaxesByTaxRates.hasTaxes)
            {
                val.TextList.AddRange(ReceiptPrintUtil.VATBreakdownPrint(isVoid));
            }


            if (!string.IsNullOrEmpty(fiscalData.Doc1))
            {
                val.TextList.Add(ReceiptPrintUtil.CenterString("- ZOI -"));
                val.TextList.Add(ReceiptPrintUtil.CenterString(fiscalData.Doc1));
            }
            if (!string.IsNullOrEmpty(fiscalData.Doc2))
            {
                val.TextList.Add(ReceiptPrintUtil.CenterString("- EOR -"));
                val.TextList.Add(ReceiptPrintUtil.CenterString(fiscalData.Doc2));
            }

            val.TextList.Add("----------------------------------------");

            if (!string.IsNullOrEmpty(fiscalData.QrCode))
            {
                byte[] qrPrint = ReceiptPrintUtil.GenerateQrCode(fiscalData.QrCode);

                val.TextList.Add(qrPrint);

            }

            ArrayList customTrailerFromEMC = args.CustomTrailer as ArrayList;

            if (customTrailerFromEMC != null)
            {
                foreach (var line in customTrailerFromEMC)
                {
                    val.TextList.Add(line);
                }
            }

            args.CustomTrailer = val.TextList;

            if (openCashDrawer)
            {
                openCashDrawer = false;
                if (cashDrawerSettings.Enabled)
                {
                    val.TextList.Add(cashDrawerSettings.Command);
                }
            }

            return EventProcessingInstruction.Continue;
        }

        private FiscalData RetrieveCheckFiscalData(string chkGuid)
        {
            try
            {
                if (!this.OpsContext.TrainingModeEnabled)
                {
                    FiscalInvoice fiscalInvoice = fiscalDataService.retrieveFiscalInvoiceByChkGUID(this.OpsContext, this.OpsContext.Check.Guid);
                    if (fiscalInvoice != null)
                    {
                        return new FiscalData()
                        {
                            Doc1 = fiscalInvoice.ZOI,
                            Doc2 = fiscalInvoice.EOR,
                            SpecialId = fiscalInvoice.SpecialId,
                            QrCode = fiscalInvoice.VerifyInvoiceCode
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                this.OpsContext.ShowError(ex.Message);
                return new FiscalData();
            }

            return new FiscalData();
        }


        private List<PaymentMethod> GetTenderMediaSettings()
        {
            try
            {
                var enumerable = from item in XDocument.Parse(base.DataStore.ReadExtensionApplicationContentTextByNameKey(base.OpsContext.RvcNumber, ApplicationName, "FiscalConfig"))
                                 .Descendants("payment")
                                 select (item);

                //IEnumerable<XElement> enumerable = from item in XDocument.Load(dllLocationPath + "\\FiscalConfig.xml").Descendants("payment") select (item);
                List<PaymentMethod> list = new List<PaymentMethod>();
                foreach (XElement item in enumerable)
                {
                    PaymentMethod paymentMethod = new PaymentMethod();
                    XElement xElement = item.Element("type");
                    if (xElement != null)
                    {
                        paymentMethod.Type = (TenderMediaType)Enum.Parse(typeof(TenderMediaType), xElement.Value);
                    }
                    XElement xElement2 = item.Element("description");
                    if (xElement2 != null)
                    {
                        paymentMethod.Description = xElement2.Value;
                    }
                    XElement xElement3 = item.Element("objectNumber");
                    if (xElement3 != null)
                    {
                        paymentMethod.ObjectNumber = Convert.ToInt32(xElement3.Value);
                    }
                    list.Add(paymentMethod);
                }
                return list;
            }
            catch (Exception ex)
            {
                this.OpsContext.ShowError("Error reading Fiscal Payments Configuration from the Database. Please contact Support.\n" + ex.Message);
                return null;
            }
        }
        private string GetEmployeeVatId()
        {
            int transEmployeeNumber = this.OpsContext.TransEmployeeNumber;
            Micros.PosCore.Extensibility.DataStore.DbRecords.DbEmployee transactionEmployee = this.DataStore.ReadEmployeeByNum(transEmployeeNumber);
            bool usePayrollId = UsePayrollId();
            string empVatId = getEmpVatId(usePayrollId, transactionEmployee);
            if (string.IsNullOrEmpty(empVatId))
            {
                this.OpsContext.ShowError("Invalid employee VAT id.\nPlease, check the system configuration.");
            }
            return empVatId;
        }
        private bool UsePayrollId()
        {
            try
            {
                using (IEnumerator<XElement> enumerator = (from item in XDocument.Parse(base.DataStore.ReadExtensionApplicationContentTextByNameKey(base.OpsContext.RvcNumber, ApplicationName, "FiscalConfig")).Descendants("global")
                                                           select (item) into el
                                                           select el.Element("usepayrollid") into xElementPayrollId
                                                           where xElementPayrollId != null
                                                           select xElementPayrollId).GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        if (bool.TryParse(enumerator.Current.Value, out var result))
                        {
                            return result;
                        }
                        return true;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return true;
            }
        }
        private string getEmpVatId(bool usePayrollId, Micros.PosCore.Extensibility.DataStore.DbRecords.DbEmployee transactionEmployee)
        {
            if (usePayrollId)
            {
                switch (getTransactionType())
                {
                    case TransactionEmployeeType.PID:
                        return transactionEmployee.PayrollID;
                        break;
                    case TransactionEmployeeType.PIN:
                        return transactionEmployee.PIN;
                        break;
                    default:
                        break;
                }
            }
            return this.DataStore.ReadExtensionDataValue("EMPLOYEE", "EmployeeVatId", ((BaseDbKey<long>)(object)transactionEmployee.EmployeeID));
        }
        private TransactionEmployeeType getTransactionType()
        {
            TransactionEmployeeType result = TransactionEmployeeType.PID;
            try
            {
                using (IEnumerator<XElement> enumerator = (from item in XDocument.Parse(base.DataStore.ReadExtensionApplicationContentTextByNameKey(base.OpsContext.RvcNumber, ApplicationName, "FiscalConfig")).Descendants("global")
                                                           select (item) into el
                                                           select el.Element("transactiontype") into xTransactionType
                                                           where xTransactionType != null
                                                           select xTransactionType).GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        if (Enum.TryParse<TransactionEmployeeType>(enumerator.Current.Value, ignoreCase: true, out result))
                        {
                            return result;
                        }
                        return TransactionEmployeeType.PID;
                    }
                }
                return TransactionEmployeeType.PID;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        

        private CisConfiguration ReadCisConfiguration()
        {
            try
            {
                IEnumerable<XElement> enumerable = from item in XDocument.Parse(base.DataStore.ReadExtensionApplicationContentTextByNameKey(base.OpsContext.RvcNumber, ApplicationName, "FiscalConfig")).Descendants("cis")
                                                   select (item);

                CisConfiguration cisConfiguration = new CisConfiguration();
                foreach (XElement item in enumerable)
                {
                    cisConfiguration.PropertyTaxNumber = item?.Element("PropertyTaxNumber")?.Value;
                    if (string.IsNullOrEmpty(cisConfiguration.PropertyTaxNumber))
                    {
                        this.OpsContext.ShowError("Error reading Fiscal Configuration from the Database. Please contact Support.");
                    }
                    cisConfiguration.PropertyCode = item?.Element("PropertyCode")?.Value;
                    if (string.IsNullOrEmpty(cisConfiguration.PropertyCode))
                    {
                        this.OpsContext.ShowError("Error reading Fiscal Configuration from the Database. Please contact Support.");
                    }
                    cisConfiguration.BusinessPremiseId = item?.Element("BusinessPremiseId")?.Value;
                    if (string.IsNullOrEmpty(cisConfiguration.BusinessPremiseId))
                    {
                        this.OpsContext.ShowError("Error reading Fiscal Configuration from the Database. Please contact Support.");
                    }
                    cisConfiguration.SellerName = item?.Element("SellerName")?.Value;
                    if (string.IsNullOrEmpty(cisConfiguration.SellerName))
                    {
                        this.OpsContext.ShowError("Error reading Fiscal Configuration from the Database. Please contact Support.");
                    }
                    cisConfiguration.SellerOwner = item?.Element("SellerOwner")?.Value;
                    if (string.IsNullOrEmpty(cisConfiguration.SellerOwner))
                    {
                        this.OpsContext.ShowError("Error reading Fiscal Configuration from the Database. Please contact Support.");
                    }
                    cisConfiguration.SellerAddr = item?.Element("SellerAddr")?.Value;
                    if (string.IsNullOrEmpty(cisConfiguration.SellerAddr))
                    {
                        this.OpsContext.ShowError("Error reading Fiscal Configuration from the Database. Please contact Support.");
                    }
                    cisConfiguration.SellerCity = item?.Element("SellerCity")?.Value;
                    if (string.IsNullOrEmpty(cisConfiguration.SellerCity))
                    {
                        this.OpsContext.ShowError("Error reading Fiscal Configuration from the Database. Please contact Support.");
                    }
                    cisConfiguration.SellerCountry = item?.Element("SellerCountry")?.Value;
                    if (string.IsNullOrEmpty(cisConfiguration.SellerCountry))
                    {
                        this.OpsContext.ShowError("Error reading Fiscal Configuration from the Database. Please contact Support.");
                    }
                }
                return cisConfiguration;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private string ReadServiceUrlBase()
        {
            try
            {
                IEnumerable<XElement> enumerable = from item in XDocument.Parse(base.DataStore.ReadExtensionApplicationContentTextByNameKey(base.OpsContext.RvcID, ApplicationName, "FiscalConfig")).Descendants("service")
                                                   select (item);

                string result = "";
                foreach (XElement item in enumerable)
                {
                    result = item?.Element("url")?.Value;
                    if (string.IsNullOrEmpty(result))
                    {
                        this.OpsContext.ShowError("Error reading Fiscal Service URL from the Database. Please contact Support.");
                    }
                }
                return result;
            }
            catch (Exception ex)
            { 
                return "";
            }
        }

        private CashDrawerSettings ReadOpenCashDrawerSetting()
        {
            try
            {
                IEnumerable<XElement> enumerable = from item in XDocument.Parse(base.DataStore.ReadExtensionApplicationContentTextByNameKey(base.OpsContext.RvcID, ApplicationName, "FiscalConfig")).Descendants("global")
                                                   select (item);
                CashDrawerSettings result = new CashDrawerSettings()
                {
                    Enabled = false
                };
                foreach (XElement item in enumerable)
                {
                    bool enabled = false;
                    if (bool.TryParse(item?.Element("opencashdrawer")?.Value, out enabled))
                    {
                        result.Enabled = enabled;
                    }
                }

                if (result.Enabled)
                {
                    foreach (XElement item in enumerable)
                    {
                        string b64Command = item?.Element("command")?.Value;
                        if (!string.IsNullOrWhiteSpace(b64Command))
                        {
                            result.Command = Convert.FromBase64String(b64Command);
                        }
                    } 
                }
                return result;
            }
            catch (Exception ex)
            {
                return new CashDrawerSettings()
                {
                    Enabled = false
                };
            }
        }

        private async Task<FiscalResponseData> sendRequestToRacunko(FiscalRequestData invoice)
        {
            var result = await ApiHelper.PostAsync(invoice, ReadServiceUrlBase() + invoicesUrl);

            return result;
        }

        [ExtensibilityMethod]
        public void Retransmit(object arg)
        {
            try
            {
                switch (getTypeFromArgs(arg))
                {
                    case RetransmitType.Undefined:
                        base.OpsContext.ShowError($"Undefined arguments for Retransmit [{arg}]");
                        break;
                    case RetransmitType.FiscalInvoice:
                        retransmitFiscalInvoice();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                base.OpsContext.EndProgressRequest();
                base.OpsContext.ShowError("Error during manual Fiscal Invoice retransmission:\n" + ex.Message + " \r\n " + ex.InnerException?.Message);
            }
        }

        private void retransmitFiscalInvoice()
        {
            using (FiscalContext fiscalContext = new FiscalContext())
            {
                IList<OpsSelectionEntry> list = (from fr in fiscalContext.FiscalInvoices.Where((FiscalInvoice m) => m.Queued || m.Error).ToList()
                                                 select new OpsSelectionEntry(fr.CheckNumber, $"{fr.RvcNumber}-{fr.WorkstationNumber} | {fr.SyncDateTime} | {fr.ChkGUID}")).ToList();
                int? num = base.OpsContext.SelectionRequest("Manual retransmission", "Select Fiscal Invoice to retransmit:", list);
                if (!num.HasValue)
                {
                    return;
                }
                OpsSelectionEntry opsSelectionEntry = list[Convert.ToInt32(num)];
                long chkNum = opsSelectionEntry.Number;
                string checkGuid = opsSelectionEntry.Name.Split('|').LastOrDefault();
                if (checkGuid == null)
                {
                    return;
                }
                OpsCommand openCheckByGuid = new OpsCommand(OpsCommandType.ReopenClosedCheckByGuid);
                openCheckByGuid.Data = checkGuid.Trim();
                base.OpsContext.ProcessCommand(openCheckByGuid);
            }
        }

        private RetransmitType getTypeFromArgs(object args)
        {
            RetransmitType result = RetransmitType.Undefined;
            if (!Enum.TryParse<RetransmitType>(args.ToString(), ignoreCase: true, out result))
            {
                throw new Exception($"Retransmit operation inconsistencies with argument [{args}]");
            }
            return result;
        }

        [ExtensibilityMethod]
        public void AddBuyer(object arg)
        {
            try
            {
                BuyerInfoEntryBoxData BuyerInfoEntryBoxData = new BuyerInfoEntryBoxData();
                object obj = this.OpsContext.RequestCustomDialog("Enter Information", "Enter data", "BuyerInfo.NewBuyerInfoInput", (object)BuyerInfoEntryBoxData);
                if (obj == null)
                {
                    return;
                }
                BuyerInfoInput BuyerInfoInput = new BuyerInfoInput
                {
                    name = BuyerInfoEntryBoxData.BuyerName,
                    vatId = BuyerInfoEntryBoxData.VatId,
                    address = BuyerInfoEntryBoxData.Address,
                    town = BuyerInfoEntryBoxData.Town,
                    zip = BuyerInfoEntryBoxData.ZIP,
                    country = BuyerInfoEntryBoxData.Country,
                    active = true
                };
                bool isCorrect = this.OpsContext.AskQuestion($"{BuyerInfoInput.name,-25}\n{BuyerInfoInput.address,-25}\n{BuyerInfoInput.vatId,-25}\n{BuyerInfoInput.town,-25}\n{BuyerInfoInput.zip,-25}\n{BuyerInfoInput.country,-25}\n\nIs buyer information correct?");
                while (!isCorrect)
                {
                    obj = this.OpsContext.RequestCustomDialog("Enter Information", "Enter data", "BuyerInfo.NewBuyerInfoInput", (object)BuyerInfoEntryBoxData);
                    if (obj == null)
                    {
                        return;
                    }
                    BuyerInfoInput = new BuyerInfoInput
                    {
                        name = BuyerInfoEntryBoxData.BuyerName,
                        vatId = BuyerInfoEntryBoxData.VatId,
                        address = BuyerInfoEntryBoxData.Address,
                        town = BuyerInfoEntryBoxData.Town,
                        zip = BuyerInfoEntryBoxData.ZIP,
                        country = BuyerInfoEntryBoxData.Country,
                        active = true
                    };
                    isCorrect = this.OpsContext.AskQuestion($"{BuyerInfoInput.name,-25}\n{BuyerInfoInput.address,-25}\n{BuyerInfoInput.vatId,-25}\n{BuyerInfoInput.town,-25}\n{BuyerInfoInput.zip,-25}\n{BuyerInfoInput.country,-25}\n\nIs buyer information correct?");
                }

                if (BuyerInfoUtil.ValidateBuyerData(BuyerInfoInput, this.OpsContext))
                {
                    fiscalDataService.SaveBuyerInfoToDb(this.OpsContext, BuyerInfoInput);
                    return;
                }
                else
                {
                    bool again = this.OpsContext.AskQuestion($"This buyer information will not be saved to the database. \nDo You want to try again?");
                    if (again)
                    {
                        AddBuyer(null);
                    }
                }
            }
            catch (Exception ex)
            {
                this.OpsContext.EndProgressRequest();
                this.OpsContext.ShowError(ex.Message);
            }
        }

        [ExtensibilityMethod]
        public void SelectBuyer(object arg)
        {
            using (FiscalContext fiscalContext = new FiscalContext())
            {
                IList<OpsSelectionEntry> list = (from fr in fiscalContext.BuyerInfos.Where((BuyerInfo m) => m.active).ToList()
                                                 select new OpsSelectionEntry(fr.Id, $"{fr.vatId} | {fr.name}")).ToList();
                int? num = base.OpsContext.SearchRequest("Previous buyers", "Select a buyer:", list);

                if (!num.HasValue)
                {
                    return;
                }
                OpsSelectionEntry opsSelectionEntry = list[Convert.ToInt32(num)];

                buyerInfo = fiscalDataService.retrieveBuyerInfoById(this.OpsContext, opsSelectionEntry.Number);

                if (buyerInfo != null)
                {
                    hasBuyer = true;
                }
            }
        }


        public override void Destroy()
        {
            base.OpsFinalTenderEvent -= Application_FinalTenderEvent;
            base.OpsCustomReceiptEvent -= Application_CustomReceiptEvent;
            base.OpsVoidClosedCheckEventPreview -= Application_VoidClosedCheckEventPreview;
            base.OpsVoidReasonEvent -= Application_VoidReasonEvent;
            this.Destroy();
        }
    }
}
