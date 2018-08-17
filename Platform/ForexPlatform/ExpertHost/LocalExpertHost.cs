using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using Arbiter;
using CommonSupport;
using CommonFinancial;

namespace ForexPlatform
{
  /// <summary>
  /// Expert host has a tripple role:
  /// (local mode) - It can host an expert on the local arbiter in the platform execution process
  /// </summary>
  [Serializable]
  [ComponentManagement(false, false, int.MaxValue, true)]
  public class LocalExpertHost : ExpertHost
  {
    public override bool IsPersistableToDB
    {
      get
      {
        if (ExpertType == null)
        {
          return base.IsPersistableToDB;
        }

        // If we are running a platform managed expert, we can not persist.
        return ExpertType.IsSubclassOf(typeof(PlatformManagedExpert)) == false;
      }
    }

    /// <summary>
    /// Local execution. Expert to be executed locally, within the platform process space and on the platforms arbiter.
    /// </summary>
    /// <param name="platformAddress"></param>
    public LocalExpertHost(string name, Type expertType)
      : base(name, expertType)
    {
    }

    /// <summary>
    /// Deserialization constructor.
    /// </summary>
    public LocalExpertHost(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }

  /// </summary>
  [Serializable]
  [ComponentManagement(false, false, int.MaxValue, true)]

  public class RemoteExpertHost : ExpertHost
  {
    public RemoteExpertHost(string expertName, Type expertType)
      : base(expertName, expertType)
    {

    }
  }
}
