using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Nitro.Framework.Goliath.FrontEnd
{
    public class ListViewAutoMapped : ListView
    {
        ListViewAutoMapper _mapper = new ListViewAutoMapper();

        public ListViewAutoMapper Mapper
        {
            get { return _mapper; }
            set { _mapper = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ListViewAutoMapped()
        {
            _mapper.ListView = this;

            base.FullRowSelect = true;
            base.BorderStyle = BorderStyle.FixedSingle;
            base.View = View.Details;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

        }

    }
}
