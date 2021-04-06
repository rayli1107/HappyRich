using Assets;
using PlayerInfo;
using UI;
using UI.Panels.Templates;

namespace Actions
{
    public abstract class RefinanceAction : AbstractAction
    {
        private Player _player;
        private DistressedRealEstate _distressedAsset;
        private PartialRealEstate _partialRealEstate;

        public RefinanceAction(
            Player player,
            DistressedRealEstate distressedAsset,
            PartialRealEstate partialAsset,
            ActionCallback callback) : base(callback)
        {
            _player = player;
            _distressedAsset = distressedAsset;
            _partialRealEstate = partialAsset;
        }
    }
}
