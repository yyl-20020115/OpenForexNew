#include "Mediator.h"

#define EXPORTED __declspec(dllexport)

void TraceEntry(const char* methodName, const char* parameter)
{
  methodName = methodName == 0 ? "" : methodName;
  parameter = parameter == 0 ? "" : parameter;

  System::String^ methodString = gcnew System::String(methodName);

  System::Diagnostics::Trace::Write(System::String::Format(methodString->Concat(methodString, gcnew System::String("::{0}")), gcnew System::String(parameter)));
}

const char* MarshalStringToUnmanaged(System::String^ inputString)
{
  if(inputString!=nullptr)
  {
    System::IntPtr strPtr = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(inputString);

    return (const char*)strPtr.ToPointer();
  }
  return 0;
}

// Helper, static
//NOTICE: the returned string will be freed by MetaTrader, no need to free again!
//void MarshalFreeString(const char* data)
//{
//  if(data!=0)
//  {
//    System::IntPtr strPtr((void*)data);
//    System::Runtime::InteropServices::Marshal::FreeHGlobal(strPtr);
//  }
//}
// Not currently used, could be used as a way to release strings passed back to the MT4 expert in case there is a proven memory leak in those.
//EXPORTED void __stdcall FreeString(const char* data)
//{
//  if(data!=0)
//  {
//    MarshalFreeString(data);
//  }
//}


EXPORTED int __stdcall InitializeServer(const char* serverAddress)
{// Since there will be multiple calls to this (from multiple experts in same DLL), 

  TraceEntry("InitializeServer", serverAddress);

  bool queryResult = GACHelper::QueryAssembly("Arbiter");

  queryResult = queryResult && GACHelper::QueryAssembly("MT4Adapter");

  if (queryResult == false)
  {
    System::Windows::Forms::MessageBox::Show("MT4 Initialization failed. Requrired assemblies not found in GAC.", "MT4 OFxP Expert Error");
    return 0;
  }

  // Only the first valid one will be considered.
  AdapterMediator::GetMediator()->InitializeServer(gcnew System::String(serverAddress));

  return 1;
}


// This is called once for each expert.
EXPORTED int __stdcall InitializeIntegration(const char* expertID, const char* symbol,
                                             double modePoint, double modeDigits, double modeSpread, double modeStopLevel, double modeLotSize, double modeTickValue,
                                             double modeTickSize, double modeSwapLong, double modeSwapShort, double modeStarting, double modeExpiration,
                                             double modeTradeAllowed, double modeMinLot, double modeLotStep, double modeMaxLot, double modeSwapType,
                                             double modeProfitCalcMode, double modeMarginCalcMode, double modeMarginInit, double modeMarginMaintenance,
                                             double modeMarginHedged, double modeMarginRequired, double modeFreezeLevel)
{
  TraceEntry("InitializeIntegration", expertID);

  MT4Adapter::MT4RemoteAdapter^ Adapter = AdapterMediator::GetAdapter();

  if(Adapter!=nullptr)
  {
    return Adapter->InitializeIntegrationSession(
      gcnew System::String(symbol), 
      (System::Decimal)(System::Double)modePoint,(System::Decimal)(System::Double) modeDigits, (System::Decimal)(System::Double)modeSpread, (System::Decimal)(System::Double)modeStopLevel, (System::Decimal)(System::Double)modeLotSize, (System::Decimal)(System::Double)modeTickValue,
      (System::Decimal)(System::Double)modeTickSize, (System::Decimal)(System::Double)modeSwapLong,(System::Decimal)(System::Double) modeSwapShort,(System::Decimal)(System::Double) modeStarting,(System::Decimal)(System::Double) modeExpiration,
      (System::Decimal)(System::Double)modeTradeAllowed, (System::Decimal)(System::Double)modeMinLot,(System::Decimal)(System::Double) modeLotStep, (System::Decimal)(System::Double)modeMaxLot,(System::Decimal)(System::Double) modeSwapType,
      (System::Decimal)(System::Double)modeProfitCalcMode,(System::Decimal)(System::Double) modeMarginCalcMode, (System::Decimal)(System::Double)modeMarginInit,(System::Decimal)(System::Double) modeMarginMaintenance,
      (System::Decimal)(System::Double) modeMarginHedged, (System::Decimal)(System::Double)modeMarginRequired, (System::Decimal)(System::Double)modeFreezeLevel);
  }

  return 0;
}

// This is called once for each expert.
EXPORTED int __stdcall AddSymbolPeriod(const char* expertID, const char* symbol, int period)
{
  TraceEntry("AddSymbolPeriod", expertID);

  MT4Adapter::MT4RemoteAdapter^ Adapter = AdapterMediator::GetAdapter();

  if(Adapter!=nullptr)
  {
    return Adapter->AddSessionPeriod(gcnew System::String(symbol), period);
  }

  return 0;
}

