#region Author & Verson
// Name : UITip.cs
// Author : GuanRen
// CreateTime : 2024/09/10
// Job : 提示UI
#endregion

using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UITip : UIController
    {
        [SerializeField] private Text m_Content;
        [SerializeField] private Button m_ConfirmBtn;
        [SerializeField] private Button m_CancelBtn;
        [SerializeField] private Text m_ConfirmText;
        [SerializeField] private Text m_CancelText;

        public void Init(string content, string confirmBtnText, string cancelBtnText, 
            System.Action confirmAction, System.Action cancelAction)
        {
            m_Content.text = content;
            m_ConfirmText.text = confirmBtnText;
            m_CancelText.text = cancelBtnText;
            m_ConfirmBtn.onClick.AddListener(() =>
            {
                confirmAction?.Invoke();
                UIManager.Instance.Close(this);
            });
            m_CancelBtn.onClick.AddListener(() =>
            {
                cancelAction?.Invoke();
                UIManager.Instance.Close(this);
            });
        }
    }
}