#pragma once

#include "GACHelper.h"

public ref class AdapterMediator
{
protected:

  static AdapterMediator^ Mediator = nullptr;

public:

  static AdapterMediator^ GetMediator()
  {
    return Mediator!=nullptr ? Mediator : (Mediator = gcnew AdapterMediator());
  }

public:

  static MT4Adapter::MT4RemoteAdapter^ GetAdapter()
  {
    AdapterMediator^ Mediator = GetMediator();

    return Mediator!=nullptr ? Mediator->GetAdapterInstance() : nullptr;
  }


protected:

  System::Runtime::InteropServices::GCHandle gcHandle;

  HANDLE _mutexHandle;

  bool _initialized;

public:

  AdapterMediator()
    :_mutexHandle(0)
    ,_initialized(false)
    ,gcHandle()
  {
    this->_mutexHandle = CreateMutex(0, FALSE, 0);
    this->_initialized = false;
  }

  ~AdapterMediator()
  {
    MT4Adapter::MT4RemoteAdapter^ adapter = GetAdapter();

    if (adapter != nullptr)
    {
      adapter->UnInitialize();
    }

    if(this->_mutexHandle!=0)
    {
      CloseHandle(this->_mutexHandle);

      this->_mutexHandle = 0;
    }
    this->gcHandle.Free();
  }

public:

  void InitializeServer(System::String^ serverAddress)
  {
    WaitForSingleObject(this->_mutexHandle, INFINITE);

    if (_initialized)
    {// Already allocated.

      MT4Adapter::MT4RemoteAdapter^ Adapter = this->GetAdapterInstance();

      if(Adapter!=nullptr)
      {
        System::Uri^ uri = Adapter->ServerIntegrationUri;

        if(uri!=nullptr)
        {
          if (System::String::Compare(serverAddress, uri->ToString()) != 0)
          {
            System::String^ message = System::String::Format(
              "Conflicting server addresses in experts on the same MT4 instance found {0} [initial: {1}, passed: {2}].{0}Set them all the same and restart the MT4.", System::Environment::NewLine, GetAdapter()->ServerIntegrationUri->ToString(), serverAddress);
           
            System::Windows::Forms::MessageBox::Show(message, "MT4 OFxP Expert Error");
          }
        }
      }
    }
    else
    {
      _initialized = true;

      System::Diagnostics::Trace::WriteLine("AdapterMediator::InitializeServer [1]");

      if (System::String::IsNullOrEmpty(serverAddress))
      {// Assign the default value.
        System::Windows::Forms::MessageBox::Show("Assigned a server on default address.", "MT4 OFxP Expert Warning");

        serverAddress = "net.tcp://localhost:13123/TradingAPI";
      }

      System::Diagnostics::Trace::WriteLine("AdapterMediator::InitializeServer [2]");

      System::Uri^ serverIntegrationUri = gcnew System::Uri(serverAddress);

      if(serverIntegrationUri!=nullptr)
      {
        MT4Adapter::MT4RemoteAdapter^ integrationAdapter = gcnew MT4Adapter::MT4RemoteAdapter(serverIntegrationUri);

        if(integrationAdapter!=nullptr)
        {
          this->gcHandle = System::Runtime::InteropServices::GCHandle::Alloc(integrationAdapter);
        }
      }
    }

    ReleaseMutex(this->_mutexHandle);

    System::Diagnostics::Trace::WriteLine("AdapterMediator::InitializeServer [3]");
  }


  MT4Adapter::MT4RemoteAdapter^ GetAdapterInstance()
  {
    MT4Adapter::MT4RemoteAdapter^ result = nullptr;

    if(System::Runtime::InteropServices::GCHandle::ToIntPtr(this->gcHandle)!=System::IntPtr::Zero)
    {
      WaitForSingleObject(this->_mutexHandle, INFINITE);

      result = static_cast<MT4Adapter::MT4RemoteAdapter^>(this->gcHandle.Target);

      ReleaseMutex(this->_mutexHandle);
    }

    return result;
  }

};
