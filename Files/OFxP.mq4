#import "MT4Integration.dll"

// All bool parameters get transfered WRONG, use INT !!!
// all variables are with SMALL LETTERS !!!

// <<< INIT FUNCTIONS >>>

int InitializeServer(string serverAddress);

int InitializeIntegration(string expertID, string symbol,
double modePoint, double modeDigits, double modeSpread, double modeStopLevel, double modeLotSize, double modeTickValue,
double modeTickSize, double modeSwapLong, double modeSwapShort, double modeStarting, double modeExpiration,
double modeTradeAllowed, double modeMinLot, double modeLotStep, double modeMaxLot, double modeSwapType,
double modeProfitCalcMode, double modeMarginCalcMode, double modeMarginInit, double modeMarginMaintenance,
double modeMarginHedged, double modeMarginRequired, double modeFreezeLevel);

int UnInitializeIntegration(string expertID, string symbol);

// <<< TRADE FUNCTIONS >>>

// Those orders are coming from the dll.
string RequestNewOrder(string expertID, string symbol); // Result format ; string& symbol, double& volume, double& desiredPrice (0 for none), int& slippage, double& takeProfit, double& stopLoss, int& orderType, int& operationID, string& comment
string RequestCloseOrder(string expertID, string symbol);
string RequestModifyOrder(string expertID, string symbol);

string RequestOrderInformation(string expertID); // format is : (int& orderTicket, int& operationID)

int RequestAccountInformation(string expertID);

// Request information for all orders currently on the integration.
// Will return an operation ID > 0, if a request has arrived.
int RequestAllOrders(string expertID); // (int& operationID > 0)

void OrderOpened(string expertID, string symbol, int operationID, int orderTicket, double openingPrice, int orderOpenTime, int operationResult, string operationResultMessage);
void OrderClosed(string expertID, string symbol, int operationID, int orderTicket, int orderNewTicket, double closingPrice, int orderCloseTime, int operationResult, string operationResultMessage);
void OrderModified(string expertID, string symbol, int operationID, int orderTicket, int orderNewTicket, int operationResult, string operationResultMessage);

void OrderInformation(string expertID, string symbol, int operationID, int orderTicket, string orderSymbol, int orderType, double volume, 
      double openPrice, double closePrice, double orderStopLoss, double orderTakeProfit, 
      double currentProfit, double orderSwap, int orderPlatformOpenTime, 
      int orderPlatformCloseTime, int orderExpiration, double orderCommission,
      string orderComment, int orderMagicNumber, int operationResult, string operationResultMessage);

void AccountInformation(string expertID, int operationID,
      double accountBalance, double accountCredit, string accountCompany,
      string accountCurrency, double accountEquity, double accountFreeMargin,
      double accountLeverage, double accountMargin, string accountName,
      int accountNumber, double accountProfit, string accountServer, 
      int operationResult, string operationResultMessage);

void ErrorOccured(string expertID, int operationID, string errorMessage);

void AllOrders(string expertID, string symbol, int operationID, int openCount, int openCustomIDs[], 
      int openTickets[], int closedCount, int closedCustomIDs[], int closedTickets[], int operationResult);

// <<< DATA FUNCTIONS >>>

// If value count is 0, means only requesting quote.
string RequestValues(string expertID); // (int& operationID, int& desiredValueCount, string& symbol)

// Adds an available period to a symbol.
int AddSymbolPeriod(string expertID, string symbol, int period);

// Rates : 
//0 - time,
//1 - open,
//2 - low,
//3 - high,
//4 - close,
//5 - volume.
// OperationId is not mandatory - but is should be there when the update was requested by a special recepient.
void TradingValues(string expertId, string symbol, int operationId, double time, int period, int arrayRatesCount, int requestedValueCount, int availableBarsCount, double rates[]);

// Quotes:
void Quotes(string expertId, string symbol, int operationId, double ask, double bid, double open, double close, 
   double low, double high, double volume, double time);

// Use this to free strings received back.
// void FreeString(string data);

//+------------------------------------------------------------------+
extern string e_serverAddress = "net.tcp://localhost:13123/TradingAPI";
extern bool e_smartValueUpdate = false;//false is slow but correct way
extern int e_updateSpeedMilliseconds = 1000;

string g_commandSeparator = ";";

bool g_statusOK = false;
int g_iteration = 0;   

//+------------------------------------------------------------------+
void Trace(string message, bool isImportant)
{
   if (isImportant)
   {
      Alert(TerminalName() + " ***IMPORTANT*** " + message);
   }
   else
   {
      Print(message);
   }
}

//+------------------------------------------------------------------+  
void ReportError(string message, int operationID)
{
   Alert("***ERROR*** " + message, true);
}

//+------------------------------------------------------------------+
// Currently using  Symbol() + Period() for ID; this is not 100%, as many windows with those 2 can exist.
string GetExpertId()
{
   if (IsTesting())
   {// Testing mode = different ID.
      return (Symbol() + "(Test." + Period() + ")");
   }
   
   return (Symbol()  + "(" + Period() + ")");
}

//+------------------------------------------------------------------+
int init()
{
   Trace("Integrator OFxP [init()].", false);

   g_statusOK = false;
   
   if (InitializeServer(e_serverAddress) == 0)
   {
      Trace("Integrator OFxP initialization failed!", true);
      return (0);
   }
   
   double modePoint = MarketInfo(Symbol(), MODE_POINT);
   double modeDigits = MarketInfo(Symbol(), MODE_DIGITS);
   double modeSpread = MarketInfo(Symbol(), MODE_SPREAD );
   double modeStopLevel = MarketInfo(Symbol(), MODE_STOPLEVEL); 
   double modeLotSize = MarketInfo(Symbol(), MODE_LOTSIZE );
   double modeTickValue = MarketInfo(Symbol(), MODE_TICKVALUE );
   double modeTickSize = MarketInfo(Symbol(), MODE_TICKSIZE );
   double modeSwapLong = MarketInfo(Symbol(), MODE_SWAPLONG );
   double modeSwapShort = MarketInfo(Symbol(), MODE_SWAPSHORT );
   double modeStarting = MarketInfo(Symbol(), MODE_STARTING );
   double modeExpiration = MarketInfo(Symbol(), MODE_EXPIRATION );
   double modeTradeAllowed = MarketInfo(Symbol(), MODE_TRADEALLOWED );
   double modeMinLot = MarketInfo(Symbol(), MODE_MINLOT );
   double modeLotStep = MarketInfo(Symbol(), MODE_LOTSTEP );
   double modeMaxLot = MarketInfo(Symbol(), MODE_MAXLOT );
   double modeSwapType = MarketInfo(Symbol(), MODE_SWAPTYPE );
   double modeProfitCalcMode = MarketInfo(Symbol(), MODE_PROFITCALCMODE );
   double modeMarginCalcMode = MarketInfo(Symbol(), MODE_MARGINCALCMODE );
   double modeMarginInit = MarketInfo(Symbol(), MODE_MARGININIT );
   double modeMarginMaintenance = MarketInfo(Symbol(), MODE_MARGINMAINTENANCE );
   double modeMarginHedged = MarketInfo(Symbol(), MODE_MARGINHEDGED );
   double modeMarginRequired = MarketInfo(Symbol(), MODE_MARGINREQUIRED );
   double modeFreezeLevel = MarketInfo(Symbol(), MODE_FREEZELEVEL );

   Trace("Integrator OFxP [init(), 2].", false);
   string expertId = GetExpertId();
   string symbol = Symbol();
   int period = Period();
  
   Trace("Integrator OFxP [init(), 3].", false);
   
   g_statusOK = InitializeIntegration(expertId, symbol, 
      modePoint, modeDigits, modeSpread, modeStopLevel, modeLotSize, modeTickValue,
      modeTickSize, modeSwapLong, modeSwapShort, modeStarting, modeExpiration,
      modeTradeAllowed, modeMinLot, modeLotStep, modeMaxLot, modeSwapType,
      modeProfitCalcMode, modeMarginCalcMode, modeMarginInit, modeMarginMaintenance,
      modeMarginHedged, modeMarginRequired, modeFreezeLevel);

   AddSymbolPeriod(expertId, symbol, Period());

   Trace("Integrator OFxP [init(), 4].", false);
   
   if (g_statusOK == false)
   {
      ReportError("Initializing integrator failed at expert code " + GetExpertId(), -1);
   }
   else
   {
      Trace("Initializing integrator succeeded.", false);
      HandleDllCalls();
   }

   return(0);
}