// This is called once for each expert.
EXPORTED int __stdcall UnInitializeIntegration(const char* expertID, const char* symbol)
{
  TraceEntry("UnInitializeIntegration", expertID);

  MT4Adapter::MT4RemoteAdapter^ Adapter = AdapterMediator::GetAdapter();

  if(Adapter!=nullptr)
  {
  	//Adapter->UnInitialize();
  }
  return 1;
}


// Reference parameters can not be read or written to at all!  We need to use strings. 
EXPORTED int __stdcall RequestAllOrders(const char* expertID) //(int& operationID > 0)
{
  TraceEntry("RequestAllOrders", expertID);
  MT4Adapter::MT4RemoteAdapter^ Adapter = AdapterMediator::GetAdapter();

  if(Adapter!=nullptr)
  {
  //return Adapter->RequestAllOrders();
  }
  return 0;
}

// Reference parameters can not be read or written to at all!  We need to use strings. 
EXPORTED const char* __stdcall RequestModifyOrder(const char* expertID, const char* symbol) //(int& orderTicket, int& operationID)
{
  TraceEntry("RequestModifyOrder", expertID);

  MT4Adapter::MT4RemoteAdapter^ Adapter = AdapterMediator::GetAdapter();

  if(Adapter!=nullptr)
  {
    return MarshalStringToUnmanaged(Adapter->RequestModifyOrder());
  }
  return 0;
}


// Reference parameters can not be read or written to at all! We need to use return strings.
//(double& amount...
EXPORTED const char* __stdcall RequestNewOrder(const char* expertID, const char* symbol) 
{
  TraceEntry("RequestNewOrder", expertID);

  MT4Adapter::MT4RemoteAdapter^ Adapter = AdapterMediator::GetAdapter();

  if(Adapter!=nullptr)
  {
    return MarshalStringToUnmanaged(Adapter->RequestNewOrder());
  }
  return 0;
}

// Reference parameters can not be read or written to at all!  We need to use strings. 
EXPORTED const char* __stdcall RequestOrderInformation(const char* expertID)
{
  TraceEntry("RequestOrderInformation", expertID);

  MT4Adapter::MT4RemoteAdapter^ Adapter = AdapterMediator::GetAdapter();

  if(Adapter!=nullptr)
  {
    return MarshalStringToUnmanaged(Adapter->RequestOrderInformation());
  }
  return 0;
}

// Reference parameters can not be read or written to at all!  We need to use strings. 
EXPORTED const char* __stdcall RequestCloseOrder(const char* expertID, const char* symbol) //(int& orderTicket, int& operationID ...)
{
  TraceEntry("RequestCloseOrder", expertID);

  MT4Adapter::MT4RemoteAdapter^ Adapter = AdapterMediator::GetAdapter();

  if(Adapter!=nullptr)
  {
    return MarshalStringToUnmanaged(Adapter->RequestCloseOrder());
  }
  return 0;
}

EXPORTED const char* __stdcall RequestValues(const char* expertID) // operationId; int (preffered count)
{
  TraceEntry("RequestValues", expertID);

  MT4Adapter::MT4RemoteAdapter^ Adapter = AdapterMediator::GetAdapter();

  if(Adapter!=nullptr)
  {
    return MarshalStringToUnmanaged(Adapter->RequestValues(gcnew System::String(expertID)));
  }
  return 0;
}

EXPORTED void __stdcall OrderOpened(const char* expertID, const char* symbol, int operationID, int orderTicket, double openingPrice, int orderOpenTime, int operationResult, const char* operationResultMessage)
{
  TraceEntry("OrderOpened", expertID);

  MT4Adapter::MT4RemoteAdapter^ Adapter = AdapterMediator::GetAdapter();

  if(Adapter!=nullptr)
  {
    Adapter->OrderOpened(gcnew System::String(symbol), operationID, orderTicket, (System::Decimal)(System::Double)openingPrice, orderOpenTime, operationResult != 0, gcnew System::String(operationResultMessage));
  }
}

EXPORTED void __stdcall OrderClosed(const char* expertID, const char* symbol, int operationID, int orderTicket, int orderNewTicket, double closingPrice, int orderCloseTime, int operationResult, const char* operationResultMessage)
{
  TraceEntry("OrderClosed", expertID);

  MT4Adapter::MT4RemoteAdapter^ Adapter = AdapterMediator::GetAdapter();

  if(Adapter!=nullptr)
  {
    Adapter->OrderClosed(
      gcnew System::String(symbol), 
      operationID, 
      orderTicket, 
    orderNewTicket,
    (System::Decimal)(System::Double)closingPrice,
    orderCloseTime, 
    operationResult != 0,  
    gcnew System::String(operationResultMessage)
    );
  }
}

