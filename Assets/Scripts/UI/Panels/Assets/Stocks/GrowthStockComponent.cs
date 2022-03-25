using Actions;
using Assets;
using PlayerInfo;
using ScriptableObjects;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class GrowthStockComponent : StockPanel
    {
#pragma warning disable 0649
        [SerializeField]
        private ItemValuePanel _panelEstimatedValue;
#pragma warning restore 0649

        public GrowthStock growthStock;
        public override AbstractStock stock => growthStock;

        public override void Refresh()
        {
            base.Refresh();

            if (_panelEstimatedValue != null)
            {
                bool evaluate = player.HasSkill(SkillType.STOCK_EVALUATION);
                _panelEstimatedValue.gameObject.SetActive(evaluate);
                if (evaluate)
                {
                    int value = Mathf.RoundToInt(growthStock.basePrice);
                    _panelEstimatedValue.SetValue(
                        Localization.Instance.GetCurrencyPlain(value));
                }
            }
        }
    }
}