//+------------------------------------------------------------------+

int deinit()
{
   if (g_statusOK)
   {
      if ( UnInitializeIntegration(GetExpertId(), Symbol()) == false)
      {
         Trace("Integrator uninitialization error.", false);  
      }
      Trace("Integrator uninitialized.", false);
   }
   else
   {
      Trace("Integrator uninit skipped.", false);
   }
   
   g_statusOK = false;
   
   return (0);
}

//+------------------------------------------------------------------+  
// This is run every time a new tick occurs. Also when statusOK it will run all the time.
int start()
{
   int lastUpdateTime = GetTickCount();
 
   if (IsTesting())
   {//What we do in testing mode is update each time an update comes.
      // In testing mode time runs in a different way - Sleep() does not take effect.
      if (g_statusOK && IsStopped() == false && IsExpertEnabled())
      {
         //int now = GetTickCount();
         //if (now - lastUpdateTime > 5000)
         //lastUpdateTime = now;
         //Trace("[TEST MODE] Integrator running iteration : " + g_iteration, false);
         HandleDllCalls();
         g_iteration++;
      }
      return(0);
   }
   
   if (g_statusOK == false && IsStopped() == false && IsExpertEnabled() && IsConnected())
   {// Try to reinitialize the expert.
      init();
   }
   
   SendAccountInformation(false);
   
   Trace("Integrator OFxP [start()].", false);
   while(g_statusOK && IsStopped() == false && IsExpertEnabled())
   {
      if (IsConnected() == false)
      {// Connection has dropped.
         deinit();
         return;
      }

      //Trace("Integrator running iteration : " + g_iteration, false);
      HandleDllCalls();
      Sleep(e_updateSpeedMilliseconds);
      g_iteration++;
   }

   if (g_statusOK == false)
   {
      if (IsConnected() == false)
      {
         Trace("Expert at " + Symbol() + "." + Period() + "not connected.", false);
      }
      else
      {
         Trace("Expert at " + Symbol() + "." + Period() + "stopped (may be not properly started and not functioning).", false);
      }
   }

   return(0);
}

//+------------------------------------------------------------------+  
//+------------------------------------------------------------------+
// Helper function for parsing command strings, results resides in g_parsedString.
bool SplitString(string stringValue, string separatorSymbol, string& results[], int expectedResultCount = 0)
{
//	 Alert("--SplitString--");
//	 Alert(stringValue);

   if (StringFind(stringValue, separatorSymbol) < 0)
   {// No separators found, the entire string is the result.
      ArrayResize(results, 1);
      results[0] = stringValue;
   }
   else
   {   
      int separatorPos = 0;
      int newSeparatorPos = 0;
      int size = 0;

      while(newSeparatorPos > -1)
      {
         size = size + 1;
         newSeparatorPos = StringFind(stringValue, separatorSymbol, separatorPos);
         
         ArrayResize(results, size);
         if (newSeparatorPos > -1)
         {
            if (newSeparatorPos - separatorPos > 0)
            {// Evade filling empty positions, since 0 size is considered by the StringSubstr as entire string to the end.
               results[size-1] = StringSubstr(stringValue, separatorPos, newSeparatorPos - separatorPos);
            }
         }
         else
         {// Reached final element.
            results[size-1] = StringSubstr(stringValue, separatorPos, 0);
         }
         
         
         //Alert(results[size-1]);
         separatorPos = newSeparatorPos + 1;
      }
   }   
   
   if (expectedResultCount == 0 || expectedResultCount == ArraySize(results))
   {// Results OK.
      return (true);
   }
   else
   {// Results are WRONG.
      Trace("ERROR - size of parsed string not expected.", true);
      return (false);
   }
}

//+------------------------------------------------------------------+  
// Helper function to handle the string parsing.
// Input format ; string& symbol, double& volume, double& prefferedPrice, double& slippage, double& takeProfit, double& stopLoss, int& orderType, int& operationID, string& comment
bool ParseRequestNewOrder(string& symbol, double& volume, double& prefferedPrice, double& slippage, double& takeProfit, double& stopLoss, int& orderType, int& operationID, string& comment)
{
   string tempString = RequestNewOrder(GetExpertId(), Symbol());
   if (StringLen(tempString) == 0)
   {
      return (false);
   }
   
   string results[];
   if (SplitString(tempString, g_commandSeparator, results, 9) == false)
   {
      return (false);   
   }

   symbol = results[0];
   volume = StrToDouble(results[1]);
   prefferedPrice = StrToDouble(results[2]);
   slippage = StrToDouble(results[3]);
   takeProfit = StrToDouble(results[4]);
   stopLoss = StrToDouble(results[5]);
   orderType = StrToInteger(results[6]);
   
   operationID = StrToInteger(results[7]);
   comment = results[8];
   
   // Instruct the external module to free the memory it allocated, as we already used it.
   //FreeString(tempString);
   
   return (true);
}

//+------------------------------------------------------------------+  
// Helper function to handle the string parsing.
// If closing volume is negative, close the entire order.
// Result format : int& orderTicket, int& operationID, double& volume, double price, double slippage
bool ParseRequestCloseOrder(int& orderTicket, int& operationId, double& closingVolume, double& price, double& slippage)
{
   string tempString = RequestCloseOrder(GetExpertId(), Symbol());
   if (StringLen(tempString) == 0)
   {
      return (false);
   }
 
   string results[];
   if (SplitString(tempString, g_commandSeparator, results, 5) == false)
   {
      return (false);
   }
   
   orderTicket = StrToInteger(results[0]);
   operationId = StrToInteger(results[1]);
   closingVolume = StrToDouble(results[2]);   
   price = StrToDouble(results[3]);   
   slippage = StrToDouble(results[4]);   

   // Instruct the external module to free the memory it allocated, as we already used it.
   // FreeString(tempString);
   
   return (true);
}