EXPORTED void __stdcall OrderModified(const char* expertID, const char* symbol, int operationID, int orderTicket, int orderNewTicket, int operationResult, const char* operationResultMessage)
{
  TraceEntry("OrderModified", expertID);

  MT4Adapter::MT4RemoteAdapter^ Adapter = AdapterMediator::GetAdapter();

  if(Adapter!=nullptr)
  {
    Adapter->OrderModified(
      gcnew System::String(symbol), 
      operationID, 
      orderTicket, 
    orderNewTicket, 
    operationResult != 0,
    gcnew System::String(operationResultMessage)
    );
  }
}

EXPORTED void __stdcall AllOrders(const char*  expertID, const char* symbol, int operationID, 
                                  int openCount, const int* openCustomIDs, const int* openTickets, 
                                  int historicalCount, const int* historicalCustomIDs, const int* historicalTickets, 
                                  int operationResult)
{
  TraceEntry("AllOrders", expertID);

  // Copy over the data into proper managed arrays.
  array<System::Int32>^ openManagedCustomIDs = gcnew array<System::Int32>(openCount);
  array<System::Int32>^ openManagedTickets = gcnew array<System::Int32>(openCount);

  for(int i = 0;  i < openCount; i++)
  {
    openManagedCustomIDs[i] = openCustomIDs[i];
    openManagedTickets[i] = openTickets[i];		
  }

  // Copy over the data into proper managed arrays.
  array<System::Int32>^ historicalManagedCustomIDs  =gcnew array<System::Int32>(historicalCount);
  array<System::Int32>^ historicalManagedTickets =gcnew array<System::Int32>(historicalCount);

  for(int i = 0;  i < historicalCount; i++)
  {
    historicalManagedCustomIDs[i] = historicalCustomIDs[i];
    historicalManagedTickets[i] = historicalTickets[i];		
  }
  MT4Adapter::MT4RemoteAdapter^ Adapter = AdapterMediator::GetAdapter();

  if(Adapter!=nullptr)
  {
    Adapter->AllOrders(
      operationID, 
      gcnew System::String(symbol),
    openManagedCustomIDs, 
    openManagedTickets, 
    historicalManagedCustomIDs,
    historicalManagedTickets, 
    operationResult != 0);
  }
}


EXPORTED void __stdcall ErrorOccured(const char* expertID, int operationID, const char* errorMessage)
{
  TraceEntry("ErrorOccured", expertID);

  MT4Adapter::MT4RemoteAdapter^ Adapter = AdapterMediator::GetAdapter();

  if(Adapter!=nullptr)
  {
    Adapter->ErrorOccured(operationID, gcnew System::String(errorMessage));
  }
}

EXPORTED void __stdcall OrderInformation(const char* expertID, const char* symbol, int operationID, int orderTicket, const char* orderSymbol, int orderType, double volume, 
                                         double openPrice, double closePrice, double orderStopLoss, double orderTakeProfit, 
                                         double currentProfit, double orderSwap, int orderPlatformOpenTime, 
                                         int orderPlatformCloseTime, int orderExpiration, double orderCommission,
                                         const char* orderComment, int orderCustomID, int operationResult, const char* operationResultMessage)
{
  TraceEntry("OrderInformation", expertID);

  MT4Adapter::MT4RemoteAdapter^ Adapter = AdapterMediator::GetAdapter();

  if(Adapter!=nullptr)
  {
    Adapter->OrderInformation(
      orderTicket,
      operationID, 
      gcnew System::String( orderSymbol), 
      orderType, 
      (System::Decimal)(System::Double)volume, 
    (System::Decimal)(System::Double) openPrice, 
    (System::Decimal)(System::Double)closePrice,
    (System::Decimal)(System::Double)orderStopLoss,
    (System::Decimal)(System::Double) orderTakeProfit, 
    (System::Decimal)(System::Double)currentProfit,
    (System::Decimal)(System::Double)orderSwap, orderPlatformOpenTime, 
    orderPlatformCloseTime,
    orderExpiration,
    (System::Decimal)(System::Double)orderCommission,
    gcnew System::String(orderComment), 
    orderCustomID,
    operationResult != 0, 
    gcnew System::String(operationResultMessage)
    );
  }
}

