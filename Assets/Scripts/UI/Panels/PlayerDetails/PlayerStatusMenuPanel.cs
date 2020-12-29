﻿using System;
using UnityEngine;

namespace UI.Panels.PlayerDetails
{
    public class PlayerStatusMenuPanel : MonoBehaviour
    {
        public Player player;

        public void ShowAssetLiabilityStatusPanel()
        {
            UIManager.Instance.ShowAssetLiabilityStatusPanel();
        }

        public void ShowIncomeExpenseStatusPanel()
        {
            UIManager.Instance.ShowIncomeExpenseStatusPanel();
        }

        public void ShowJobListPanel()
        {
            UIManager.Instance.ShowJobListPanel(JobListPanelMode.kCurrentJobs);
        }

        public void ShowContactListPanel()
        {
            UIManager.Instance.ShowContactListPanel();
        }

        public void ShowHappinessListPanel()
        {
            UIManager.Instance.ShowHappinessListPanel();
        }
    }
}
