using Micros.Ops;
using Mikos.SI.Fiscal.Datastore.Dao;
using Mikos.SI.Fiscal.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikos.SI.Fiscal.Util
{
    public class ReceiptPrintUtil
    {
        public static byte[] GenerateQrCode(string qrCodeData)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write((byte)16);
            binaryWriter.Write((byte)15);
            binaryWriter.Write('\u001b');
            binaryWriter.Write('a');
            binaryWriter.Write((byte)1);
            int num = qrCodeData.Length + 3;
            byte value = (byte)(num % 256);
            byte value2 = (byte)(num / 256);
            binaryWriter.Write((byte)29);
            binaryWriter.Write((byte)40);
            binaryWriter.Write((byte)107);
            binaryWriter.Write((byte)4);
            binaryWriter.Write((byte)0);
            binaryWriter.Write((byte)49);
            binaryWriter.Write((byte)65);
            binaryWriter.Write((byte)50);
            binaryWriter.Write((byte)0);
            binaryWriter.Write((byte)29);
            binaryWriter.Write((byte)40);
            binaryWriter.Write((byte)107);
            binaryWriter.Write((byte)3);
            binaryWriter.Write((byte)0);
            binaryWriter.Write((byte)49);
            binaryWriter.Write((byte)67);
            binaryWriter.Write((byte)5);
            binaryWriter.Write((byte)29);
            binaryWriter.Write((byte)40);
            binaryWriter.Write((byte)107);
            binaryWriter.Write((byte)3);
            binaryWriter.Write((byte)0);
            binaryWriter.Write((byte)49);
            binaryWriter.Write((byte)69);
            binaryWriter.Write((byte)48);
            binaryWriter.Write((byte)29);
            binaryWriter.Write((byte)40);
            binaryWriter.Write((byte)107);
            binaryWriter.Write(value);
            binaryWriter.Write(value2);
            binaryWriter.Write((byte)49);
            binaryWriter.Write((byte)80);
            binaryWriter.Write((byte)48);
            foreach (char c in qrCodeData)
            {
                binaryWriter.Write((byte)c);
            }
            binaryWriter.Write((byte)29);
            binaryWriter.Write((byte)40);
            binaryWriter.Write((byte)107);
            binaryWriter.Write((byte)3);
            binaryWriter.Write((byte)0);
            binaryWriter.Write((byte)49);
            binaryWriter.Write((byte)81);
            binaryWriter.Write((byte)48);
            binaryWriter.Write((byte)16);
            binaryWriter.Write((byte)14);
            binaryWriter.Flush();
            return memoryStream.ToArray();
        }

        public static ArrayList VATBreakdownPrint(bool isVoid)
        {
            ArrayList textList = new ArrayList();
            textList.Add("Stopnja DDV    Osnova DDV     Znesek DDV");
            textList.Add("----------------------------------------");
            if (!string.IsNullOrEmpty(Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Name) && Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Net != 0m)
            {
                string taxName = Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Name;
                decimal taxNet = isVoid ? decimal.Negate(Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Net) : Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Net;
                decimal taxVat = isVoid ? decimal.Negate(Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Vat) : Mikos.SI.Fiscal.TaxesByTaxRates.Tax1Vat;
                string value = $"{taxName,10}%{taxNet,14:0.00}{taxVat,15:0.00}";
                textList.Add(value);
            }
            if (!string.IsNullOrEmpty(Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Name) && Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Net != 0m)
            {
                string taxName = Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Name;
                decimal taxNet = isVoid ? decimal.Negate(Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Net) : Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Net;
                decimal taxVat = isVoid ? decimal.Negate(Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Vat) : Mikos.SI.Fiscal.TaxesByTaxRates.Tax2Vat;
                //string value = $"DDV      {taxName}{taxNet,11:0.00}{taxVat,15:0.00}";
                string value = $"{taxName,10}%{taxNet,14:0.00}{taxVat,15:0.00}";
                textList.Add(value);
            }
            if (!string.IsNullOrEmpty(Mikos.SI.Fiscal.TaxesByTaxRates.Tax3Name) && Mikos.SI.Fiscal.TaxesByTaxRates.Tax3Net != 0m)
            {
                string taxName = Mikos.SI.Fiscal.TaxesByTaxRates.Tax3Name;
                decimal taxNet = isVoid ? decimal.Negate(Mikos.SI.Fiscal.TaxesByTaxRates.Tax3Net) : Mikos.SI.Fiscal.TaxesByTaxRates.Tax3Net;
                decimal taxVat = isVoid ? decimal.Negate(Mikos.SI.Fiscal.TaxesByTaxRates.Tax3Vat) : Mikos.SI.Fiscal.TaxesByTaxRates.Tax3Vat;
                string value = $"{taxName,10}%{taxNet,14:0.00}{taxVat,15:0.00}";
                textList.Add(value);
            }
            if (!string.IsNullOrEmpty(Mikos.SI.Fiscal.TaxesByTaxRates.Tax4Name) && Mikos.SI.Fiscal.TaxesByTaxRates.Tax4Net != 0m)
            {
                string taxName = Mikos.SI.Fiscal.TaxesByTaxRates.Tax4Name;
                decimal taxNet = isVoid ? decimal.Negate(Mikos.SI.Fiscal.TaxesByTaxRates.Tax4Net) : Mikos.SI.Fiscal.TaxesByTaxRates.Tax4Net;
                decimal taxVat = isVoid ? decimal.Negate(Mikos.SI.Fiscal.TaxesByTaxRates.Tax4Vat) : Mikos.SI.Fiscal.TaxesByTaxRates.Tax4Vat;
                string value = $"{taxName,10}%{taxNet,14:0.00}{taxVat,15:0.00}";
                textList.Add(value);
            }
            if (Mikos.SI.Fiscal.TaxesByTaxRates.NonTaxable != 0m)
            {
                string inputString = "DDV ni obračunan na podlagi 6c";
                textList.Add(ReceiptPrintUtil.CenterString(inputString));
                inputString = "odstavka 36. člena ZDDV1.";
                textList.Add(ReceiptPrintUtil.CenterString(inputString));
                textList.Add("----------------------------------------");
            }
            textList.Add("----------------------------------------");

            return textList;
        }

        public static ArrayList AddBuyerInfoHeader(BuyerInfo buyerInfo)
        {
            ArrayList textList = new ArrayList();
            textList.Add("----------------------------------------");
            textList.Add("Name: " + FormatHeaderString(buyerInfo.name));
            textList.Add("Vat ID: " + FormatHeaderString(buyerInfo.vatId));
            textList.Add("Address: " + FormatHeaderString(buyerInfo.address));
            if (buyerInfo.town != null)
            {
                textList.Add("Town: " + FormatHeaderString(buyerInfo.town));
            }
            if (buyerInfo.zip != null)
            {
                textList.Add("Postal code: " + FormatHeaderString(buyerInfo.zip));
            }
            if (buyerInfo.country != null)
            {
                if (!buyerInfo.country.ToLower().Equals("si") && !buyerInfo.country.ToLower().Equals("svn"))
                {
                    textList.Add("Country: " + FormatHeaderString(buyerInfo.country));
                }
            }
            textList.Add("----------------------------------------");

            return textList;
        }

        public static string CenterString(string inputString)
        {
            if (inputString == null)
            {
                return string.Empty;
            }
            int length = inputString.Length;
            return inputString.PadLeft((40 - length) / 2 + length).PadRight(40);
        }

        public static string addNumber(string SPECIALID, string businessPremiseid, OpsContext opsContext)
        {
            StringBuilder sb = new StringBuilder();

            string invoiceNumber = !"0".Equals(SPECIALID) ? sb.Append(businessPremiseid).Append("-").Append(opsContext.WorkstationNumber).Append("-").Append(SPECIALID).ToString() : "";

            return CenterString(invoiceNumber);
        }

        public static string FormatVoidCheckItem(VoidCheckItem item)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(FormatQuantity(item.Quantity));
            sb.Append(" ");
            sb.Append(FormatName(item.Name));
            sb.Append(" ");
            sb.Append(item.Price);


            return sb.ToString();
        }

        public static string FormatTenderNameAndAmount(string tenderName, decimal tenderAmount)
        {
            StringBuilder sb = new StringBuilder();
            int length = 29;

            string tenderAmountString = Math.Round(tenderAmount, 2) + "EUR";

            sb.Append(tenderName);

            for (int i = 0; i < (length - tenderName.Length - tenderAmountString.Length); i++)
            {
                sb.Append(" ");
            }

            sb.Append(tenderAmountString);

            return CenterString(sb.ToString());
        }

        private static string FormatQuantity(decimal quantity)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" ");
            sb.Append(quantity);

            string temp = sb.ToString();

            if (temp.Length < 3)
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        private static string FormatName(string name)
        {
            int length = 25;
            StringBuilder result = new StringBuilder();

            if (name.Length <= length)
            {
                result.Append(name);
                for (int i = 0; i < (length - name.Length); i++)
                {
                    result.Append(" ");
                }
                return result.ToString();
            }

            string[] words = name.Split(' ');
            int currentLength = 0;

            foreach (string word in words)
            {
                if (currentLength + word.Length + (currentLength > 0 ? 1 : 0) > length)
                {
                    result.Append("\n");
                    result.Append("    ");
                    currentLength = 0;
                }
                else if (currentLength > 0)
                {
                    result.Append(" ");
                    currentLength += 1;
                }

                result.Append(word);
                currentLength += word.Length;
            }

            if (currentLength < length)
            {
                for (int i = 0; i < (length - currentLength); i++)
                {
                    result.Append(" ");
                }
            }

            return result.ToString();
        }

        private static string FormatHeaderString(string text)
        {
            int length = 25;
            StringBuilder result = new StringBuilder();

            if (text.Length <= length)
            {
                result.Append(text);
                for (int i = 0; i < (length - text.Length); i++)
                {
                    result.Append(" ");
                }
                return result.ToString();
            }

            string[] words = text.Split(' ');
            int currentLength = 0;

            foreach (string word in words)
            {
                if (currentLength + word.Length + (currentLength > 0 ? 1 : 0) > length)
                {
                    result.Append("\n");
                    result.Append("         ");
                    currentLength = 0;
                }
                else if (currentLength > 0)
                {
                    result.Append(" ");
                    currentLength += 1;
                }

                result.Append(word);
                currentLength += word.Length;
            }

            if (currentLength < length)
            {
                for (int i = 0; i < (length - currentLength); i++)
                {
                    result.Append(" ");
                }
            }

            return result.ToString();
        }

        public static ArrayList VoidCheckItemsPrint(VoidCheckItems voidCheckItems)
        {
            ArrayList result = new ArrayList();

            result.Add("----------------------------------------");
            foreach (VoidCheckItem voidCheckItem in voidCheckItems.Items)
            {
                result.Add(ReceiptPrintUtil.FormatVoidCheckItem(voidCheckItem));
            }
            result.Add("");
            result.Add(ReceiptPrintUtil.FormatTenderNameAndAmount("S K U P A J", voidCheckItems.Total));
            result.Add(ReceiptPrintUtil.FormatTenderNameAndAmount("Placilo", voidCheckItems.TotalTendered));
            result.Add(ReceiptPrintUtil.FormatTenderNameAndAmount(voidCheckItems.TenderName, voidCheckItems.TotalTendered));
            result.Add("");

            return result;
        }
    }
}

