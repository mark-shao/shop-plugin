using System;

namespace Hishop.Plugins.Integration
{
    public class UserEntity
    {
        private string ppp = String.Empty;
        private string tpp = String.Empty;
        private string pmsound = String.Empty;
        private string invisible = String.Empty;
        private string sigstatus = String.Empty;
        private string uid = String.Empty;

        public UserEntity(System.Data.DataTable uinfo)
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //

            if (Object.Equals(uinfo, null) || uinfo.Rows.Count != 1) return;
            this.tpp = uinfo.Rows[0][0].ToString();
            this.ppp = uinfo.Rows[0][1].ToString();
            this.pmsound = uinfo.Rows[0][2].ToString();
            this.invisible = uinfo.Rows[0][3].ToString();
            this.sigstatus = uinfo.Rows[0][4].ToString();
            this.uid = uinfo.Rows[0][5].ToString();
        }

        public string Uid
        {
            get { return this.uid; }
        }

        public string Ppp
        {
            get { return this.ppp; }
        }

        public string Tpp
        {
            get { return this.tpp; }
        }

        public string Pmsound
        {
            get { return this.pmsound; }
        }

        public string Invisible
        {
            get { return this.invisible; }
        }

        public string Sigstatus
        {
            get { return this.sigstatus; }
        }
    }
}