//+------------------------------------------------------------------+  
// Helper function to handle the string parsing.
// If a parameter is 0, it means leave unchanged. If it is < 0 it means set to "value not assigned".
// targetOpenPrice applies to pending orders only.
// Result format : string symbol, int& orderTicket, int& operationID, double& stopLoss, double takeProfit, double targetOpenPrice, int expiration
bool ParseRequestModifyOrder(string& symbol, int& orderTicket, int& operationId, double& stopLoss, double& takeProfit, double& targetOpenPrice, datetime& expiration)
{
   string tempString = RequestModifyOrder(GetExpertId(), Symbol());
   if (StringLen(tempString) == 0)
   {
      return (false);
   }
 
   string results[];
   if (SplitString(tempString, g_commandSeparator, results, 7) == false)
   {
      return (false);
   }
   
   symbol = results[0];
   orderTicket = StrToInteger(results[1]);
   operationId = StrToInteger(results[2]);
   stopLoss = StrToDouble(results[3]);   
   takeProfit = StrToDouble(results[4]);   
   targetOpenPrice = StrToDouble(results[5]);
   expiration = StrToInteger(results[6]);
   
   // Instruct the external module to free the memory it allocated, as we already used it.
   // FreeString(tempString);
   
   return (true);
}

//+------------------------------------------------------------------+  
bool ParseRequestValues(int& operationID, int& prefferedValueCount, string& symbol)
{
   string tempString = RequestValues(GetExpertId());
   if (StringLen(tempString) == 0)
   {
      return (false);
   }
   
   string results[];
   if (SplitString(tempString, g_commandSeparator, results, 3) == false)
   {
      return (false);
   }

   operationID = StrToInteger(results[0]);
   prefferedValueCount = StrToInteger(results[1]);
   symbol = results[2];

   Trace("Requesting values [" + prefferedValueCount + "].", false);
  
   //int separatorPos = StringFind(tempString, ";");
   //string operationIdString = StringSubstr(tempString, 0, separatorPos);
   //string prefferedValueCountString = StringSubstr(tempString, separatorPos+1, StringLen(tempString) - (separatorPos+1));
   
   //prefferedValueCount = StrToInteger(prefferedValueCountString);
   //operationID = StrToInteger(operationIdString);

   // Instruct the external module to free the memory it allocated, as we already used it.
   // FreeString(tempString);
   
   return (true);
}


//+------------------------------------------------------------------+  
// Helper function to handle the string parsing.
bool ParseRequestOrderInformation(int& operationID, int& orderTicket)
{
   string tempString = RequestOrderInformation(GetExpertId());
   if (StringLen(tempString) == 0)
   {
      return (false);
   }
 
   int separatorPos = StringFind(tempString, ";");
   string orderTicketString = StringSubstr(tempString, 0, separatorPos);
   string operationIdString = StringSubstr(tempString, separatorPos+1, StringLen(tempString) - (separatorPos+1));
   
   orderTicket = StrToInteger(orderTicketString);
   operationID = StrToInteger(operationIdString);

   // Instruct the external module to free the memory it allocated, as we already used it.
   // FreeString(tempString);
   
   return (true);
}

//+------------------------------------------------------------------+  
void SendAllOrders(int operationId)
{
   int totalOpenOrders = OrdersTotal();
   int totalHistoricalOrders = OrdersHistoryTotal();
   
   int operationResult = 1;

   //-- part 1 - open/pending orders. 
   int openOrdersCount = 0;      
   int openOrdersCustomIDs[];
   int openOrdersTickets[];

   // Size to maximum possible size - all orders.
   if (ArrayResize(openOrdersCustomIDs, totalOpenOrders) < 0 || ArrayResize(openOrdersTickets, totalOpenOrders) < 0)
   {
      Trace("General array error", true);
      operationResult = 0;
      return;
   }

   // write open orders
   for(int pos=0; operationResult > 0 && pos < totalOpenOrders; pos++)
   {
      if(OrderSelect(pos,SELECT_BY_POS) == false) 
      {// Failed to select order or order belongs to different symbol - do not touch it.
         Trace("Order select by position error.", true);
         continue;
      }

      if (OrderSymbol() != Symbol())
      {// This expert only works with its symbol orders; to integrate other symbols order must run experts on them too.
         continue;
      }

      openOrdersCustomIDs[openOrdersCount] = OrderMagicNumber();
      openOrdersTickets[openOrdersCount] = OrderTicket();
      openOrdersCount++;
   }

   // ReSize to actual resulting size.
   if (ArrayResize(openOrdersCustomIDs, openOrdersCount) < 0 || ArrayResize(openOrdersTickets, openOrdersCount) < 0)
   {
      Trace("General array error", true);
      operationResult = 0;
   }


   //-- part 2 - historical orders. 
   int historicalOrdersCount = 0;      
   int historicalOrdersCustomIDs[];
   int historicalOrdersTickets[];
   
   // Size to maximum possible size - all orders.
   if (ArrayResize(historicalOrdersCustomIDs, totalHistoricalOrders) < 0 || ArrayResize(historicalOrdersTickets, totalHistoricalOrders) < 0)
   {
      Trace("General array error [2]", true);
      operationResult = 0;
      return;
   }


   // write historical orders
   for(pos=0; operationResult > 0 && pos < totalHistoricalOrders; pos++)
   {
      if(OrderSelect(pos,SELECT_BY_POS, MODE_HISTORY) == false) 
      {// Failed to select order or order belongs to different symbol - do not touch it.
         Trace("Order select by position error.", true);
         continue;
      }

      if (OrderSymbol() != Symbol())
      {// This expert only works with its symbol orders; to integrate other symbols order must run experts on them too.
         continue;
      }

      historicalOrdersCustomIDs[historicalOrdersCount] = OrderMagicNumber();
      historicalOrdersTickets[historicalOrdersCount] = OrderTicket();
      historicalOrdersCount++;
   }

   // ReSize to actual resulting size.
   if (ArrayResize(historicalOrdersCustomIDs, historicalOrdersCount) < 0 || ArrayResize(historicalOrdersTickets, historicalOrdersCount) < 0)
   {
      Trace("General array error [3]", true);
      operationResult = 0;
      return;
   }


   AllOrders(GetExpertId(), Symbol(), operationId, openOrdersCount, openOrdersCustomIDs, openOrdersTickets, historicalOrdersCount, historicalOrdersCustomIDs, historicalOrdersTickets, operationResult);
}

//+------------------------------------------------------------------+  

