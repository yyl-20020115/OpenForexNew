using System;
using System.Collections.Generic;
using System.Text;
using Rss;

namespace CommonSupport
{
    /// <summary>
    /// News event item, generated from RSS feed.
    /// </summary>
    public class RssNewsEvent : FinancialNewsEvent
    {
        string _author = String.Empty;
        [DBPersistenceToXmlData()]
        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        string _comments = String.Empty;
        [DBPersistenceToXmlData()]
        public string Comments
        {
            get { return _comments; }
            set { _comments = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public RssNewsEvent()
        {
        }

        /// <summary>
        /// Use a typical RssItem as data source.
        /// </summary>
        public RssNewsEvent(RssItem item)
        {
            // If created from an rss item this means this is a new item, not known to the DB yet.
            this.IsRead = false;

            Author = item.Author;
            Comments = item.Comments;
            Description = item.Description;
            if (item.Guid != null)
            {
                EventId = item.Guid.Name;
            }

            Link = item.Link.ToString();
            DateTime = item.PubDate;
            Title = item.Title.Trim();
        }

		///// <summary>
		///// 
		///// </summary>
		//public override int CompareTo(EventBase other)
		//{
		//    throw new NotImplementedException();

		//    //RssNewsEvent otherItem = (RssNewsEvent)other;
		//    //if (string.IsNullOrEmpty(this.EventId) == false)
		//    //{// Just compare the Ids (Guids), if they are present, since otherwise some
		//    //    // sources republish items and this causes multiplication.
		//    //    return EventId.CompareTo(otherItem.EventId);
		//    //}

		//    //int compare = base.CompareTo(other);
		//    //if (compare != 0 || other.GetType() != this.GetType())
		//    //{
		//    //    return compare;
		//    //}
            
		//    //compare = _author.CompareTo(otherItem.Author);
		//    //if (compare != 0)
		//    //{
		//    //    return compare;
		//    //}

		//    //compare = _comments.CompareTo(otherItem._comments);
		//    //if (compare != 0)
		//    //{
		//    //    return compare;
		//    //}

		//    ////compare = _comments.CompareTo(otherItem._comments);
		//    ////if (compare != 0)
		//    ////{
		//    ////    return compare;
		//    ////}

		//    //return GeneralHelper.CompareNullable(this.Link, other.Link);
		//}
    }
}
