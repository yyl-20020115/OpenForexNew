using System;
using System.Collections.Generic;
using System.Text;
using Rss;
using System.Net;
using System.Security;
using System.Drawing;

namespace CommonSupport
{
    /// <summary>
    /// Default RSS news source; delivers news items from web RSS feeds.
    /// </summary>
    [EventItemType(typeof(RssNewsEvent))]
    public class RssNewsSource : EventSource
    {
        volatile RssFeed _feed;

        /// <summary>
        /// Constructor, also needed for persistence.
        /// </summary>
        public RssNewsSource()
        {
        }

        /// <summary>
        /// Initialize source for operation.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool Initialize(string address)
        {
            Address = address;
            return true;
        }

        /// <summary>
        /// Actual updating of items happens here.
        /// </summary>
        void DoUpdateItems()
        {
            if (_feed == null)
            {
                return;
            }

            List<RssChannel> channels;
            lock (this)
            {
                channels = GeneralHelper.EnumerableToList<RssChannel>(_feed.Channels);
            }

            foreach (RssChannel channel in channels)
            {
                // Obtain or create channel.
                EventSourceChannel sourceChannel = base.GetChannelByName(channel.Title, true);
                sourceChannel.ItemsUpdateEnabled = false;
                sourceChannel.Address = channel.Link.ToString();

                List<EventBase> newItems = new List<EventBase>();
                foreach (RssItem item in channel.Items)
                {
                    RssNewsEvent eventItem = new RssNewsEvent(item);
                    eventItem.Initialize(sourceChannel);
                    newItems.Add(eventItem);
                }
                
                // Add all by default to the "Default" channel, since RSS feeds never seem to bother with proper inner channels.
                sourceChannel.AddItems(newItems);
            }
        }

        /// <summary>
        /// On update, update items.
        /// </summary>
        protected override void OnUpdate()
        {
            try
            {
                if (_feed == null)
                {
                    _feed = RssFeed.Read(base.Address);

                    if (_feed.Channels.Count == 1)
                    {
                        // Some feeds have those symbols in their names.
                        _name = _feed.Channels[0].Title.Replace("\r", "").Replace("\n", "").Trim();
                    }
                    else
                    {
                        _name = _feed.Url.ToString();
                    }

                    List<string> names = new List<string>();
                    foreach (RssChannel channel in _feed.Channels)
                    {
                        names.Add(channel.Title);
                    }

                    foreach (string name in names)
                    {
                        if (ChannelsNames.Contains(name) == false)
                        {
                            EventSourceChannel channel = new EventSourceChannel(name, true);
                            channel.Initialize(this);
                            base.AddChannel(channel);
                        }
                    }

                }
                else
                {
                    _feed = RssFeed.Read(_feed);
                }

                //OperationalStateEnum newState = OperationalStateEnum.Operational;
            }
            catch (WebException we)
            {// Feed not found or some other problem.
                SystemMonitor.OperationWarning("Failed to initialize feed [" + Address + ", " + we.Message + "]");
                ChangeOperationalState(OperationalStateEnum.NotOperational);
            }
            catch (Exception ex)
            {// RssFeed class launches IOExceptions too, so get safe here.
                SystemMonitor.OperationWarning("Failed to initialize feed [" + Address + ", " + ex.Message + "]");
                ChangeOperationalState(OperationalStateEnum.NotOperational);
            }

            DoUpdateItems();

            //RaisePersistenceDataUpdatedEvent();
        }
    }
}