void HandleAllOrdersRequests()
{
   int operationID = RequestAllOrders(GetExpertId());
   while(operationID != 0)
   {
      SendAllOrders(operationID);
      
      // Try the next one.
      operationID = RequestAllOrders(GetExpertId());
   }
}

//+------------------------------------------------------------------+  

bool IsOrderPending(int orderType)
{
   return (orderType == OP_SELLLIMIT || orderType == OP_SELLSTOP || orderType == OP_BUYLIMIT || orderType == OP_BUYSTOP);
}


//+------------------------------------------------------------------+  

int ToSlippagePoints(string symbol, double value)
{
   int digits = MarketInfo(symbol, MODE_DIGITS);
   int result = (value / MarketInfo(symbol, MODE_POINT));
   
   if (digits == 5)
   {// 5 digit brokers still use the 4th digit as point, when it comes to slippage calculation, so compensate it here.
      result = (result / 10);
   }

   return (result);
   //actualSlippage = inputSlippagePoints * ;
}

//+------------------------------------------------------------------+  
// This fixes the problem with 4/5 digit slippage processing.
int ProcessSlippage(string symbol, int inputSlippagePoints, double& actualSlippage)
{
/*   // Perform slippage compensation.
   double currentPriceDifferencePointsRaw = (MathAbs(prefferedPrice - currentPrice) / MarketInfo(symbol, MODE_POINT));
   //int currentPriceDifferencePoints = ProcessSlippage(symbol, (int)currentPriceDifferencePointsRaw);
   Trace("SendingOrder - compensated slippage points [" + currentPriceDifferencePoints + "]", false);
   
   int appliedCompensatedSlippage = appliedSlippage - currentPriceDifferencePoints;

   int digits = MarketInfo(symbol, MODE_DIGITS);
   actualSlippage = inputSlippagePoints * MarketInfo(symbol, MODE_POINT);
   
   if (digits == 5)
   {// 5 digit brokers still use the 4th digit as point, when it comes to slippage calculation, so compensate it here.
      return (inputSlippagePoints / 10);
   }
      
   return (inputSlippagePoints);
   */
}

//+------------------------------------------------------------------+  

// Helper, will check if the slippage criteria met, as well as process the slippage, to compensate the for 5th digit bugs.
bool PreCheckSlippage(bool isBuy, double currentPrice, double prefferedPrice, double actualSlippage)
{
   double currentSlippage = MathAbs(currentPrice - prefferedPrice);
   
   return (actualSlippage >= currentSlippage);
}

//+------------------------------------------------------------------+  

void HandleNewOrderRequests()
{
   
   int operationId = 0;
   string symbol;
   double volume = 0;
   double prefferedPrice = 0;
   double slippagePoints = 0;
   double takeProfit = 0;
   double stopLoss = 0;
   int orderType = 0;
   int orderTicket = -1;
   double orderPrice = 0;
   string comment;

   string operationResultMessage;

   while(ParseRequestNewOrder(symbol, volume, prefferedPrice, slippagePoints, takeProfit, stopLoss, orderType, operationId, comment))
   {
      Trace("OpeningOrder volume[" + volume + "] prefferedPrice[" + prefferedPrice + "] slippagePoints[" + slippagePoints + "] takeProfit[" + takeProfit + "] stopLoss[" + stopLoss + "] orderType[" + orderType + "] operationId[" + operationId + "] comment[" + comment + "]", false);

      RefreshRates();

      bool sent = false;

      // Negative slippage means, disregard it.
      if (slippagePoints < 0)
      {// Full ask value.
        slippagePoints = MarketInfo(symbol, MODE_ASK) / MarketInfo(symbol, MODE_POINT);
      }

      bool isBuy = (orderType == OP_BUY || orderType == OP_BUYLIMIT || orderType == OP_BUYSTOP);
      if (prefferedPrice <= 0)
      {
         if (orderType == OP_BUY || orderType == OP_BUYLIMIT || orderType == OP_BUYSTOP)
         {
            prefferedPrice = MarketInfo(symbol, MODE_ASK);
         }
         else
         {
            prefferedPrice = MarketInfo(symbol, MODE_BID);
         }
      }

      double currentPrice = MarketInfo(symbol, MODE_ASK);
      if (orderType == OP_SELL)
      {
         currentPrice = MarketInfo(symbol, MODE_BID);
      }


      double slippage = slippagePoints * MarketInfo(symbol, MODE_POINT);
      double currentSlippage = currentPrice - prefferedPrice;
      double compensatedSlippage = slippage - currentSlippage;
      int compensatedSlippagePoints = ToSlippagePoints(symbol, compensatedSlippage);

      double requestMarketPrice = currentPrice;

      if (compensatedSlippagePoints < 0)
      {// Price slipped too much already, so just place the original call instead of the optimized one (since the optimized is now invalid).
         requestMarketPrice = prefferedPrice;
         compensatedSlippagePoints = slippagePoints;
      }

      Trace("SendingOrder orderType[" + orderType + "] volume[" + volume + "] prefferedPrice[" + prefferedPrice + "] currentPrice[" + currentPrice + 
         "] requestSlip[" + slippagePoints + "pts >> " + slippage + "] currentSlip[" + currentSlippage + "] appliedSlip ["+ compensatedSlippage+ " >> " + compensatedSlippagePoints + "pts"
         + "] takeProfit [" + takeProfit + "] stopLoss[" + stopLoss + "]  operationId[" + operationId + "] comment[" + comment + "]", false);

      if (compensatedSlippage < 1)
      {// Evade passing 0 as slippage.
         Trace("SendingOrder [2] Upgrading slippage to 1.", false);
         compensatedSlippagePoints = 1;
      }

      if (IsOrderPending(orderType))
      {// Pending order.
         orderTicket = OrderSend(symbol, orderType, volume, prefferedPrice, ToSlippagePoints(symbol, slippage), stopLoss, takeProfit, comment, GetNextUniqueMagicNumer(), 0, Green);
         sent = true;
      }
      else
      {// Market order.
      
         orderTicket = OrderSend(symbol, orderType, volume, requestMarketPrice, compensatedSlippagePoints, 
            stopLoss, takeProfit, comment, GetNextUniqueMagicNumer(), 0, Green);
            
         sent = true;
      }
      
      double orderOpeningPrice = 0;
      int orderOpeningTime = 0;
      
      if(orderTicket < 0)
      {// Error occured.
         if (sent)
         {
            operationResultMessage = PrintLastErrorDescription();
         }
      }
      else
      {// Order ok.
         if (OrderSelect(orderTicket, SELECT_BY_TICKET))
         {
             orderOpeningPrice = OrderOpenPrice();
             orderOpeningTime = OrderOpenTime();
         }
         
         
         // Usefull to detect unanounced broker slippages, since the MT4 will also report an action here, but with the actual (eventual) price and the difference can be spotted instantly.
         Trace("Order opened Time[" + TimeToStr(orderOpeningTime) + "], Price[" + orderOpeningPrice + "].", false);
      }
      
      OrderOpened(GetExpertId(), symbol, operationId, orderTicket, orderOpeningPrice, orderOpeningTime, orderTicket >= 0, operationResultMessage);
   }// while
}

