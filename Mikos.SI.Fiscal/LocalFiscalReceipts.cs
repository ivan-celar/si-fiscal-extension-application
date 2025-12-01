using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikos.SI.Fiscal
{
    public class LocalFiscalReceipts
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);

        private long _L_SeqNum;

        private string _L_CertificateName;

        private DateTime _L_ChkCloseDateTime;

        private long _L_ChkNum;

        private int _L_RevenueCenterId;

        private long _L_ChkSeqNum;

        private int _L_WorkstationId;

        private string _L_EmployeeVatId;

        private string _L_UUID;

        private string _L_JIR;

        private bool _L_Queued;

        private DateTime? _L_SyncDateTime;

        private bool _L_Error;

        private string _L_ErrorCode;

        private string _L_ErrorDesc;

        private string _L_PaymentMethod;

        private string _L_PaymentTotal;

        private string _L_Tax1Name;

        private string _L_Tax1Net;

        private string _L_Tax1Vat;

        private string _L_Tax2Name;

        private string _L_Tax2Net;

        private string _L_Tax2Vat;

        private string _L_Tax3Name;

        private string _L_Tax3Net;

        private string _L_Tax3Vat;

        private string _L_Tax4Name;

        private string _L_Tax4Net;

        private string _L_Tax4Vat;

        private string _L_Tax5Name;

        private string _L_Tax5Net;

        private string _L_Tax5Vat;

        private string _L_Tax6Name;

        private string _L_Tax6Net;

        private string _L_Tax6Vat;

        private string _L_Tax7Name;

        private string _L_Tax7Net;

        private string _L_Tax7Vat;

        private string _L_Tax8Name;

        private string _L_Tax8Net;

        private string _L_Tax8Vat;

        private string _L_pnpName;

        private string _L_pnpNet;

        private string _L_pnpVat;

        private string _L_TaxExemptTtl;

        private string _L_NoTaxTtl;

        private string _L_ManDeliveryId;

        private string _L_SpecificPurpose;

        private DateTime _L_InsertDateTime;

        private DateTime? _L_UpdateDateTime;

        private DateTime? _L_MasterDbInsertDateTime;

        private DateTime? _L_MasterDbUpdateDateTime;

        public long L_SeqNum
        {
            get
            {
                return _L_SeqNum;
            }
            set
            {
                if (_L_SeqNum != value)
                {
                    SendPropertyChanging();
                    _L_SeqNum = value;
                    SendPropertyChanged("L_SeqNum");
                }
            }
        }
        public string L_CertificateName
        {
            get
            {
                return _L_CertificateName;
            }
            set
            {
                if (_L_CertificateName != value)
                {
                    SendPropertyChanging();
                    _L_CertificateName = value;
                    SendPropertyChanged("L_CertificateName");
                }
            }
        }

        public DateTime L_ChkCloseDateTime
        {
            get
            {
                return _L_ChkCloseDateTime;
            }
            set
            {
                if (_L_ChkCloseDateTime != value)
                {
                    SendPropertyChanging();
                    _L_ChkCloseDateTime = value;
                    SendPropertyChanged("L_ChkCloseDateTime");
                }
            }
        }

        public long L_ChkNum
        {
            get
            {
                return _L_ChkNum;
            }
            set
            {
                if (_L_ChkNum != value)
                {
                    SendPropertyChanging();
                    _L_ChkNum = value;
                    SendPropertyChanged("L_ChkNum");
                }
            }
        }

        public int L_RevenueCenterId
        {
            get
            {
                return _L_RevenueCenterId;
            }
            set
            {
                if (_L_RevenueCenterId != value)
                {
                    SendPropertyChanging();
                    _L_RevenueCenterId = value;
                    SendPropertyChanged("L_RevenueCenterId");
                }
            }
        }

        public long L_ChkSeqNum
        {
            get
            {
                return _L_ChkSeqNum;
            }
            set
            {
                if (_L_ChkSeqNum != value)
                {
                    SendPropertyChanging();
                    _L_ChkSeqNum = value;
                    SendPropertyChanged("L_ChkSeqNum");
                }
            }
        }

        public int L_WorkstationId
        {
            get
            {
                return _L_WorkstationId;
            }
            set
            {
                if (_L_WorkstationId != value)
                {
                    SendPropertyChanging();
                    _L_WorkstationId = value;
                    SendPropertyChanged("L_WorkstationId");
                }
            }
        }

        public string L_EmployeeVatId
        {
            get
            {
                return _L_EmployeeVatId;
            }
            set
            {
                if (_L_EmployeeVatId != value)
                {
                    SendPropertyChanging();
                    _L_EmployeeVatId = value;
                    SendPropertyChanged("L_EmployeeVatId");
                }
            }
        }

        public string L_UUID
        {
            get
            {
                return _L_UUID;
            }
            set
            {
                if (_L_UUID != value)
                {
                    SendPropertyChanging();
                    _L_UUID = value;
                    SendPropertyChanged("L_UUID");
                }
            }
        }

        public string L_JIR
        {
            get
            {
                return _L_JIR;
            }
            set
            {
                if (_L_JIR != value)
                {
                    SendPropertyChanging();
                    _L_JIR = value;
                    SendPropertyChanged("L_JIR");
                }
            }
        }

        public bool L_Queued
        {
            get
            {
                return _L_Queued;
            }
            set
            {
                if (_L_Queued != value)
                {
                    SendPropertyChanging();
                    _L_Queued = value;
                    SendPropertyChanged("L_Queued");
                }
            }
        }

        public DateTime? L_SyncDateTime
        {
            get
            {
                return _L_SyncDateTime;
            }
            set
            {
                if (_L_SyncDateTime != value)
                {
                    SendPropertyChanging();
                    _L_SyncDateTime = value;
                    SendPropertyChanged("L_SyncDateTime");
                }
            }
        }

        public bool L_Error
        {
            get
            {
                return _L_Error;
            }
            set
            {
                if (_L_Error != value)
                {
                    SendPropertyChanging();
                    _L_Error = value;
                    SendPropertyChanged("L_Error");
                }
            }
        }

        public string L_ErrorCode
        {
            get
            {
                return _L_ErrorCode;
            }
            set
            {
                if (_L_ErrorCode != value)
                {
                    SendPropertyChanging();
                    _L_ErrorCode = value;
                    SendPropertyChanged("L_ErrorCode");
                }
            }
        }

        public string L_ErrorDesc
        {
            get
            {
                return _L_ErrorDesc;
            }
            set
            {
                if (_L_ErrorDesc != value)
                {
                    SendPropertyChanging();
                    _L_ErrorDesc = value;
                    SendPropertyChanged("L_ErrorDesc");
                }
            }
        }

        public string L_PaymentMethod
        {
            get
            {
                return _L_PaymentMethod;
            }
            set
            {
                if (_L_PaymentMethod != value)
                {
                    SendPropertyChanging();
                    _L_PaymentMethod = value;
                    SendPropertyChanged("L_PaymentMethod");
                }
            }
        }

        public string L_PaymentTotal
        {
            get
            {
                return _L_PaymentTotal;
            }
            set
            {
                if (_L_PaymentTotal != value)
                {
                    SendPropertyChanging();
                    _L_PaymentTotal = value;
                    SendPropertyChanged("L_PaymentTotal");
                }
            }
        }

        public string L_Tax1Name
        {
            get
            {
                return _L_Tax1Name;
            }
            set
            {
                if (_L_Tax1Name != value)
                {
                    SendPropertyChanging();
                    _L_Tax1Name = value;
                    SendPropertyChanged("L_Tax1Name");
                }
            }
        }

        public string L_Tax1Net
        {
            get
            {
                return _L_Tax1Net;
            }
            set
            {
                if (_L_Tax1Net != value)
                {
                    SendPropertyChanging();
                    _L_Tax1Net = value;
                    SendPropertyChanged("L_Tax1Net");
                }
            }
        }

        public string L_Tax1Vat
        {
            get
            {
                return _L_Tax1Vat;
            }
            set
            {
                if (_L_Tax1Vat != value)
                {
                    SendPropertyChanging();
                    _L_Tax1Vat = value;
                    SendPropertyChanged("L_Tax1Vat");
                }
            }
        }

        public string L_Tax2Name
        {
            get
            {
                return _L_Tax2Name;
            }
            set
            {
                if (_L_Tax2Name != value)
                {
                    SendPropertyChanging();
                    _L_Tax2Name = value;
                    SendPropertyChanged("L_Tax2Name");
                }
            }
        }

        public string L_Tax2Net
        {
            get
            {
                return _L_Tax2Net;
            }
            set
            {
                if (_L_Tax2Net != value)
                {
                    SendPropertyChanging();
                    _L_Tax2Net = value;
                    SendPropertyChanged("L_Tax2Net");
                }
            }
        }

        public string L_Tax2Vat
        {
            get
            {
                return _L_Tax2Vat;
            }
            set
            {
                if (_L_Tax2Vat != value)
                {
                    SendPropertyChanging();
                    _L_Tax2Vat = value;
                    SendPropertyChanged("L_Tax2Vat");
                }
            }
        }

        public string L_Tax3Name
        {
            get
            {
                return _L_Tax3Name;
            }
            set
            {
                if (_L_Tax3Name != value)
                {
                    SendPropertyChanging();
                    _L_Tax3Name = value;
                    SendPropertyChanged("L_Tax3Name");
                }
            }
        }

        public string L_Tax3Net
        {
            get
            {
                return _L_Tax3Net;
            }
            set
            {
                if (_L_Tax3Net != value)
                {
                    SendPropertyChanging();
                    _L_Tax3Net = value;
                    SendPropertyChanged("L_Tax3Net");
                }
            }
        }

        public string L_Tax3Vat
        {
            get
            {
                return _L_Tax3Vat;
            }
            set
            {
                if (_L_Tax3Vat != value)
                {
                    SendPropertyChanging();
                    _L_Tax3Vat = value;
                    SendPropertyChanged("L_Tax3Vat");
                }
            }
        }

        public string L_Tax4Name
        {
            get
            {
                return _L_Tax4Name;
            }
            set
            {
                if (_L_Tax4Name != value)
                {
                    SendPropertyChanging();
                    _L_Tax4Name = value;
                    SendPropertyChanged("L_Tax4Name");
                }
            }
        }

        public string L_Tax4Net
        {
            get
            {
                return _L_Tax4Net;
            }
            set
            {
                if (_L_Tax4Net != value)
                {
                    SendPropertyChanging();
                    _L_Tax4Net = value;
                    SendPropertyChanged("L_Tax4Net");
                }
            }
        }

        public string L_Tax4Vat
        {
            get
            {
                return _L_Tax4Vat;
            }
            set
            {
                if (_L_Tax4Vat != value)
                {
                    SendPropertyChanging();
                    _L_Tax4Vat = value;
                    SendPropertyChanged("L_Tax4Vat");
                }
            }
        }

        public string L_Tax5Name
        {
            get
            {
                return _L_Tax5Name;
            }
            set
            {
                if (_L_Tax5Name != value)
                {
                    SendPropertyChanging();
                    _L_Tax5Name = value;
                    SendPropertyChanged("L_Tax5Name");
                }
            }
        }

        public string L_Tax5Net
        {
            get
            {
                return _L_Tax5Net;
            }
            set
            {
                if (_L_Tax5Net != value)
                {
                    SendPropertyChanging();
                    _L_Tax5Net = value;
                    SendPropertyChanged("L_Tax5Net");
                }
            }
        }

        public string L_Tax5Vat
        {
            get
            {
                return _L_Tax5Vat;
            }
            set
            {
                if (_L_Tax5Vat != value)
                {
                    SendPropertyChanging();
                    _L_Tax5Vat = value;
                    SendPropertyChanged("L_Tax5Vat");
                }
            }
        }

        public string L_Tax6Name
        {
            get
            {
                return _L_Tax6Name;
            }
            set
            {
                if (_L_Tax6Name != value)
                {
                    SendPropertyChanging();
                    _L_Tax6Name = value;
                    SendPropertyChanged("L_Tax6Name");
                }
            }
        }

        public string L_Tax6Net
        {
            get
            {
                return _L_Tax6Net;
            }
            set
            {
                if (_L_Tax6Net != value)
                {
                    SendPropertyChanging();
                    _L_Tax6Net = value;
                    SendPropertyChanged("L_Tax6Net");
                }
            }
        }

        public string L_Tax6Vat
        {
            get
            {
                return _L_Tax6Vat;
            }
            set
            {
                if (_L_Tax6Vat != value)
                {
                    SendPropertyChanging();
                    _L_Tax6Vat = value;
                    SendPropertyChanged("L_Tax6Vat");
                }
            }
        }

        public string L_Tax7Name
        {
            get
            {
                return _L_Tax7Name;
            }
            set
            {
                if (_L_Tax7Name != value)
                {
                    SendPropertyChanging();
                    _L_Tax7Name = value;
                    SendPropertyChanged("L_Tax7Name");
                }
            }
        }

        public string L_Tax7Net
        {
            get
            {
                return _L_Tax7Net;
            }
            set
            {
                if (_L_Tax7Net != value)
                {
                    SendPropertyChanging();
                    _L_Tax7Net = value;
                    SendPropertyChanged("L_Tax7Net");
                }
            }
        }

        public string L_Tax7Vat
        {
            get
            {
                return _L_Tax7Vat;
            }
            set
            {
                if (_L_Tax7Vat != value)
                {
                    SendPropertyChanging();
                    _L_Tax7Vat = value;
                    SendPropertyChanged("L_Tax7Vat");
                }
            }
        }

        public string L_Tax8Name
        {
            get
            {
                return _L_Tax8Name;
            }
            set
            {
                if (_L_Tax8Name != value)
                {
                    SendPropertyChanging();
                    _L_Tax8Name = value;
                    SendPropertyChanged("L_Tax8Name");
                }
            }
        }

        public string L_Tax8Net
        {
            get
            {
                return _L_Tax8Net;
            }
            set
            {
                if (_L_Tax8Net != value)
                {
                    SendPropertyChanging();
                    _L_Tax8Net = value;
                    SendPropertyChanged("L_Tax8Net");
                }
            }
        }

        public string L_Tax8Vat
        {
            get
            {
                return _L_Tax8Vat;
            }
            set
            {
                if (_L_Tax8Vat != value)
                {
                    SendPropertyChanging();
                    _L_Tax8Vat = value;
                    SendPropertyChanged("L_Tax8Vat");
                }
            }
        }

        public string L_pnpName
        {
            get
            {
                return _L_pnpName;
            }
            set
            {
                if (_L_pnpName != value)
                {
                    SendPropertyChanging();
                    _L_pnpName = value;
                    SendPropertyChanged("L_pnpName");
                }
            }
        }

        public string L_pnpNet
        {
            get
            {
                return _L_pnpNet;
            }
            set
            {
                if (_L_pnpNet != value)
                {
                    SendPropertyChanging();
                    _L_pnpNet = value;
                    SendPropertyChanged("L_pnpNet");
                }
            }
        }

        public string L_pnpVat
        {
            get
            {
                return _L_pnpVat;
            }
            set
            {
                if (_L_pnpVat != value)
                {
                    SendPropertyChanging();
                    _L_pnpVat = value;
                    SendPropertyChanged("L_pnpVat");
                }
            }
        }

        public string L_TaxExemptTtl
        {
            get
            {
                return _L_TaxExemptTtl;
            }
            set
            {
                if (_L_TaxExemptTtl != value)
                {
                    SendPropertyChanging();
                    _L_TaxExemptTtl = value;
                    SendPropertyChanged("L_TaxExemptTtl");
                }
            }
        }

        public string L_NoTaxTtl
        {
            get
            {
                return _L_NoTaxTtl;
            }
            set
            {
                if (_L_NoTaxTtl != value)
                {
                    SendPropertyChanging();
                    _L_NoTaxTtl = value;
                    SendPropertyChanged("L_NoTaxTtl");
                }
            }
        }

        public string L_ManDeliveryId
        {
            get
            {
                return _L_ManDeliveryId;
            }
            set
            {
                if (_L_ManDeliveryId != value)
                {
                    SendPropertyChanging();
                    _L_ManDeliveryId = value;
                    SendPropertyChanged("L_ManDeliveryId");
                }
            }
        }

        public string L_SpecificPurpose
        {
            get
            {
                return _L_SpecificPurpose;
            }
            set
            {
                if (_L_SpecificPurpose != value)
                {
                    SendPropertyChanging();
                    _L_SpecificPurpose = value;
                    SendPropertyChanged("L_SpecificPurpose");
                }
            }
        }

        public DateTime L_InsertDateTime
        {
            get
            {
                return _L_InsertDateTime;
            }
            set
            {
                if (_L_InsertDateTime != value)
                {
                    SendPropertyChanging();
                    _L_InsertDateTime = value;
                    SendPropertyChanged("L_InsertDateTime");
                }
            }
        }

        public DateTime? L_UpdateDateTime
        {
            get
            {
                return _L_UpdateDateTime;
            }
            set
            {
                if (_L_UpdateDateTime != value)
                {
                    SendPropertyChanging();
                    _L_UpdateDateTime = value;
                    SendPropertyChanged("L_UpdateDateTime");
                }
            }
        }

        public DateTime? L_MasterDbInsertDateTime
        {
            get
            {
                return _L_MasterDbInsertDateTime;
            }
            set
            {
                if (_L_MasterDbInsertDateTime != value)
                {
                    SendPropertyChanging();
                    _L_MasterDbInsertDateTime = value;
                    SendPropertyChanged("L_MasterDbInsertDateTime");
                }
            }
        }

        public DateTime? L_MasterDbUpdateDateTime
        {
            get
            {
                return _L_MasterDbUpdateDateTime;
            }
            set
            {
                if (_L_MasterDbUpdateDateTime != value)
                {
                    SendPropertyChanging();
                    _L_MasterDbUpdateDateTime = value;
                    SendPropertyChanged("L_MasterDbUpdateDateTime");
                }
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SendPropertyChanging()
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        protected virtual void SendPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
