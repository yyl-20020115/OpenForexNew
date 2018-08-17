using System.Windows.Forms;
using ForexPlatform;
using System;
using CommonFinancial;
using CommonSupport;
using Arbiter;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace ForexPlatformFrontEnd
{
  public partial class RemoteExpertHostForm : Form
  {
    protected ProxyIntegrationAdapterClient client = null;

    protected RemoteExpertHost expertHost = null;

    protected Platform platform = null;

    protected string message = null;

    protected Type expertType = null;

    public PlatformManagedExpert PlatformManagedExpert
    {
      get
      {
        return this.expertHost != null ? this.expertHost.PlatformManagedExpert : null;
      }
    }

    public virtual string PlatformUri
    {
      get
      {
        return this.URLTextBox.Text;
      }
      set
      {
        this.URLTextBox.Text = value;
      }
    }

    public virtual string ExpertName
    {
      get
      {
        return this.ExpertNameTextBox.Text;
      }
      set
      {
        this.ExpertNameTextBox.Text = value;
      }
    }


    public virtual string Message
    {
      get
      {
        return this.MessageToolStripStatusLabel.Text;
      }
      set
      {
        this.MessageToolStripStatusLabel.Text = value;
      }
    }

    public virtual string SessionName
    {
      get
      {
        return this.SessionNameTextBox.Text;
      }
      set
      {
        this.SessionNameTextBox.Text = value;
      }
    }

    public virtual string SymbolName
    {
      get
      {
        return this.SymbolsComboBox.Text;
      }
      set
      {
        this.SymbolsComboBox.Text = value;
      }
    }

    public virtual decimal LotSize
    {
      get
      {
        decimal r = 0m;

        if(decimal.TryParse(this.LotTextBox.Text, out r))
        {

        }
        return r;
      }
      set
      {
        this.LotTextBox.Text = value.ToString();
      }

    }

    public int DecimalPlaces
    {
      get
      {
        int r = 0;

        if (int.TryParse(this.DecimalPlacesTextBox.Text, out r))
        {

        }
        return r;
      }
      set
      {
        this.DecimalPlacesTextBox.Text = value.ToString();
      }
    }

    protected ComponentId clientID = ComponentId.Empty;

    protected Guid sessionGuid = Guid.Empty;

    /// <summary>
    /// 
    /// </summary>
    public RemoteExpertHostForm(string platformUri, Type expertType, string expertName)
    {
      this.InitializeComponent();

      this.SymbolsComboBox.SelectedIndex = 0;

      this.SessionCheckBox.Enabled = false;

      this.PlatformUri = string.IsNullOrEmpty(platformUri) ? Properties.Resources.DefaultURI : platformUri;

      this.expertType = expertType ?? typeof(AIPlatformManagedExpert);

      this.ExpertName = expertName ?? this.expertType.Name;

      this.platform = new Platform("DefaultPlatform");

    }

    protected override void OnClosed(EventArgs e)
    {
      if (this.expertHost != null)
      {
        this.expertHost.UnInitialize();
        this.expertHost = null;
      }
      if (this.client != null)
      {
        this.SessionCheckBox.Enabled = false;

        if (this.platform != null)
        {
          this.platform.UnRegisterSource(SourceTypeEnum.HighPriorityLiveFullProvider, this.client.ProxyTransportInfo);
        }

        this.client.Stop(out this.message);
        this.client = null;
      }

      if (this.platform != null)
      {
        this.platform = null;
      }


      base.OnClosed(e);

    }
    ~RemoteExpertHostForm()
    {

    }
    protected virtual void ConnectCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      if (this.ConnectCheckBox.Checked)
      {
        if (this.client == null)
        {
          this.client = new ProxyIntegrationAdapterClient(new Uri(this.PlatformUri));

          if (this.client.Start(this.platform, out this.message))
          {
            this.UpdateStatus();

            this.client.OperationalStateChangedEvent += new OperationalStateChangedDelegate(client_OperationalStateChangedEvent);

          }
        }
      }
      else
      {
        if (this.client != null && this.clientID!=ComponentId.Empty)
        {
          this.SessionCheckBox.Enabled = false;
          this.SessionCheckBox.Checked = false;
          this.URLTextBox.Enabled = true;
          this.ExpertNameTextBox.Enabled = true;

          this.clientID = ComponentId.Empty;

          if (this.platform != null)
          {
            this.platform.UnRegisterSource(SourceTypeEnum.HighPriorityLiveFullProvider, this.client.ProxyTransportInfo);
          }
          this.client.Stop(out this.message);
          this.UpdateStatus();
          this.client = null;
        }
      }
    }

    protected virtual void client_OperationalStateChangedEvent(IOperational operational, OperationalStateEnum previousOperationState)
    {
      lock (this)
      {
        if(this.client!=null && !this.SessionCheckBox.Enabled)
        {
          if (this.client.IsOperational)
          {
            this.ConnectCheckBox.Invoke(new SetControlBoolValue(this.SetControlChecked), this.ConnectCheckBox, true);

            TransportInfo ti = this.client.ProxyTransportInfo;

            if(this.platform.RegisterSource(
              new SourceInfo(
                 SourceTypeEnum.HighPriorityLiveFullProvider,
                ti)
                ))
            {

              this.Message = "Client Proxy connected";

              this.clientID = ti.OriginalSenderId.Value.Id;
              this.client.OperationalStateChangedEvent -= new OperationalStateChangedDelegate(client_OperationalStateChangedEvent);

              this.Invoke(new SetControlBoolValue(this.SetControlEnable), this.URLTextBox, false);
              this.Invoke(new SetControlBoolValue(this.SetControlEnable), this.ExpertNameTextBox, false);
              this.Invoke(new SetControlBoolValue(this.SetControlEnable), this.SessionCheckBox, true);

            }
          }
        }
 
      }
    }
    protected delegate void SetControlBoolValue(Control c, bool value);
    protected delegate void SetControlIntValue(Control c, int value);

    protected virtual void SetControlEnable(Control c, bool enabled)
    {
      if(c!=null)
      {
        c.Enabled = enabled;
      }
    }
    protected virtual void SetControlChecked(Control c, bool value)
    {
      if (c != null)
      {
        CheckBox check = c as CheckBox;

        if(check!=null)
        {
          check.Checked = value;
        }
      }
    }
    protected virtual void SetProgressValue(Control c, int value)
    {
      if (c != null)
      {
        ProgressBar progress = c as ProgressBar;

        if (progress != null)
        {
          progress.Value = value;
        }
      }
    }
    protected virtual void SetProgressLimit(Control c, int value)
    {
      if (c != null)
      {
        ProgressBar progress = c as ProgressBar;

        if (progress != null)
        {
          progress.Maximum = value;
        }
      }
    }

    protected virtual void SessionCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      if(this.SessionCheckBox.Checked)
      {
        if(this.expertHost==null)
        {
          if (platform.RegisterComponent(this.expertHost = new RemoteExpertHost(this.ExpertName, this.expertType)))
          {

            ISessionDataProvider provider = this.expertHost.CreateRemoteExpertSession(
              new DataSessionInfo(
                this.sessionGuid = Guid.NewGuid(),
                this.SessionName, 
                new Symbol(this.SymbolName),
                this.LotSize,
                this.DecimalPlaces
                ),
              this.clientID
            );

            if (provider != null)
            {
              provider.ObtainDataBarProvider(TimeSpan.FromMinutes(1));

              this.SessionNameTextBox.Enabled = false;
              this.SymbolsComboBox.Enabled = false;
              this.LotTextBox.Enabled = false;
              this.DecimalPlacesTextBox.Enabled = false;
              this.Message = "Session Connected";
            }
          }
        }
      }
      else
      {
        if(this.expertHost!=null)
        {
          this.expertHost.UnInitialize();
          this.expertHost = null;

          this.Message = "Session Lost";
        }
        this.SessionNameTextBox.Enabled = true;
        this.SymbolsComboBox.Enabled = true;
        this.LotTextBox.Enabled = true;
        this.DecimalPlacesTextBox.Enabled = true;

      }
    }

    protected virtual void UpdateStatus()
    {
      this.Message = this.message;
    }

    protected virtual void LotTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      decimal r = 0;
      if (!decimal.TryParse(this.LotTextBox.Text, out r))
      {
        e.Cancel = true;
      }
    }
    protected virtual void DecimalPlacesTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int r = 0;
      if (!int.TryParse(this.DecimalPlacesTextBox.Text, out r))
      {
        e.Cancel = true;
      }
    }
  }
}
