#include "gachelper.h"

// Include only this, if GacHelper not included.
//#include <windows.h>

// Adding those confuses the Linker and results in errors, so no STL!
//#include <vector>
//#include <string>

#define EXPORTED __declspec(dllexport)

//ALL MT4 EXPORTS NEED TO BE DECLARED IN THE DEF FILE AS WELL !!
#pragma push_macro("new")
#undef new

// Since many experts of the same type will be talking simultaniously to this DLL, it has to have sessioning, thread protection etc.
// The way to distinguish one expert instance from another is trough the ExpertID.

class ManagerMediator
{
	System::Runtime::InteropServices::GCHandle gcHandle;
	HANDLE _mutexHandle;

	bool _initialized;

public:
	ManagerMediator()
	{
		_mutexHandle = CreateMutex(0, FALSE, 0);
		_initialized = false;
	}

	~ManagerMediator()
	{
		MT4Adapter::IntegrationMT4Server* manager = GetManager();

		if (manager != NULL)
		{
			manager->UnInitialize();
		}

		CloseHandle(_mutexHandle);
		_mutexHandle = NULL;
		gcHandle.Free();
	}

	void InitializeServer(System::String* serverAddress)
	{
		WaitForSingleObject(_mutexHandle, INFINITE);

		if (_initialized)
		{// Already allocated.
			if (System::String::Compare(serverAddress, GetManager()->ServerIntegrationUri->ToString()) != 0)
			{
				System::String* message = System::String::Format("Conflicting server addresses in experts on the same MT4 instance found {0} [initial: {1}, passed: {2}].{0}Set them all the same and restart the MT4.", System::Environment::NewLine, GetManager()->ServerIntegrationUri->ToString(), serverAddress);
				System::Windows::Forms::MessageBox::Show(message, "MT4 OFxP Expert Error");
			}
			ReleaseMutex(_mutexHandle);
			return;
		}

		_initialized = true;

		System::Diagnostics::Trace::WriteLine("ManagerMediator::InitializeServer [1]");
		if (System::String::IsNullOrEmpty(serverAddress))
		{// Assign the default value.
			System::Windows::Forms::MessageBox::Show("Assigned a server on default address.", "MT4 OFxP Expert Warning");
			serverAddress = "net.tcp://localhost:13123/TradingAPI";
		}

		System::Diagnostics::Trace::WriteLine("ManagerMediator::InitializeServer [2]");
		System::Uri* serverIntegrationUri = new System::Uri(serverAddress);
		MT4Adapter::IntegrationMT4Server* integrationManager = new MT4Adapter::IntegrationMT4Server(serverIntegrationUri);
		gcHandle = System::Runtime::InteropServices::GCHandle::Alloc(integrationManager);

		ReleaseMutex(_mutexHandle);
		System::Diagnostics::Trace::WriteLine("ManagerMediator::InitializeServer [3]");
	}

	MT4Adapter::IntegrationMT4ServerSession* GetSession(System::String* expertID)
	{
		WaitForSingleObject(_mutexHandle, INFINITE);

		MT4Adapter::IntegrationMT4Server* manager = static_cast<MT4Adapter::IntegrationMT4Server*>(gcHandle.Target);
		MT4Adapter::IntegrationMT4ServerSession* result = manager->GetSessionById(expertID);
		
		ReleaseMutex(_mutexHandle);

		if (result == NULL)
		{
			System::Diagnostics::Trace::WriteLine("ManagerMediator::GetSession - *ERROR* SESSION NOT FOUND; Using dummy session.");
			return manager->get_DummySession();
		}
		else
		{
			return result;
		}
	}

	MT4Adapter::IntegrationMT4Server* GetManager()
	{
		WaitForSingleObject(_mutexHandle, INFINITE);
		
		MT4Adapter::IntegrationMT4Server* result = static_cast<MT4Adapter::IntegrationMT4Server*>(gcHandle.Target);
		
		ReleaseMutex(_mutexHandle);
		return result;
	}

	// Static
	const char* MarshalStringToUnmanaged(System::String* inputString)
	{
		System::IntPtr strPtr = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(inputString);
		return (const char*)strPtr.ToPointer();
	}

	// Static
	void MarshalFreeString(const char* data)
	{
		System::IntPtr strPtr((void*)data);
		System::Runtime::InteropServices::Marshal::FreeHGlobal(strPtr);
	}

};

ManagerMediator ManagerMediator;

// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
void TraceEntry(const char* methodName, const char* parameter)
{
	System::String* methodString = new System::String(methodName);
	System::Diagnostics::Trace::Write(System::String::Format(methodString->Concat(methodString, new System::String("::{0}")), new System::String(parameter)));
}

// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//void Trace(const char* methodName, const char* message, const char* parameter)
//{
//	System::String* methodString = new System::String(methodName);
//	System::String* messageString = new System::String(message);
//
//	System::Diagnostics::Trace::Write(System::String::Format(methodString->Concat(methodString, new System::String("::{0}")), new System::String(parameter)));
//}

//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//EXPORTED int __stdcall InitializeServer(const char* serverAddress)
//{// Since there will be multiple calls to this (from multiple experts in same DLL), 
//	TraceEntry("InitializeServer", serverAddress);
//	//System::Diagnostics::Trace:: Write("InitializeServer::");
//	//System::Diagnostics::Trace::WriteLine(serverAddress);
//
//	bool queryResult = GACHelper::QueryAssembly("Arbiter");
//	queryResult = queryResult && GACHelper::QueryAssembly("MT4Adapter");
//
//	if (queryResult == false)
//	{
//		System::Windows::Forms::MessageBox::Show("MT4 Initialization failed. Requrired assemblies not found in GAC.", "MT4 OFxP Expert Error");
//		return 0;
//	}
//
//	// Only the first valid one will be considered.
//	ManagerMediator.InitializeServer(serverAddress);
//
//	return 1;
//}
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//// Not currently used, could be used as a way to release strings passed back to the MT4 expert in case there is a proven memory leak in those.
//EXPORTED void __stdcall FreeString(const char* data)
//{
//	ManagerMediator.MarshalFreeString(data);
//}
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//// This is called once for each expert.
//EXPORTED int __stdcall InitializeIntegration(const char* expertID, const char* symbol, int period,
//double modePoint, double modeDigits, double modeSpread, double modeStopLevel, double modeLotSize, double modeTickValue,
//double modeTickSize, double modeSwapLong, double modeSwapShort, double modeStarting, double modeExpiration,
//double modeTradeAllowed, double modeMinLot, double modeLotStep, double modeMaxLot, double modeSwapType,
//double modeProfitCalcMode, double modeMarginCalcMode, double modeMarginInit, double modeMarginMaintenance,
//double modeMarginHedged, double modeMarginRequired, double modeFreezeLevel)
//{
//	TraceEntry("InitializeIntegration", expertID);
//	//System::Diagnostics::Trace::Write("InitializeIntegration::");
//
//	return ManagerMediator.GetManager()->InitializeIntegrationSession(expertID, symbol, period,
//      modePoint, modeDigits, modeSpread, modeStopLevel, modeLotSize, modeTickValue,
//      modeTickSize, modeSwapLong, modeSwapShort, modeStarting, modeExpiration,
//      modeTradeAllowed, modeMinLot, modeLotStep, modeMaxLot, modeSwapType,
//      modeProfitCalcMode, modeMarginCalcMode, modeMarginInit, modeMarginMaintenance,
//      modeMarginHedged, modeMarginRequired, modeFreezeLevel);
//}
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//// This is called once for each expert.
//EXPORTED int __stdcall UnInitializeIntegration(const char* expertID)
//{
//	TraceEntry("UnInitializeIntegration", expertID);
//	return ManagerMediator.GetManager()->UnInitializeIntegrationSession(expertID);
//}
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//// Reference parameters can not be read or written to at all! We need to use return strings.
////(double& amount...
//EXPORTED const char* __stdcall RequestNewOrder(const char* expertID) 
//{
//	TraceEntry("RequestNewOrder", expertID);
//
//	return ManagerMediator.MarshalStringToUnmanaged(ManagerMediator.GetSession(expertID)->RequestNewOrder());
//}
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//// Reference parameters can not be read or written to at all!  We need to use strings. 
//EXPORTED const char* __stdcall RequestOrderInformation(const char* expertID)
//{
//	TraceEntry("RequestOrderInformation", expertID);
//	return ManagerMediator.MarshalStringToUnmanaged(ManagerMediator.GetSession(expertID)->RequestOrderInformation());
//}
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//// Reference parameters can not be read or written to at all!  We need to use strings. 
//EXPORTED const char* __stdcall RequestCloseOrder(const char* expertID) //(int& orderTicket, int& operationID ...)
//{
//	TraceEntry("RequestCloseOrder", expertID);
//	return ManagerMediator.MarshalStringToUnmanaged(ManagerMediator.GetSession(expertID)->RequestCloseOrder());
//}
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//// Reference parameters can not be read or written to at all!  We need to use strings. 
//EXPORTED const char* __stdcall RequestModifyOrder(const char* expertID) //(int& orderTicket, int& operationID)
//{
//	TraceEntry("RequestModifyOrder", expertID);
//	return ManagerMediator.MarshalStringToUnmanaged(ManagerMediator.GetSession(expertID)->RequestModifyOrder());
//}
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//// Reference parameters can not be read or written to at all!  We need to use strings. 
//EXPORTED int __stdcall RequestAllOrders(const char* expertID) //(int& operationID > 0)
//{
//	TraceEntry("RequestAllOrders", expertID);
//	return ManagerMediator.GetSession(expertID)->RequestAllOrders();
//}
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//EXPORTED int __stdcall RequestCloseAllOrders(const char* expertID)
//{
//	TraceEntry("RequestCloseAllOrders", expertID);
//	return ManagerMediator.GetSession(expertID)->RequestCloseAllOrders();
//}
//
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//EXPORTED const char* __stdcall RequestValues(const char* expertID) // operationId; int (preffered count)
//{
//	TraceEntry("RequestValues", expertID);
//	return ManagerMediator.MarshalStringToUnmanaged(ManagerMediator.GetSession(expertID)->RequestValues());
//}
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//EXPORTED void __stdcall OrderOpened(const char* expertID, int operationID, int orderTicket, double openingPrice, int orderOpenTime, int operationResult, const char* operationResultMessage)
//{
//	TraceEntry("OrderOpened", expertID);
//	ManagerMediator.GetSession(expertID)->OrderOpened(operationID, orderTicket, openingPrice, orderOpenTime, operationResult != 0, operationResultMessage);
//}
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//EXPORTED void __stdcall OrderClosed(const char* expertID, int operationID, int orderTicket, int orderNewTicket, double closingPrice, int orderCloseTime, int operationResult, const char* operationResultMessage)
//{
//	TraceEntry("OrderClosed", expertID);
//	ManagerMediator.GetSession(expertID)->OrderClosed(operationID, orderTicket, 
//		orderNewTicket, closingPrice, orderCloseTime, operationResult != 0, operationResultMessage);
//}
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//EXPORTED void __stdcall OrderModified(const char* expertID, int operationID, int orderTicket, int orderNewTicket, int operationResult, const char* operationResultMessage)
//{
//	TraceEntry("OrderModified", expertID);
//	ManagerMediator.GetSession(expertID)->OrderModified(operationID, orderTicket, 
//		orderNewTicket, operationResult != 0, operationResultMessage);
//}
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//EXPORTED void __stdcall AllOrders(const char*  expertID, int operationID, 
//								  int openCount, const int* openCustomIDs, const int* openTickets, 
//								  int historicalCount, const int* historicalCustomIDs, const int* historicalTickets, 
//								  int operationResult)
//{
//	TraceEntry("AllOrders", expertID);
//	
//	// Copy over the data into proper managed arrays.
//	System::Int32 openManagedCustomIDs __gc[] = new System::Int32 __gc[openCount];
//	System::Int32 openManagedTickets __gc[] = new System::Int32 __gc[openCount];
//	
//	for(int i = 0;  i < openCount; i++)
//	{
//		openManagedCustomIDs[i] = openCustomIDs[i];
//		openManagedTickets[i] = openTickets[i];		
//	}
//
//	// Copy over the data into proper managed arrays.
//	System::Int32 historicalManagedCustomIDs __gc[] = new System::Int32 __gc[historicalCount];
//	System::Int32 historicalManagedTickets __gc[] = new System::Int32 __gc[historicalCount];
//	
//	for(int i = 0;  i < historicalCount; i++)
//	{
//		historicalManagedCustomIDs[i] = historicalCustomIDs[i];
//		historicalManagedTickets[i] = historicalTickets[i];		
//	}
//
//	ManagerMediator.GetSession(expertID)->AllOrders(operationID, 
//		openManagedCustomIDs, openManagedTickets, 
//		historicalManagedCustomIDs, historicalManagedTickets, 
//		operationResult != 0);
//}
//
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//EXPORTED void __stdcall AllOrdersClosed(const char* expertID, int operationID, int closedOrdersCount, int operationResult, const char* operationResultMessage)
//{
//	TraceEntry("AllOrdersClosed", expertID);
//
//	ManagerMediator.GetSession(expertID)->AllOrdersClosed(operationID, closedOrdersCount, operationResult != 0, operationResultMessage);
//}
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//EXPORTED void __stdcall ErrorOccured(const char* expertID, int operationID, const char* errorMessage)
//{
//	TraceEntry("ErrorOccured", expertID);
//	
//	ManagerMediator.GetSession(expertID)->ErrorOccured(operationID, errorMessage);
//}
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//EXPORTED void __stdcall OrderInformation(const char* expertID, int operationID, int orderTicket, const char* orderSymbol, int orderType, double volume, 
//										 double openPrice, double closePrice, double orderStopLoss, double orderTakeProfit, 
//										 double currentProfit, double orderSwap, int orderPlatformOpenTime, 
//										 int orderPlatformCloseTime, int orderExpiration, double orderCommission,
//										 const char* orderComment, int orderCustomID, int operationResult, const char* operationResultMessage)
//{
//	TraceEntry("OrderInformation", expertID);
//	
//	ManagerMediator.GetSession(expertID)->OrderInformation(orderTicket, operationID, orderSymbol, orderType, volume, 
//										 openPrice, closePrice, orderStopLoss, orderTakeProfit, 
//										 currentProfit, orderSwap, orderPlatformOpenTime, 
//										 orderPlatformCloseTime, orderExpiration, orderCommission,
//										 orderComment, orderCustomID, operationResult != 0, operationResultMessage);
//}
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//EXPORTED void __stdcall Quotes(const char* expertId, int operationId, double ask, double bid, 
//								   double open, double close, double low, double high, double volume, double time)
//{
//	TraceEntry("Quotes", expertId);
//
//	ManagerMediator.GetSession(expertId)->Quotes(operationId, ask, bid, open, close, low, high, volume, time);
//}
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//EXPORTED void __stdcall TradingValues(const char* expertID, int operationId, double time, 
//									  int arrayRatesCount, int requestedValueCount, int availableBarsCount, const double* rates)
//{
//	TraceEntry("TradingValues", expertID);
//
//	//requestedValueCount = arrayRatesCount;
//
//	// Copy over the data into proper managed arrays.
//	System::Int64 timesCopy __gc[] = new System::Int64 __gc[requestedValueCount];
//	System::Decimal opensCopy __gc[] = new System::Decimal __gc[requestedValueCount];
//	System::Decimal closesCopy __gc[] = new System::Decimal __gc[requestedValueCount];
//	System::Decimal highsCopy __gc[] = new System::Decimal __gc[requestedValueCount];
//	System::Decimal lowsCopy __gc[] = new System::Decimal __gc[requestedValueCount];
//	System::Decimal volumesCopy __gc[] = new System::Decimal __gc[requestedValueCount];
//
//	//int startingIndex = 0;
//	//System::Diagnostics::Trace::Write(System::String::Format(System::String::Concat("TradingValues", new System::String("::{0}")), 
//	//	requestedValueCount.ToString()));
//	//System::Diagnostics::Trace::Write(System::String::Format(System::String::Concat("TradingValues", new System::String("::{0}")), 
//	//	arrayRatesCount.ToString()));
//	//System::Diagnostics::Trace::Write(System::String::Format(System::String::Concat("TradingValues", new System::String("::{0}")), 
//	//	availableBarsCount.ToString()));
//
//	try
//	{
//		//for	(int i = arrayRatesCount - 1; i >= 0 && i >= arrayRatesCount - requestedValueCount; i--)
//		for	(int i = 0; i < requestedValueCount; i++)
//		{// There are 6 elements in each set, one after the other.
//			// Actually the data in DateTime is kept in 4 bytes (according to docs),
//			// but all here seems to be 8B.
//			int rateIndex = i + arrayRatesCount - requestedValueCount;
//			timesCopy[i] = (System::Int64)rates[rateIndex * 6];
//			opensCopy[i] = rates[rateIndex * 6 + 1];
//			lowsCopy[i] = rates[rateIndex * 6 + 2];
//			highsCopy[i] = rates[rateIndex * 6 + 3];
//			closesCopy[i] = rates[rateIndex * 6 + 4];
//			volumesCopy[i] = rates[rateIndex * 6 + 5];
//		}
//	}
//	catch(System::Exception* ex)
//	{
//		System::Diagnostics::Trace::Write(System::String::Format(
//			System::String::Concat("TradingValues", new System::String("::{0}")), ex->get_Message()));
//	}
//
//	ManagerMediator.GetSession(expertID)->TradingValuesUpdate(operationId, time, availableBarsCount, 
//		timesCopy, opensCopy, closesCopy, highsCopy, lowsCopy, volumesCopy);
//}
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//
//EXPORTED void __stdcall AccountInformation(const char* expertID, int operationID,
//        double accountBalance, double accountCredit, const char* accountCompany,
//        const char* accountCurrency, double accountEquity, double accountFreeMargin,
//        double accountLeverage, double accountMargin, const char* accountName,
//        int accountNumber, double accountProfit, const char* accountServer, 
//		int operationResult, const char* operationResultMessage)
//{
//	TraceEntry("AccountInformation", expertID);
//
//	ManagerMediator.GetSession(expertID)->AccountInformation(operationID, accountBalance, accountCredit, 
//            accountCompany, accountCurrency, 
//            accountEquity, accountFreeMargin, accountLeverage, 
//            accountMargin, accountName, accountNumber,
//            accountProfit, accountServer, operationResult != 0, operationResultMessage);
//}
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
//
//EXPORTED int __stdcall RequestAccountInformation(const char* expertID)
//{
//	TraceEntry("RequestAccountInformation", expertID);
//
//	return ManagerMediator.GetSession(expertID)->RequestAccountInformation();
//}
//
//// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 


#pragma pop_macro("new")