//+------------------------------------------------------------------+  
// Give a unique magic numer for order.
int GetNextUniqueMagicNumer()
{
/* 
   // The way this was done, it takes the largest order id from all open orders, or if none are open, returns 1.
   int result = 1;
   for(int j=0; j<OrdersTotal(); j++)
   {
      if (OrderSelect(j, SELECT_BY_POS))
      {
         if (OrderTicket() > result)
         {
            result = OrderTicket();
         }
      }
   }
 
   // Verify the result (maybe something went wrong).
   for(j=0; j<OrdersTotal(); j++)
   {
      if (OrderSelect(j, SELECT_BY_POS))
      {
         if (OrderMagicNumber() == result)
         {
            Alert("Error in establishing unique magic numer.");
            result = 0;
         }
      }
   }
   return (result);   
   */
   
   return (TimeCurrent());
}

//+------------------------------------------------------------------+  
/*
int GetOrderTicketByMagicNumer(int magicNumber, bool history)
{
   int pool = MODE_TRADES;
   if (history)
   {
      pool = MODE_HISTORY;
   }
   
   for(int j=0; j<OrdersTotal(); j++)
   {
      if (OrderSelect(j, SELECT_BY_POS, pool))
      {
         if (OrderMagicNumber() == magicNumber)
         {
            return (OrderTicket());
         }
      }
   }
  
   return (-1);
}
*/
//+------------------------------------------------------------------+  
// Helper

double GetCurrentOrderClosingPrice()
{
   RefreshRates();

   int orderType = OrderType();
   if (orderType == OP_BUY || orderType == OP_BUYLIMIT || orderType == OP_BUYSTOP)
   {
      return (MarketInfo(OrderSymbol(), MODE_BID));
      //return (Bid);
   }
   else
   {
      return (MarketInfo(OrderSymbol(), MODE_ASK));
      //return (Ask);
   }
}


//+------------------------------------------------------------------+  

void HandleCloseOrderRequests()
{

   int orderTicket = 0;
   int orderNewTicket = 0;
   int operationId = 0;
   double closingVolume = 0;
   double prefferedPrice = 0;
   double slippagePoints = 0;
   double orderClosedAtPrice = 0;

   while(ParseRequestCloseOrder(orderTicket, operationId , closingVolume, prefferedPrice, slippagePoints))
   {
      Trace("Closing Order ticket[" + orderTicket + "] closingVolume[" + closingVolume + "] prefferedPrice["+ prefferedPrice + "] slippagePts[" + slippagePoints + "]", false);
      bool operationResult = false;
      string operationResultMessage = "Order closed.";
      int orderClosingTime = 0;
      // Is the order fully closed or just modified in volume.
      bool fullOrderClose = true;

      if (OrderSelect(orderTicket, SELECT_BY_TICKET) == false) // Get order by ticket.
      {
         operationResultMessage = PrintLastErrorDescription();
         OrderClosed(GetExpertId(), Symbol(), operationId, orderTicket, orderNewTicket, orderClosedAtPrice, orderClosingTime, operationResult, operationResultMessage);
         continue;
      }
      
/*      
      if (OrderSymbol() != Symbol()) // Verify order symbol as well - we only close current integration orders.
      {
         operationResultMessage = "Unexpected error occured! Order symbol mismatch.";
         Trace(operationResultMessage, true);
         OrderClosed(GetExpertId(), Symbol(), operationId, orderTicket, orderNewTicket, orderClosedAtPrice, orderClosingTime, operationResult, operationResultMessage);
         continue;
      } 
*/
      // All is OK with order - time to close it.
      if (IsOrderPending(OrderType()))
      {// Pending orders get deleted.
      
         operationResult = OrderDelete(orderTicket);
         orderClosingTime = OrderCloseTime();
         
         // Whatever the result - send back a notification regarding it.
         OrderClosed(GetExpertId(), Symbol(), operationId, orderTicket, orderNewTicket, orderClosedAtPrice, orderClosingTime, operationResult, operationResultMessage);
         continue;
      }         
         
      // Close typical order.
      if (closingVolume < 0)
      {// Since there is no proper closing volume specified, set it to complete volume of the order (close order).
         closingVolume = OrderLots();
      }

      double currentClosingPrice = GetCurrentOrderClosingPrice();
      string symbol = OrderSymbol();
   
      // Negative slippage means, disregard it.
      if (slippagePoints < 0)
      {// Full ask value.
         slippagePoints = MarketInfo(symbol, MODE_ASK) / MarketInfo(OrderSymbol(), MODE_POINT);
      }

      if (prefferedPrice <= 0)
      {
         prefferedPrice = currentClosingPrice;
      }

      double slippage = slippagePoints * MarketInfo(symbol, MODE_POINT);
      double currentSlippage = currentClosingPrice - prefferedPrice;
      double compensatedSlippage = slippage - currentSlippage;
      int compensatedSlippagePoints = ToSlippagePoints(symbol, compensatedSlippage);

      double requestMarketPrice = currentClosingPrice;

      if (compensatedSlippagePoints < 0)
      {// Price slipped too much already, so just place the original call instead of the optimized one (since the optimized is now invalid).
         requestMarketPrice = prefferedPrice;
         compensatedSlippagePoints = slippagePoints;
      }

      Trace("ClosingOrder closeVolume[" + closingVolume + "] prefferedPrice[" + prefferedPrice + "] currentClosePrice[" + currentClosingPrice + 
         "] requestSlip[" + slippagePoints + "pts >> " + slippage + "] currentSlip[" + currentSlippage + "] appliedSlip ["+ compensatedSlippage+ " >> " + compensatedSlippagePoints + "pts"
         + "]  operationId[" + operationId + "]", false);


      fullOrderClose = closingVolume == OrderLots();

      if (OrderClose(orderTicket, closingVolume, prefferedPrice, compensatedSlippagePoints, Yellow) == false)
      {// There was a problem with closing the order.
        operationResultMessage = PrintLastErrorDescription();
      }
      else
      {// Order properly closed.
         if (fullOrderClose)
         {
            orderClosedAtPrice = OrderClosePrice();
            orderClosingTime = OrderCloseTime();
            orderNewTicket = OrderTicket();
         }
         else
         {// Signal the order it has been terminated, a new one will come to replace it.
            orderNewTicket = GetOrderTicketByComments(orderTicket);
         }
   
         Trace("Order close at " + orderClosedAtPrice, false);
         operationResult = true;
      }

      // Whatever the result - send back a notification regarding it.
      OrderClosed(GetExpertId(), Symbol(), operationId, orderTicket, orderNewTicket, orderClosedAtPrice, orderClosingTime, operationResult, operationResultMessage);
   }// while

}

