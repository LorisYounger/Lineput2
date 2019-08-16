using System;
using System.Windows.Documents;
using LinePutScript;
using System.Windows.Controls;

namespace LineputPlus
{
    /// <summary>
    /// LPT播放器:Lineput内容
    /// </summary>
    public class LPTPlayer : LptDocument
    {
        public RichTextBox Document;
        public LineDisplay IADisplay;
        public LPTPlayer(RichTextBox fdm, string LPT, int start = 1) : base(LPT)
        {
            Document = fdm;
            LineNode = start;

        }
        /// <summary>
        /// 文本置换
        /// </summary>
        /// <param name="Reptex">原文本</param>
        public void TextDeReplace(ref string Reptex)
        {
            Reptex = Reptex.Replace("/now", DateTime.Now.ToString());
            Reptex = Reptex.Replace("/date", DateTime.Now.ToShortDateString());
            Reptex = Reptex.Replace("/time", DateTime.Now.ToShortTimeString());
        }
        /// <summary>
        /// 控制显示委托 可用于插件,实现特殊功能等
        /// </summary>
        /// <param name="order">指令</param>
        /// <param name="ldp">预备显示内容</param>
        /// <param name="fd">流文档(输出端口</param>
        public void PlayDisplay(Sub order, ref LineDisplay ldp, ref FlowDocument fd)
        {
            TextDeReplace(ref ldp.OutPut);
            switch (order.Name.ToLower())
            {
                case "":
                    break;
            }
        }
    }
}
