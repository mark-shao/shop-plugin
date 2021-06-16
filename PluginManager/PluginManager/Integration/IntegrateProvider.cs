using System;

namespace Hishop.Plugins.Integration
{
    public abstract class IntegrateProvider
    {
        public static IntegrateProvider Instance(string applicationType, string configStr)
        {
            Type type = Type.GetType(applicationType);
            IntegrateProvider instance = Activator.CreateInstance(type) as IntegrateProvider;

            instance.Init(configStr);

            return instance;
        }

        /// <summary>
        /// 初始化配置信息
        /// </summary>
        /// <param name="node"></param>
        protected abstract void Init(string configStr);

        /// <summary>
        /// 同步注册
        /// </summary>
        /// <param name="username">帐号</param>
        /// <param name="gender">性别</param>
        /// <param name="password">密码(未加密)</param>
        /// <param name="email">邮箱</param>
        /// <param name="regip">注册IP</param>
        /// <param name="qq">QQ号</param>
        /// <param name="msn">MSN</param>
        public abstract void Register(string username, int gender, string password, string email, string regip, string qq, string msn);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="username">帐号</param>
        /// <param name="password">密码(未加密)</param>
        /// <returns>是否修改成功</returns>
        public abstract void ChangePassword(string username, string password);

        /// <summary>
        /// 同步登陆
        /// </summary>
        /// <param name="username">帐号</param>
        /// <param name="password">密码(未加密)</param>
        /// <param name="reFerer">返回页面</param>
        public abstract void Login(string username, string password, string returnUrl);

        /// <summary>
        /// 管理员登录
        /// </summary>
        /// <param name="username">帐号</param>
        /// <param name="password">密码(未加密)</param>
        /// <param name="reFerer">返回页面</param>
        public abstract void AdminLogin(string username, string password, string returnUrl);

        /// <summary>
        /// 同步退出
        /// </summary>
        public abstract void Logout();

        /// <summary>
        /// 通过用户名获取用户ID
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public abstract int GetUserID(string username);

        /// <summary>
        /// 删除指定用户的所有信息
        /// </summary>
        /// <param name="userId">指定的用户编号</param>
        public abstract void DeleteUser(int userId);
    }
}