//+------------------------------------------------------------------+  
// This is a helper, used to find a new order that came from an old modified one
// (in this the comment looks like this : "from #{oldOrderTicket}";
int GetOrderTicketByComments(int oldOrderTicket)
{
   int totalOpenOrders = OrdersTotal();
   for(int pos=0; pos < totalOpenOrders; pos++)
   {
      if(OrderSelect(pos,SELECT_BY_POS) == false) 
      {// Failed to select order or order belongs to different symbol - do not touch it.
         Trace("Order select by position error.", true);
         continue;
      }
/*
      if (OrderSymbol() != Symbol())
      {
         continue;
      }
*/
      string comment = OrderComment();
      int stringPosition = StringFind(comment, DoubleToStr(oldOrderTicket, 0), 0);
      if (stringPosition > 1 && stringPosition < 8)
      {// Found it, this is our new order.
         Trace("Order found : " + OrderTicket(), false);
         return (OrderTicket());
      }
   }

   Trace("Order (by comments) not found [" + oldOrderTicket + "].", true);
   return (-1);
}

//+------------------------------------------------------------------+  

void HandleModifyOrderRequests()
{
   string symbol = "";
   int orderTicket = 0;
   int orderNewTicket = 0;
   int operationId = 0;
   double stopLoss = 0;
   double takeProfit = 0;
   double targetOpenPrice = 0;
   datetime expiration = 0;

   while(ParseRequestModifyOrder(symbol, orderTicket, operationId , stopLoss, takeProfit, targetOpenPrice, expiration))
   {
      Trace("ParseRequestModifyOrder" + orderTicket + "; " + operationId + "; " + stopLoss + "; " + takeProfit + "; " + targetOpenPrice + "; " + expiration, false);

      bool operationResult = false;
      string operationResultMessage = "Order modified.";
      if (OrderSelect(orderTicket, SELECT_BY_TICKET) == false) // Get order by ticket.
      {
         operationResultMessage = PrintLastErrorDescription();
      }
      else if (OrderSymbol() != symbol) // Verify order symbol as well - we only close current integration orders.
      {
         operationResultMessage = "Unexpected error occured! Order symbol mismatch [" + symbol + ", " + OrderSymbol() + "].";
      } 
      else
      {// All is OK with order - time to close it.
      
        
         if (stopLoss == 0)
         {// This means do not change, so we shall convert it to the current value.
            stopLoss = OrderStopLoss();
         }
         else if (stopLoss < 0)
         {// < 0 means assign a "not value" so set 0 for the MT4 to read it this way.
            stopLoss = 0;
         }
            
         if (takeProfit == 0)
         {// This means do not change, so we shall convert it to the current value.
            takeProfit = OrderTakeProfit();
         }
         else if (takeProfit < 0)
         {// < 0 means assign a "not value" so set 0 for the MT4 to read it this way.
            takeProfit = 0;
         }
      
         if (targetOpenPrice == 0)
         {// This means do not change, so we shall convert it to the current value.
            targetOpenPrice = OrderOpenPrice();
         }
         else if (targetOpenPrice < 0)
         {// < 0 means assign a "not value" so set 0 for the MT4 to read it this way.
            targetOpenPrice = 0;
         }
         
         if (expiration == 0)
         {// This means do not change, so we shall convert it to the current value.
            expiration = OrderExpiration();
         }
         else if (expiration < 0)
         {// < 0 means assign a "not value" so set 0 for the MT4 to read it this way.
            expiration = 0;
         }

         if (OrderModify(orderTicket, targetOpenPrice, stopLoss, takeProfit, expiration) == false)
         {// There was a problem with modifying the order.
            operationResultMessage = PrintLastErrorDescription();
         }         
	      else
	      {// Order properly modified.
	         operationResult = true;
	      }
      }
      
      // Modifications of this type always leave the ID the same so just return it.
      orderNewTicket = orderTicket;
            
      // Whatever the result - send back a notification regarding it.
      OrderModified(GetExpertId(), Symbol(), operationId, orderTicket, orderNewTicket, operationResult, operationResultMessage);
   
   }// while
}

//+------------------------------------------------------------------+  
/*
void HandleCloseAllOrdersRequest()
{
   int operationID = RequestCloseAllOrders(GetExpertId());
   if(operationID == 0)
   {
      return;
   }
   
   int closedOrdersCount = 0;
   
   int totalOrders = OrdersTotal();
   for(int j=0; j<5 && totalOrders > 0; j++)
   {// Retry a few times to close all orders.
      totalOrders = OrdersTotal();  
      for(int pos=0; pos < totalOrders; pos++)
      {
        if(OrderSelect(pos, SELECT_BY_POS)) 
        {
          if (OrderSymbol() != Symbol())
          {// This order belongs to different integration - do not touch it.
            continue;
          }
          
          
          double orderPrice = GetCurrentOrderClosingPrice();

          if ( OrderClose(OrderTicket(), OrderLots(), orderPrice, 12, Yellow) == false)
          {
             int errorCode = GetLastError();
             //ReportError("OrderClose has failed with error #" + errorCode + ":" + PrintErrorDescription(errorCode), operationID);
          }
          else
          {
            closedOrdersCount++;
          }
        }
        
        Sleep(250);
      }
   }
   
   AllOrdersClosed(GetExpertId(), operationID, closedOrdersCount, 1, PrintErrorDescription(errorCode));
}
*/

datetime g_lastDataHistoryPeriod = 0;
//+------------------------------------------------------------------+  
void SendTradingValues(bool ratesWereRefreshed)
{
   int operationId = -1, prefferedValueCount = 0;
   string symbol;
   
   if (ParseRequestValues(operationId, prefferedValueCount, symbol) == false)
   {
      operationId = -1;
   }
   else
   {// A specific symbol was requested.
      if (Symbol() != symbol)
      {
         ReportError("Symbol " + symbol + " not supported.", operationId);
         return;
      }
   }
   
   if (ratesWereRefreshed || e_smartValueUpdate == false)
   {
      Quotes(GetExpertId(), Symbol(), operationId, Ask, Bid, Open[0], Close[0], Low[0], High[0], Volume[0], TimeCurrent());
      
      if (g_lastDataHistoryPeriod != Time[0])
      {// Send updates of data history every time a new bar is formed.
         SendTradingValuesHistory(operationId, 20);
         g_lastDataHistoryPeriod = Time[0];
      }
      
      if(IsTesting())
      {
        operationId = -1;
      }

   }

   if (operationId == -1)
   {// No more work to do here.
      return;
   }

   if (prefferedValueCount == 0)
   {// Only quote requested.
      Quotes(GetExpertId(), Symbol(), operationId, Ask, Bid, Open[0], Close[0], Low[0], High[0], Volume[0], TimeCurrent());
      return;
   }

   SendTradingValuesHistory(operationId, prefferedValueCount);

}