EXPORTED void __stdcall Quotes(const char* expertId, const char* symbol, int operationId, double ask, double bid, 
                               double open, double close, double low, double high, double volume, double time)
{
  TraceEntry("Quotes", expertId);

  MT4Adapter::MT4RemoteAdapter^ Adapter = AdapterMediator::GetAdapter();

  if(Adapter!=nullptr)
  {
    Adapter->Quotes(gcnew System::String(symbol), operationId, ask, bid, open, close, low, high, volume, time);
  }
}

EXPORTED void __stdcall TradingValues(const char* expertID, const char* symbol, int operationId, double time, int period,
                                      int arrayRatesCount, int requestedValueCount, int availableBarsCount, const double* rates)
{
  TraceEntry("TradingValues", expertID);

  // Copy over the data into proper managed arrays.
  array<System::Int64>^ timesCopy = gcnew array<System::Int64>(requestedValueCount);
  array<System::Decimal>^ opensCopy = gcnew array<System::Decimal>(requestedValueCount);
  array<System::Decimal>^closesCopy =gcnew array<System::Decimal>(requestedValueCount);
  array<System::Decimal>^highsCopy  =gcnew array<System::Decimal>(requestedValueCount);
  array<System::Decimal>^ lowsCopy  =gcnew array<System::Decimal>(requestedValueCount);
  array<System::Decimal>^volumesCopy = gcnew array<System::Decimal>(requestedValueCount);

  try
  {
    for	(int i = 0; i < requestedValueCount; i++)
    {// There are 6 elements in each set, one after the other.
      // Actually the data in DateTime is kept in 4 bytes (according to docs),
      // but all here seems to be 8B.
      int rateIndex = i + arrayRatesCount - requestedValueCount;

      timesCopy[i] = (System::Int64)*(long long*)&rates[rateIndex * 6];
      opensCopy[i] = (System::Decimal)(System::Double)rates[rateIndex * 6 + 1];
      lowsCopy[i] = (System::Decimal)(System::Double)rates[rateIndex * 6 + 2];
      highsCopy[i] = (System::Decimal)(System::Double)rates[rateIndex * 6 + 3];
      closesCopy[i] = (System::Decimal)(System::Double)rates[rateIndex * 6 + 4];
      volumesCopy[i] = (System::Decimal)(System::Double)rates[rateIndex * 6 + 5];
    }
  }
  catch(System::Exception^ ex)
  {
    System::Diagnostics::Trace::Write(
      System::String::Format(
      System::String::Concat("TradingValues", gcnew System::String("::{0}")), ex->Message)
      );
  }
  MT4Adapter::MT4RemoteAdapter^ Adapter = AdapterMediator::GetAdapter();

  if(Adapter!=nullptr)
  {
    Adapter->TradingValuesUpdate( 
      gcnew System::String(symbol), 
      operationId,
      time,
      period, 
      availableBarsCount, 
      timesCopy,
      opensCopy, 
      closesCopy,
      highsCopy,
      lowsCopy, 
      volumesCopy
      );
  }
}

EXPORTED void __stdcall AccountInformation(const char* expertID, int operationID,
                                           double accountBalance, double accountCredit, const char* accountCompany,
                                           const char* accountCurrency, double accountEquity, double accountFreeMargin,
                                           double accountLeverage, double accountMargin, const char* accountName,
                                           int accountNumber, double accountProfit, const char* accountServer, 
                                           int operationResult, const char* operationResultMessage)
{
  TraceEntry("AccountInformation", expertID);

  MT4Adapter::MT4RemoteAdapter^ Adapter = AdapterMediator::GetAdapter();

  if(Adapter!=nullptr)
  {
    Adapter->AccountInformation(
      operationID, 
      (System::Decimal)(System::Double)accountBalance, 
      (System::Decimal)(System::Double)accountCredit, 
      gcnew System::String( accountCompany), 
      gcnew System::String(accountCurrency), 
      (System::Decimal)(System::Double) accountEquity,
      (System::Decimal)(System::Double) accountFreeMargin,
      (System::Decimal)(System::Double) accountLeverage, 
      (System::Decimal)(System::Double)accountMargin, 
      gcnew System::String(accountName), accountNumber,
      (System::Decimal)(System::Double)accountProfit,
      gcnew System::String(accountServer), 
      operationResult != 0, 
      gcnew System::String(operationResultMessage)
      );
  }
}

EXPORTED int __stdcall RequestAccountInformation(const char* expertID)
{
  TraceEntry("RequestAccountInformation", expertID);

  MT4Adapter::MT4RemoteAdapter^ Adapter = AdapterMediator::GetAdapter();

  if(Adapter!=nullptr)
  {
    return Adapter->RequestAccountInformation();
  }
  return 0;
}

