using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Landi.FrameWorks.Frameworks
{
    public class FrameActivity : Activity
    {

        private static string btnHomeName;
        private static string btnReturnName;

        protected override void OnCreate()
        {
            if (!string.IsNullOrEmpty(btnHomeName))
                GetElementById(btnHomeName).Click += new HtmlElementEventHandler(HomeClick);
        }

        protected override void OnEnter()
        {
            if(!string.IsNullOrEmpty(btnReturnName))
                GetElementById(btnReturnName).Click += new HtmlElementEventHandler(ReturnClick);
        }

        private void HomeClick(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        private void ReturnClick(object sender, HtmlElementEventArgs e)
        {
            FrameReturnClick();
        }

        protected virtual void FrameReturnClick()
        {
        }

        protected override void OnLeave()
        {
            if (!string.IsNullOrEmpty(btnReturnName))
                GetElementById(btnReturnName).Click -= new HtmlElementEventHandler(ReturnClick);
        }

        public static void setBtnName(string returnName, string homeName)
        {
            if(string.IsNullOrEmpty(returnName)||string.IsNullOrEmpty(homeName))
            {
                return;
            }
            btnHomeName = homeName;
            btnReturnName = returnName;
        }

        #region 设置组件是否显示
        /// <summary>
        /// 设置组件是否显示
        /// </summary>
        /// <param name="comName">控件名</param>
        /// <param name="comFlag">是否显示控件</param>
        /// <param name="homeName">返回主页按钮名</param>
        /// <param name="retFlag">是否显示返回主页按钮</param>
        /// <param name="backName">返回上一步按钮名</param>
        /// <param name="backFlag">是否显示返回上一步按钮</param>
        protected void setComponnents(string comName, bool comFlag, bool homeFlag, bool returnFlag)
        {
            if (string.IsNullOrEmpty(comName))
            {
                Log.Error("[setComponnents] err name is null");
                return;
            }
            else
            {
                if (comFlag)
                {
                    GetElementById(comName).Style = "visibility: block;";
                    if (homeFlag)
                    {
                        GetElementById(btnHomeName).Style = "visibility: block;";
                    }
                    else
                    {
                        GetElementById(btnHomeName).Style = "visibility: hidden;";
                    }
                    if (returnFlag)
                    {
                        GetElementById(btnReturnName).Style = "visibility: block;";
                    }
                    else
                    {
                        GetElementById(btnReturnName).Style = "visibility: hidden;";
                    }
                }
                else
                {
                    GetElementById(comName).Style = "visibility: hidden;";
                }
            }
        }
        #endregion

    }
}