void SendTradingValuesHistory(int operationId, int prefferedValueCount)
{
   // PrefferedValueCount > 0 && OperationId != -1 ...

   // Notes: This rates array is normally used to pass data to a DLL function.
   // Memory is not really allocated for data array, and no real copying is performed. When such an array is accessed, the access will be redirected.
   double ratesArray[][6];
   int arrayCopyResultCount = ArrayCopyRates(ratesArray);
   if (arrayCopyResultCount == -1)
   {// Error - copying went wrong.
      ReportError("Failed to copy needed data, communication with DLL failed. Error is #" + arrayCopyResultCount + ", " + PrintErrorDescription(arrayCopyResultCount), -1);
      return;
   }

   if (prefferedValueCount > arrayCopyResultCount)
   {// Make sure we return a max of what is available.
      prefferedValueCount = arrayCopyResultCount;
   }

   // We shall create an array copy. The reason for this is the array we are provided 
   // with in ArrayCopyRates is strange and does not work properly when exported to DLL;
   // it is a pseudo-array that gets allocated only after accessed and causes problems.
   // So copy it and pass it over properly.
   double ratesArrayCopy[][6];
   
   int resizeResult = ArrayResize(ratesArrayCopy, arrayCopyResultCount);
   if (resizeResult != 6 * arrayCopyResultCount)
   {
      ReportError("ERROR - There is size difference at array resize. Size provided was : " + resizeResult + " / " + (6 * arrayCopyResultCount), -1);
      return;
   }

   //Trace(resizeResult, true);

   // Now we copy the portion we are interested in.
   // COPY the entire thing over, so that it can be properly processed outside, since here
   // it IS A MESS, this dynamic array !!! Do no try to manipulate detailed here...
   int copiedResult = ArrayCopy(ratesArrayCopy, ratesArray, 0, 0, resizeResult);
   if (copiedResult == 0)
   {
      ReportError("ERROR - Copying error.", -1);
      return;
   }
   
   //Trace(DoubleToStr(copiedResult, 0) + ", "  + DoubleToStr(prefferedValueCount, 0) + " of " + DoubleToStr(arrayCopyResultCount, 0), true);

   // Now pass the entire array to the middle DLL, as well as how many to extract from it.
   TradingValues(GetExpertId(), Symbol(), operationId, TimeCurrent(), Period(), 
      arrayCopyResultCount, prefferedValueCount, ArraySize(ratesArrayCopy), ratesArrayCopy);
}

//+------------------------------------------------------------------+  
void SendAccountInformation(bool ratesWereRefreshed)
{
   int operationID = RequestAccountInformation(GetExpertId());
   if(operationID == 0)
   {
      operationID = -1;
   }
   
   if (operationID < 1 && ratesWereRefreshed == false)
   {// No request, no rateRefresh
      return;
   }

   double accountBalance = AccountBalance(); 
   double accountCredit = AccountCredit();
   string accountCompany = AccountCompany();
   string accountCurrency = AccountCurrency();
   double accountEquity = AccountEquity();
   double accountFreeMargin = AccountFreeMargin();
   double accountLeverage = AccountLeverage();
   double accountMargin = AccountMargin();
   string accountName = AccountName();
   int accountNumber = AccountNumber();
   double accountProfit = AccountProfit();
   string accountServer = AccountServer();

   int operationResult = 1;
  
   AccountInformation(GetExpertId(), operationID,
        accountBalance, accountCredit, accountCompany,
        accountCurrency, accountEquity, accountFreeMargin,
        accountLeverage, accountMargin, accountName,
        accountNumber, accountProfit, accountServer, 
        operationResult, "");
}

//+------------------------------------------------------------------+  

void HandleOrderInformationRequests()
{
   int orderTicket = 0;
   int totalOperationID = -1;	
   int operationID = 0;
   
   // Pump all requests at once, to speed up answering of many order information requests.
   while(ParseRequestOrderInformation(operationID, orderTicket))
   {
      if (totalOperationID > -1 && operationID != totalOperationID)
      {// Make sure we do not mix operations.
         Alert("Operation ID mixup on Orders informations request.");
      }
      
      totalOperationID = operationID;
      
      int operationResult = 1;
      string operationResultMessage = "Order information retrieved.";
      if (OrderSelect(orderTicket, SELECT_BY_TICKET) == false)
      {// Failed to select order.
            int error = GetLastError();
            //ReportError("Failed to find an order with this ticket:" + orderTicket, operationID);
            operationResult = 0;
            operationResultMessage = "Failed to find an order with this ticket.";
      }

      string orderSymbol = OrderSymbol();
      double orderLots = OrderLots();
      double orderOpenPrice = OrderOpenPrice();
      double orderClosePrice = OrderClosePrice();
      double orderStopLoss = OrderStopLoss();
      double orderTakeProfit = OrderTakeProfit();
      double orderProfit = OrderProfit();
      double orderSwap = OrderSwap();
      int orderOpenTime = OrderOpenTime();
      int orderCloseTime = OrderCloseTime(); 
      int orderExpiration = OrderExpiration();
      double orderCommission = OrderCommission();
      string orderComment = OrderComment(); 
      int orderMagicNumber = OrderMagicNumber();
      int orderType = OrderType();

      OrderInformation(GetExpertId(), Symbol(), operationID, orderTicket, orderSymbol, orderType, orderLots, 
	      orderOpenPrice, orderClosePrice, orderStopLoss, orderTakeProfit, 
	      orderProfit, orderSwap, orderOpenTime, 
	      orderCloseTime, orderExpiration, orderCommission,
	      orderComment, orderMagicNumber, operationResult, operationResultMessage);
   }

   //if (totalOperationID > -1)
   //{// If anything was requested and sent.   
      // After all the information has been transfered tell the session to send the responding message (flush) - send all empty values, with order ticket == -1.
      //OrderInformation(GetExpertId(), Symbol(), totalOperationID, -1, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", 0, true, "");
   //}
}


//+------------------------------------------------------------------+  

void HandleDllCalls()
{
   if (Bars < 1)
   {
      Trace("No data will be sent to the integrated DLL as no data was found.", false);
      return;
   }

   // In testing mode we need all the updates we can get, since we do not cycle.
   bool ratesRefreshed = RefreshRates() ||  IsTesting();

   SendTradingValues(ratesRefreshed);
   SendAccountInformation(ratesRefreshed);
   if (ratesRefreshed)
   {
      SendAllOrders(-1);
   }
     
   HandleNewOrderRequests();
   HandleCloseOrderRequests();
   HandleModifyOrderRequests();
   HandleOrderInformationRequests();
   HandleAllOrdersRequests();
}


//+------------------------------------------------------------------+
// Helper

string PrintLastErrorDescription()
{
   return (PrintErrorDescription(GetLastError()));
}

//+------------------------------------------------------------------+
// Helper

