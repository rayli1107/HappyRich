using Actions;
using PlayerInfo;
using PlayerState;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;

using Assets;

namespace Events.Market
{
    public static class SellRealEstateEvent
    {
        public static Action<Action> GetEvent(Player player, int index, int offer)
        {
            return cb => run(player, index, offer, cb);
        }

        private static void messageBoxHandler(
            ButtonType buttonType,
            Player player,
            int index,
            int finalOffer,
            Action callback)
        {
            Localization local = Localization.Instance;
            RentalRealEstate asset = player.portfolio.rentalProperties[index].Item2;

            string message;

            if (buttonType == ButtonType.OK)
            {
                TransactionManager.SellRentalProperty(player, index, finalOffer);
                message = string.Format(
                    "You've successfully sold the {0} property for {1}. Congratulations!",
                    local.GetRealEstateDescription(asset.description),
                    local.GetCurrency(finalOffer));
            }
            else
            {
                message = string.Format(
                    "You've decided not to sell the {0} property.",
                    local.GetRealEstateDescription(asset.description));
            }
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_ONLY,
                _ => callback?.Invoke());
        }

        private static void run(
            Player player, int index, int initialOffer, Action callback)
        {
            RentalRealEstate asset = player.portfolio.rentalProperties[index].Item2;
            PartialInvestment partialAsset = player.portfolio.rentalProperties[index].Item1;
            int finalOffer = initialOffer;

            MessageBoxHandler handler =
                b => messageBoxHandler(b, player, index, finalOffer, callback);
            UI.UIManager.Instance.ShowRealEstateSalePanel(
                asset, partialAsset, initialOffer, finalOffer, handler, false);
        }
    }
}
