using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.PlayerDetails
{
    public class CharacterSelectionPanel : ModalObject
    {
#pragma warning disable 0649
        [SerializeField]
        private Image  _image;
        [SerializeField]
        private TextMeshProUGUI _textProfession;
        [SerializeField]
        private TextMeshProUGUI _textAnnualIncome;
        [SerializeField]
        private TextMeshProUGUI _textAnnualExpense;
        [SerializeField]
        private TextMeshProUGUI _textStartingAge;
        [SerializeField]
        private TextMeshProUGUI _textStartingCash;
        [SerializeField]
        private TextMeshProUGUI _textAutoLoan;
        [SerializeField]
        private TextMeshProUGUI _textStudentLoan;
        [SerializeField]
        private TextMeshProUGUI _textChildExpenses;
#pragma warning restore 0649

        public List<Profession> professions;
        public int professionIndex;
        public Action<Profession> selectionCallback;

        public void Refresh()
        {
            if (professionIndex >= 0 && professionIndex < professions.Count)
            {
                Profession profession = professions[professionIndex];
                Localization local = Localization.Instance;

                if (_image != null)
                {
                    _image.sprite = profession.image;
                }

                if (_textProfession != null)
                {
                    _textProfession.text = local.GetJobName(profession);
                }

                if (_textAnnualIncome != null)
                {
                    _textAnnualIncome.text = local.GetCurrency(profession.salary);
                }

                if (_textAnnualExpense != null)
                {
                    _textAnnualExpense.text = local.GetCurrency(profession.personalExpenses, true);
                }

                if (_textStartingAge != null)
                {
                    _textStartingAge.text = profession.startingAge.ToString(); ;
                }

                if (_textStartingCash != null)
                {
                    _textStartingCash.text = local.GetCurrencyPlain(profession.startingCash);
                }

                if (_textAutoLoan != null)
                {
                    _textAutoLoan.text = local.GetCurrency(profession.autoLoan, true);
                }

                if (_textStudentLoan != null)
                {
                    _textStudentLoan.text = local.GetCurrency(profession.jobCost, true);
                }

                if (_textChildExpenses != null)
                {
                    _textChildExpenses.text = local.GetCurrency(profession.costPerChild, true);
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Refresh();
        }

        public void OnPrevButton()
        {
            int count = professions.Count;
            professionIndex = (professionIndex + count - 1) % count;
            Refresh();
        }

        public void OnNextButton()
        {
            professionIndex = (professionIndex + 1) % professions.Count;
            Refresh();
        }

        public void OnSelect()
        {
            if (selectionCallback != null)
            {
                selectionCallback.Invoke(professions[professionIndex]);
            }
            Destroy();
        }
    }
}