string PrintErrorDescription(int error_code)
  {
   string error_string;
//----
   switch(error_code)
     {
      //---- codes returned from trade server
      case 0:   error_string ="no value";                                                 break;
      case 1:   error_string="no error";                                                  break;
      case 2:   error_string="common error";                                              break;
      case 3:   error_string="invalid trade parameters";                                  break;
      case 4:   error_string="trade server is busy";                                      break;
      case 5:   error_string="old version of the client terminal";                        break;
      case 6:   error_string="no connection with trade server";                           break;
      case 7:   error_string="not enough rights";                                         break;
      case 8:   error_string="too frequent requests";                                     break;
      case 9:   error_string="malfunctional trade operation (never returned error)";      break;
      case 64:  error_string="account disabled";                                          break;
      case 65:  error_string="invalid account";                                           break;
      case 128: error_string="trade timeout";                                             break;
      case 129: error_string="invalid price";                                             break;
      case 130: error_string="invalid stops";                                             break;
      case 131: error_string="invalid trade volume";                                      break;
      case 132: error_string="market is closed";                                          break;
      case 133: error_string="trade is disabled";                                         break;
      case 134: error_string="not enough money";                                          break;
      case 135: error_string="price changed";                                             break;
      case 136: error_string="off quotes";                                                break;
      case 137: error_string="broker is busy (never returned error)";                     break;
      case 138: error_string="requote";                                                   break;
      case 139: error_string="order is locked";                                           break;
      case 140: error_string="long positions only allowed";                               break;
      case 141: error_string="too many requests";                                         break;
      case 145: error_string="modification denied because order too close to market";     break;
      case 146: error_string="trade context is busy";                                     break;
      case 147: error_string="expirations are denied by broker";                          break;
      case 148: error_string="amount of open and pending orders has reached the limit";   break;
      //---- mql4 errors
      case 4000: error_string="no error (never generated code)";                                                 break;
      case 4001: error_string="wrong function pointer";                                   break;
      case 4002: error_string="array index is out of range";                              break;
      case 4003: error_string="no memory for function call stack";                        break;
      case 4004: error_string="recursive stack overflow";                                 break;
      case 4005: error_string="not enough stack for parameter";                           break;
      case 4006: error_string="no memory for parameter string";                           break;
      case 4007: error_string="no memory for temp string";                                break;
      case 4008: error_string="not initialized string";                                   break;
      case 4009: error_string="not initialized string in array";                          break;
      case 4010: error_string="no memory for array\' string";                             break;
      case 4011: error_string="too long string";                                          break;
      case 4012: error_string="remainder from zero divide";                               break;
      case 4013: error_string="zero divide";                                              break;
      case 4014: error_string="unknown command";                                          break;
      case 4015: error_string="wrong jump (never generated error)";                       break;
      case 4016: error_string="not initialized array";                                    break;
      case 4017: error_string="dll calls are not allowed";                                break;
      case 4018: error_string="cannot load library";                                      break;
      case 4019: error_string="cannot call function";                                     break;
      case 4020: error_string="expert function calls are not allowed";                    break;
      case 4021: error_string="not enough memory for temp string returned from function"; break;
      case 4022: error_string="system is busy (never generated error)";                   break;
      case 4050: error_string="invalid function parameters count";                        break;
      case 4051: error_string="invalid function parameter value";                         break;
      case 4052: error_string="string function internal error";                           break;
      case 4053: error_string="some array error";                                         break;
      case 4054: error_string="incorrect series array using";                             break;
      case 4055: error_string="custom indicator error";                                   break;
      case 4056: error_string="arrays are incompatible";                                  break;
      case 4057: error_string="global variables processing error";                        break;
      case 4058: error_string="global variable not found";                                break;
      case 4059: error_string="function is not allowed in testing mode";                  break;
      case 4060: error_string="function is not confirmed";                                break;
      case 4061: error_string="send mail error";                                          break;
      case 4062: error_string="string parameter expected";                                break;
      case 4063: error_string="integer parameter expected";                               break;
      case 4064: error_string="double parameter expected";                                break;
      case 4065: error_string="array as parameter expected";                              break;
      case 4066: error_string="requested history data in update state";                   break;
      case 4099: error_string="end of file";                                              break;
      case 4100: error_string="some file error";                                          break;
      case 4101: error_string="wrong file name";                                          break;
      case 4102: error_string="too many opened files";                                    break;
      case 4103: error_string="cannot open file";                                         break;
      case 4104: error_string="incompatible access to a file";                            break;
      case 4105: error_string="no order selected";                                        break;
      case 4106: error_string="unknown symbol";                                           break;
      case 4107: error_string="invalid price parameter for trade function";               break;
      case 4108: error_string="invalid ticket";                                           break;
      case 4109: error_string="trade is not allowed in the expert properties";            break;
      case 4110: error_string="longs are not allowed in the expert properties";           break;
      case 4111: error_string="shorts are not allowed in the expert properties";          break;
      case 4200: error_string="object is already exist";                                  break;
      case 4201: error_string="unknown object property";                                  break;
      case 4202: error_string="object is not exist";                                      break;
      case 4203: error_string="unknown object type";                                      break;
      case 4204: error_string="no object name";                                           break;
      case 4205: error_string="object coordinates error";                                 break;
      case 4206: error_string="no specified subwindow";                                   break;
      default:   error_string="unknown error";
     }
//----
   return(error_string);
  }
  
  
//+------------------------------------------------------------------+
// Helper function.
/*
void DoCloseOrder(int orderPosition, int orderTicket, int orderTagId)
{
   int errorCode = 0;

   // Get order by ticket.
   if (orderTicket != 0 && OrderSelect(orderTicket, SELECT_BY_TICKET, MODE_TRADES) == false) ||
      (orderPosition != 0 && OrderSelect(orderPosition,SELECT_BY_POS) == false) )
   {
      ReportError("Order to close was not found.");
      OrderClosed(GetExpertId(), orderTicket, orderTagId, false);
   } 
  
   
   if ( OrderClose(OrderTicket(), OrderLots(), Bid, ClosingSlippage, Yellow) == false)
   {
    errorCode = GetLastError();
    ReportError("OrderClose has failed with error #" + errorCode + ":" + PrintErrorDescription(errorCode));
   }
   else
   {
   closedOrdersCount++;
   }
   }
   Sleep(250);
   }
   
   if (orderTagId != 0 && OrderMagicNumber() != orderTagId) // Verify magic number, if it exists.
   {
      ReportError("Order to close was not found - magic number mistmatch.");
      OrderClosed(GetExpertId(), orderTicket, orderTagId, false);
   }
   else
   {// All is OK with order - time to close it.
   
      if (OrderClose(orderTicket, OrderLots(), OrderClosePrice(), ClosingSlippage, Yellow) == false)
      {// There was a problem with closing the order.
         errorCode = GetLastError();
         ReportError("OrderClose failed with error #" + errorCode + ":" + PrintErrorDescription(errorCode));
         OrderClosed(GetExpertId(), orderTicket, orderTagId, false);
      }
      else
      {// Order properly closed.
         OrderClosed(GetExpertId(), orderTicket, orderTagId, true);
      }
   }
}
*/