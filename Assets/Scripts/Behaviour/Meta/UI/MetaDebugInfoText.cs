using System.Text;

using STP.Behaviour.Starter;
using STP.State.Meta;

using TMPro;

namespace STP.Behaviour.Meta.UI {
    public sealed class MetaDebugInfoText : BaseMetaComponent {
        public TMP_Text Text;

        MetaTimeManager       _timeManager;
        PlayerShip            _playerShip;
        StarSystemsController _starSystemsController;

        readonly StringBuilder _stringBuilder = new StringBuilder();

        void Update() {
            if ( !IsInit ) {
                return;
            }
            UpdateText();
        }
        
        protected override void InitInternal(MetaStarter starter) {
            _timeManager           = starter.TimeManager;
            _playerShip            = starter.PlayerShip;
            _starSystemsController = starter.StarSystemsController;
            UpdateText();
        }

        void UpdateText() {
            _stringBuilder.Clear();
            _stringBuilder.AppendLine("<size=150%>Time</size>")
                .AppendLine($"IsPaused: {_timeManager.IsPaused}")
                .AppendLine($"CurDay: {_timeManager.CurDay}");
            if ( !_timeManager.IsPaused ) {
                _stringBuilder.AppendLine($"DayProgress: {_timeManager.DayProgress:0.00}");
            }
            _stringBuilder.AppendLine();

            _stringBuilder.AppendLine("<size=150%>Player</size>");
            if ( _playerShip.MovementController.CurSystem ) {
                var ssc = _starSystemsController;
                _stringBuilder.AppendLine(
                    $"CurSystem: {ssc.GetStarSystemName(_playerShip.MovementController.CurSystem.Id)}");
                if ( _playerShip.MovementController.IsMoving ) {
                    _stringBuilder.AppendLine(
                        $"DestSystem: {ssc.GetStarSystemName(_playerShip.MovementController.DestSystem.Id)}");
                }
            }
            
            Text.text = _stringBuilder.ToString();
        }
    }
}
