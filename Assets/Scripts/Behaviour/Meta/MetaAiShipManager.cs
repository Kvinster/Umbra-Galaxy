using UnityEngine;

using STP.Behaviour.Starter;
using STP.Common;
using STP.State;
using STP.State.Meta;

using Random = UnityEngine.Random;

namespace STP.Behaviour.Meta {
    public sealed class MetaAiShipManager : BaseMetaComponent {
        public Transform  AiShipsRoot;
        public GameObject AiShipPrefab;
        
        StarSystemsManager _starSystemsManager;
        MetaTimeManager    _timeManager;

        TimeController _timeController;

        bool _needCreate;
        int  _nextCreationDay;

        void OnDestroy() {
            _timeController.OnCurDayChanged -= OnCurDayChanged;
        }

        public void NextStep(MetaAiShipView shipView) {
            var state = shipView.State;
            switch ( state.CurMode ) {
                case MetaAiShipMode.Moving: {
                    var ssc = StarSystemsController.Instance;
                    if ( ssc.GetFactionSystemActive(state.DestSystemId) ) {
                        state.CurMode = MetaAiShipMode.Stationary;
                        var wait = Random.Range(MetaAiShipsController.MinStationaryWait,
                            MetaAiShipsController.MaxStationaryWait + 1);
                        var curDay = _timeController.CurDay;
                        state.CurDay       = curDay;
                        state.DestDay      = curDay + wait;
                        state.CurSystemId  = state.DestSystemId;
                        state.DestSystemId = string.Empty;
                    } else {
                        var masc = MetaAiShipsController.Instance;
                        if ( masc.TryUnregisterAiShip(state.Id) ) {
                            Destroy(shipView.gameObject);
                        }
                    }
                    break;
                }
                case MetaAiShipMode.Stationary: {
                    var ssc        = StarSystemsController.Instance;
                    var neighbours = ssc.GetNeighbourStarSystemIds(state.CurSystemId);
                    neighbours.RemoveAll(x => ssc.GetStarSystemType(x) != StarSystemType.Faction);
                    neighbours.RemoveAll(x => !ssc.GetFactionSystemActive(x));
                    var curDay = _timeController.CurDay;
                    var wait   = Random.Range(MetaAiShipsController.MinStationaryWait,
                        MetaAiShipsController.MaxStationaryWait + 1);
                    if ( neighbours.Count == 0 ) {
                        state.CurMode = MetaAiShipMode.Stationary;
                        state.CurDay  = curDay;
                        state.DestDay = curDay + wait;
                    } else {
                        var dest     = neighbours[Random.Range(0, neighbours.Count)];
                        var distance = ssc.GetDistance(state.CurSystemId, dest);
                        state.CurMode      = MetaAiShipMode.Moving;
                        state.CurDay       = curDay;
                        state.DestDay      = curDay + distance;
                        state.DestSystemId = dest;
                    }
                    break;
                }
                default: {
                    Debug.LogErrorFormat("Unsupported MetaAiShipMode '{0}'", state.CurMode.ToString());
                    return;
                }
            }
        }

        public void Kill(MetaAiShipView aiShipView) {
            var masc = MetaAiShipsController.Instance;
            if ( masc.TryUnregisterAiShip(aiShipView.Id) ) {
                Destroy(aiShipView.gameObject);
            }
        }

        protected override void InitInternal(MetaStarter starter) {
            _starSystemsManager = starter.StarSystemsManager;
            _timeManager        = starter.TimeManager;
            
            _timeController = TimeController.Instance;
            _timeController.OnCurDayChanged += OnCurDayChanged;
            
            var controller = MetaAiShipsController.Instance;
            
            var aiShipStates = controller.GetAiShipsStates();
            foreach ( var aiShipState in aiShipStates ) {
                CreateAiShipInstance(aiShipState);
            }

            _needCreate      = (aiShipStates.Count < MetaAiShipsController.MaxAiShips);
            _nextCreationDay = controller.LastAiShipCreatedDay + MetaAiShipsController.MinDaysBetweenAiShipsCreation;
        }

        void OnCurDayChanged(int curDay) {
            if ( _needCreate && (curDay >= _nextCreationDay) && TryCreateNewAiShip() ) {
                var masc = MetaAiShipsController.Instance;
                masc.LastAiShipCreatedDay = curDay;
                _needCreate      = (masc.GetAiShipsStates().Count < MetaAiShipsController.MaxAiShips);
                _nextCreationDay = masc.LastAiShipCreatedDay + MetaAiShipsController.MinDaysBetweenAiShipsCreation;
            }
        }

        bool TryCreateNewAiShip() {
            var masc             = MetaAiShipsController.Instance;
            var ssc              = StarSystemsController.Instance;
            var state            = masc.CreateAiShipState();
            var curDay           = _timeController.CurDay;
            var factionSystemIds = ssc.GetFactionSystemIds();
            factionSystemIds.RemoveAll(x => !ssc.GetFactionSystemActive(x));
            var factionSystemId = factionSystemIds[Random.Range(0, factionSystemIds.Count)];
            state.CurMode     = MetaAiShipMode.Stationary;
            state.CurSystemId = factionSystemId;
            state.DestDay     = curDay + 1;
            var view = CreateAiShipInstance(state);
            if ( !view ) {
                return false;
            }
            if ( !masc.TryRegisterAiShip(state) ) {
                Debug.LogError("Can't create new ai ship");
                Destroy(view.gameObject);
                return false;
            }
            return true;
        }

        MetaAiShipView CreateAiShipInstance(MetaAiShipState state) {
            var aiShipGo   = Instantiate(AiShipPrefab, AiShipsRoot, false);
            var aiShipView = aiShipGo.GetComponent<MetaAiShipView>();
            if ( !aiShipView ) {
                Debug.LogError("No MetaAiShipView component on prefab");
                Destroy(aiShipGo);
                return null;
            }
            aiShipView.Init(state, this, _starSystemsManager, _timeManager);
            switch ( state.CurMode ) {
                case MetaAiShipMode.Moving: {
                    var prevSystem = _starSystemsManager.GetStarSystem(state.CurSystemId);
                    var destSystem = _starSystemsManager.GetStarSystem(state.DestSystemId);
                    var dist = StarSystemsController.Instance.GetDistance(prevSystem.Id, destSystem.Id);
                    aiShipView.transform.position = Vector3.Lerp(prevSystem.transform.position,
                        destSystem.transform.position, (float)state.CurDay / dist);
                    break;
                }
                case MetaAiShipMode.Stationary: {
                    var curSystem = _starSystemsManager.GetStarSystem(state.CurSystemId);
                    aiShipView.transform.position = curSystem.transform.position;
                    break;
                }
                default: {
                    Debug.LogErrorFormat("Unsupported MetaAiShipMode '{0}'", state.CurMode.ToString());
                    Destroy(aiShipGo);
                    return null;
                }
            }
            return aiShipView;
        }
    }
}
