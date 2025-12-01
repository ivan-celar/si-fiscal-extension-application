NetImport from "C:\Micros\Simphony\WebServer\wwwroot\EGateway\Handlers\ExtensionApplications\Mikos.HR.Fiscal\Mikos.HR.Fiscal.dll"

var Tax01Name           : A5 = "25.00"
var Tax01Gross          : $12 
var Tax01Net            : $12 
var Tax01Vat            : $12 
var Tax02Name           : A5 = "13.00"
var Tax02Gross          : $12
var Tax02Net            : $12
var Tax02Vat            : $12
var Tax03Name           : A5 = ""
var Tax03Gross          : $12 = 0   // Currently not used
var Tax03Net            : $12 = 0   // Currently not used
var Tax03Vat            : $12 = 0   // Currently not used
var Tax04Name           : A5 = ""
var Tax04Gross          : $12 = 0   // Currently not used
var Tax04Net            : $12 = 0   // Currently not used
var Tax04Vat            : $12 = 0   // Currently not used
var Tax05Name           : A5 = ""
var Tax05Gross          : $12 = 0   // Currently not used
var Tax05Net            : $12 = 0   // Currently not used
var Tax05Vat            : $12 = 0   // Currently not used
var Tax06Name           : A5 = ""
var Tax06Gross          : $12 = 0   // Currently not used
var Tax06Net            : $12 = 0   // Currently not used
var Tax06Vat            : $12 = 0   // Currently not used
var Tax07Name           : A5 = ""
var Tax07Gross          : $12 = 0   // Currently not used
var Tax07Net            : $12 = 0   // Currently not used
var Tax07Vat            : $12 = 0   // Currently not used
var Tax08Name           : A5 = ""
var Tax08Gross          : $12 = 0   // Currently not used
var Tax08Net            : $12 = 0   // Currently not used
var Tax08Vat            : $12 = 0   // Currently not used
var PnpName             : A5 = "3.00"
var PnpGross            : $12 
var PnpNet              : $12
var PnpVat              : $12 
var NonTaxable          : $12

event inq : GetTaxTotals
    SetSignOnLeft
    call TaxCalc
    Mikos.HR.Fiscal.Application.ReceiveTaxTotals(Tax01Name, Tax01Net, Tax01Vat, \
    Tax02Name, Tax02Net, Tax02Vat,\
    Tax03Name, Tax03Net, Tax03Vat, \
    Tax04Name, Tax04Net, Tax04Vat, \
    Tax05Name, Tax05Net, Tax05Vat, \
    Tax06Name, Tax06Net, Tax06Vat, \
    Tax07Name, Tax07Net, Tax07Vat, \
    Tax08Name, Tax08Net, Tax08Vat, \
    PnpName, pnpNet, PnpVat, \
    NonTaxable)
endevent

sub TaxCalc
    Tax01Vat = 0
    Tax01Gross = 0
    Tax01Net = 0
    Tax02Vat = 0
    Tax02Gross = 0
    Tax02Net = 0
    PnpVat = 0
    PnpGross = 0
    PnpNet = 0
    NonTaxable = 0

    
    if @TXBL[ 1 ] <> 0 and @TXBL[ 2 ] = 0
        Tax01Vat = Tax01Vat + @TAXVAT[ 1 ]
        Tax01Gross = Tax01Gross + @TXBL[ 1 ]
        Tax01Net = Tax01Gross - @TAXVAT[ 1 ]

    elseif @TXBL[ 2 ] <> 0 and @TXBL[ 1 ] = 0
        Tax01Vat = Tax01Vat + @TAXVAT[ 2 ]
        Tax01Gross = Tax01Gross + @TXBL[ 2 ]
        Tax01Net = Tax01Gross - @TAXVAT[ 2 ] - @TAXVAT[ 3 ]
        
    elseif @TXBL[ 2 ] <> 0 and @TXBL[ 1 ] <> 0
        Tax01Vat = Tax01Vat + @TAXVAT[ 1 ] + @TAXVAT[ 2 ]
        Tax01Gross = Tax01Gross + @TXBL[ 1 ] + @TXBL[ 2 ]
        Tax01Net = Tax01Gross - @TAXVAT[ 1 ] - @TAXVAT[ 2 ] - @TAXVAT[ 3 ]
    endif

    if @TXBL[ 10 ] <> 0 and @TXBL[ 11 ] = 0
        Tax01Vat = Tax01Vat + @TAXVAT[ 10 ]
        Tax01Gross = Tax01Gross + @TXBL[ 10 ]
        Tax01Net = Tax01Gross - @TAXVAT[ 10 ]

    elseif @TXBL[ 11 ] <> 0 and @TXBL[ 10 ] = 0
        Tax01Vat = Tax01Vat + @TAXVAT[ 11 ]
        Tax01Gross = Tax01Gross + @TXBL[ 11 ]
        Tax01Net = Tax01Gross - @TAXVAT[ 11 ] - @TAXVAT[ 12 ]
        
    elseif @TXBL[ 11 ] <> 0 and @TXBL[ 10 ] <> 0
        Tax01Vat = Tax01Vat + @TAXVAT[ 10 ] + @TAXVAT[ 11 ]
        Tax01Gross = Tax01Gross + @TXBL[ 10 ] + @TXBL[ 11 ]
        Tax01Net = Tax01Gross - @TAXVAT[ 10 ] - @TAXVAT[ 11 ] - @TAXVAT[ 12 ]
    endif

    if @TXBL[ 3 ] <> 0 AND @TXBL[ 6 ] = 0               
        PnpVat = PnpVat + @TAXVAT[ 3 ]
        PnpGross = PnpGross + @TXBL[ 2 ]
        PnpNet = PnpGross - @TAXVAT[ 2 ] - @TAXVAT[ 3 ]

    elseif @TXBL[ 3 ] = 0 AND @TXBL[ 6 ] <> 0
        PnpVat = PnpVat + @TAXVAT[ 6 ]
        PnpGross = PnpGross + @TXBL[ 5 ]
        PnpNet = PnpGross - @TAXVAT[ 5 ] - @TAXVAT[ 6 ]
        
    elseif @TXBL[ 3 ] <> 0 AND @TXBL[ 6 ] <> 0
        PnpVat = PnpVat + @TAXVAT[ 3 ] + @TAXVAT[ 6 ]
        PnpGross = PnpGross + @TXBL[ 2 ] + @TXBL[ 5 ]
        PnpNet = PnpGross - @TAXVAT[ 2 ] - @TAXVAT[ 3 ] - @TAXVAT[ 5 ] - @TAXVAT[ 6 ]
    endif

    if @TXBL[ 12 ] <> 0     
        PnpVat = PnpVat + @TAXVAT[ 12 ]
        PnpGross = PnpGross + @TXBL[ 11 ]
        PnpNet = PnpGross - @TAXVAT[ 11 ] - @TAXVAT[ 12 ]
    endif

    if @TXBL[ 4 ] <> 0 AND @TXBL[ 5 ] = 0
        Tax02Vat = Tax02Vat + @TAXVAT[ 4 ]
        Tax02Gross = Tax02Gross + @TXBL[ 4 ]
        Tax02Net = Tax02Gross - @TAXVAT[ 4 ]

    elseif @TXBL[ 4 ] <> 0 AND @TXBL[ 5 ] <> 0
        Tax02Vat = Tax02Vat + @TAXVAT[ 4 ] + @TAXVAT[ 5 ]
        Tax02Gross = Tax02Gross + @TXBL[ 4 ] + @TXBL[ 5 ]
        Tax02Net = Tax02Gross - @TAXVAT[ 4 ] - @TAXVAT[ 5 ] - @TAXVAT[ 6 ]
        
    elseif @TXBL[ 4 ] = 0 AND @TXBL[ 5 ] <> 0
        Tax02Vat = Tax02Vat + @TAXVAT[ 5 ]
        Tax02Gross = Tax02Gross + @TXBL[ 5 ]
        Tax02Net = Tax02Gross - @TAXVAT[ 5 ] - @TAXVAT[ 6 ]
    endif

    if @TXBL[ 8 ] <> 0
        NonTaxable = @TXBL[ 8 ] 
    endif

endsub